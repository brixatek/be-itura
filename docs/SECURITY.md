# ITURA — Security Architecture & Compliance

**Document Version:** 1.0  
**Owner:** Security Engineering / Compliance  
**Last Updated:** May 2026  
**Classification:** Internal — Engineering & Leadership

---

## Table of Contents

1. [Security Philosophy](#1-security-philosophy)
2. [RBAC Design](#2-rbac-design)
3. [JWT Strategy](#3-jwt-strategy)
4. [Encryption Strategy](#4-encryption-strategy)
5. [Data Privacy Controls](#5-data-privacy-controls)
6. [Audit Logging](#6-audit-logging)
7. [Fraud Prevention](#7-fraud-prevention)
8. [Rate Limiting & API Protection](#8-rate-limiting--api-protection)
9. [NDPR Compliance](#9-ndpr-compliance)
10. [GDPR Compliance](#10-gdpr-compliance)
11. [HIPAA-Aligned Considerations](#11-hipaa-aligned-considerations)
12. [Threat Model](#12-threat-model)
13. [Security Testing](#13-security-testing)
14. [Incident Response](#14-incident-response)

---

## 1. Security Philosophy

### Zero Trust Principles

Itura operates on the principle that **no component, user, or service is trusted by default** — trust is earned and re-verified continuously.

| Principle | Implementation |
|---|---|
| **Verify explicitly** | Every request authenticated and authorized, even internal service calls |
| **Least privilege** | Users, services, and infrastructure access only what they need |
| **Assume breach** | Design systems to contain and detect breaches, not just prevent them |
| **Encrypt everything** | Data encrypted at rest and in transit, always |
| **Log everything** | Full audit trail of all sensitive operations |

### Security Layers

```
Layer 1: Network Edge
  ├── Azure Front Door (WAF, DDoS, TLS termination)
  └── Network Security Groups (restrict inbound traffic)

Layer 2: Application Gateway
  ├── YARP Reverse Proxy
  ├── Rate limiting (Redis sliding window)
  ├── Request validation
  └── CORS policy enforcement

Layer 3: API Authentication
  ├── JWT validation (RS256)
  ├── Token revocation check (Redis blacklist)
  └── Role/permission enforcement

Layer 4: Service Mesh (Istio)
  ├── mTLS between all microservices
  └── Service-to-service authorization policies

Layer 5: Data Layer
  ├── Row-level security (PostgreSQL RLS)
  ├── Column-level encryption (PII fields)
  ├── AES-256-GCM for Tier-1 sensitive data (journals, session notes)
  └── Azure Key Vault for key management

Layer 6: Monitoring
  ├── SIEM (Microsoft Sentinel)
  ├── Anomaly detection
  └── Real-time alerting
```

---

## 2. RBAC Design

### Roles

| Role | Description | Assigned To |
|---|---|---|
| `User` | Standard platform user | Individual registered users |
| `Coach` | Licensed coach/therapist | Verified coaches |
| `CorporateAdmin` | Corporate HR administrator | Corporate account admins |
| `CorporateEmployee` | Employee under corporate account | Corporate team members |
| `Moderator` | Community content moderator | Trusted community moderators |
| `Admin` | Platform super administrator | Internal Itura staff |
| `SuperAdmin` | Unrestricted system access | CEO + CTO only |

### Permission Matrix

| Resource | User | Coach | CorpAdmin | Moderator | Admin |
|---|---|---|---|---|---|
| Own profile | CRUD | CRUD | CRUD | CRUD | CRUD |
| Other user profile | R (limited) | R (own clients) | R (own employees) | None | CRUD |
| Coach profiles | R (public) | CRUD (own) | R | None | CRUD |
| Bookings | CRUD (own) | R (own sessions) | Create (for employees) | None | CRUD |
| Journal entries | CRUD (own) | R (if shared) | None | None | None |
| Mood entries | CRUD (own) | R (if shared) | Aggregate stats | None | None |
| Community posts | CRUD (own) | CRUD (own) | R | CRUD | CRUD |
| Payments | R (own) | R (own earnings) | CRUD (corp billing) | None | CRUD |
| Subscriptions | RU (own) | None | CRUD (corp seats) | None | CRUD |
| Admin panel | None | None | Corp dashboard only | Moderation queue | Full |
| AI conversations | CRUD (own) | None | None | None | None (PII) |
| Audit logs | None | None | None | None | R |
| Platform settings | None | None | None | None | CRUD |

### Permission Enforcement (Defense in Depth)

```csharp
// Layer 1: Controller-level policy
[Authorize(Policy = "RequireCoachRole")]
[HttpGet("me/clients")]
public async Task<IActionResult> GetClients() { ... }

// Layer 2: Resource-based authorization
public class BookingAuthorizationHandler : AuthorizationHandler<BookingOwnerRequirement, Booking>
{
    protected override Task HandleRequirementAsync(...)
    {
        if (booking.UserId == user.GetUserId() || user.IsInRole("Admin"))
            context.Succeed(requirement);
        return Task.CompletedTask;
    }
}

// Layer 3: Database query filter (tenant isolation)
// No data returned for wrong tenant even if auth passes

// Layer 4: Field-level: sensitive fields omitted from DTO
// Journal content excluded from list views
// PII excluded from analytics DTOs
```

---

## 3. JWT Strategy

### Token Specifications

| Property | Access Token | Refresh Token |
|---|---|---|
| Format | JWT (RS256) | Opaque random string |
| Length | ~500 bytes | 256 bits (hex-encoded) |
| Expiry | 15 minutes | 30 days |
| Storage (web) | Memory (JS) | HttpOnly cookie |
| Storage (mobile) | Memory | flutter_secure_storage |
| Revocation | Token ID blacklist in Redis | DB lookup + revocation flag |
| Rotation | Issued fresh on every refresh | Rotated on every use |

### JWT Claims Structure

```json
{
  "sub": "usr_01H7Y3KRJM",
  "jti": "tok_01H9AB2CD3",          // unique token ID
  "email": "amara@example.com",
  "role": "User",
  "tier": "Pro",
  "tenantId": null,
  "perms": ["mood:rw", "journal:rw", "booking:rw", "ai:rw"],
  "iat": 1716998400,
  "exp": 1716999300,
  "iss": "https://auth.itura.app",
  "aud": "https://api.itura.app"
}
```

### Token Revocation

**Problem:** JWTs are stateless and valid until expiry.  
**Solution:** Maintain a revocation list in Redis.

```csharp
// On logout: add jti to revocation set
await _redis.SetAddAsync("jwt:revoked", token.Jti);
await _redis.KeyExpireAsync("jwt:revoked", TimeSpan.FromMinutes(15)); // TTL matches access token

// On every request: check blacklist
var isRevoked = await _redis.SetContainsAsync("jwt:revoked", jti);
if (isRevoked) return Unauthorized();
```

### Key Rotation Strategy

- RS256 key pair stored in Azure Key Vault
- New key pair generated quarterly
- Old key retained for 15 minutes (access token lifetime) to avoid rejecting valid tokens
- JWKS endpoint exposed: `https://auth.itura.app/.well-known/jwks.json`
- API Gateway validates token against JWKS

---

## 4. Encryption Strategy

### Data Classification & Controls

| Tier | Data Types | Encryption Method |
|---|---|---|
| **Critical** | Journal content, AI conversations, session notes | AES-256-GCM (application-layer), key in Azure Key Vault |
| **Sensitive PII** | Email, phone, full name, bank account, DoB | Azure PostgreSQL TDE + column-level encryption |
| **Health Data** | Mood scores, assessment responses, wellness data | TDE + access controls + audit logging |
| **Standard** | Coach profiles, booking metadata, community posts | TDE at rest, TLS in transit |
| **Public** | Platform content, public coach profiles | TLS in transit only |

### AES-256-GCM Implementation (Journals)

```csharp
public class DataEncryptionService : IDataEncryptionService
{
    private readonly IKeyVaultClient _keyVault;
    private const string KeyId = "journal-encryption-key";

    public async Task<string> EncryptAsync(string plaintext)
    {
        var key = await _keyVault.GetSecretAsync(KeyId);
        var keyBytes = Convert.FromBase64String(key.Value);

        using var aes = new AesGcm(keyBytes, AesGcm.TagByteSizes.MaxSize);
        var nonce = RandomNumberGenerator.GetBytes(AesGcm.NonceByteSizes.MaxSize);
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize];

        aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        // Encode: Base64(nonce || tag || ciphertext)
        var combined = nonce.Concat(tag).Concat(ciphertext).ToArray();
        return Convert.ToBase64String(combined);
    }

    public async Task<string> DecryptAsync(string encryptedData)
    {
        var key = await _keyVault.GetSecretAsync(KeyId);
        var keyBytes = Convert.FromBase64String(key.Value);
        var data = Convert.FromBase64String(encryptedData);

        var nonceSize = AesGcm.NonceByteSizes.MaxSize;
        var tagSize = AesGcm.TagByteSizes.MaxSize;

        var nonce = data[..nonceSize];
        var tag = data[nonceSize..(nonceSize + tagSize)];
        var ciphertext = data[(nonceSize + tagSize)..];
        var plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(keyBytes, AesGcm.TagByteSizes.MaxSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }
}
```

### TLS Configuration

```csharp
// Enforce TLS 1.3 minimum
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(https =>
    {
        https.SslProtocols = SslProtocols.Tls13;
        https.ClientCertificateMode = ClientCertificateMode.NoCertificate;
    });
});
```

### mTLS Between Services (Istio)

```yaml
# Istio PeerAuthentication: require mTLS for all service communication
apiVersion: security.istio.io/v1beta1
kind: PeerAuthentication
metadata:
  name: default
  namespace: itura-prod
spec:
  mtls:
    mode: STRICT

# AuthorizationPolicy: booking-service can only call coach-service
apiVersion: security.istio.io/v1beta1
kind: AuthorizationPolicy
metadata:
  name: coach-service-policy
spec:
  selector:
    matchLabels:
      app: coach-service
  rules:
    - from:
        - source:
            principals:
              - cluster.local/ns/itura-prod/sa/booking-service
              - cluster.local/ns/itura-prod/sa/admin-service
```

---

## 5. Data Privacy Controls

### PII Inventory

| Data Field | Location | Access Controls |
|---|---|---|
| Email address | `accounts.email` | Encrypted at rest; only auth-service reads |
| Phone number | `accounts.phone_number` | Encrypted at rest; auth + notification services |
| Full name | `user_profiles` | TDE; filtered in public-facing DTOs |
| Date of birth | `user_profiles` | TDE; age bracket shown publicly, not DoB |
| Location | `user_profiles` | City level only stored; country for routing |
| Journal content | `journal_entries.content_encrypted` | AES-256-GCM; user-only access |
| Mood notes | `mood_entries.note` | TDE; user-only access |
| Bank account number | `coach_payouts` | AES-256; decrypted only at payout time |
| AI conversations | MongoDB | User-only access; 90-day retention default |
| IP addresses | Audit logs | Hashed in analytics; retained 90 days |
| Session recordings | Azure Blob | User-only access; explicit consent required |

### Data Minimization

- Only collect data strictly necessary for service delivery
- AI companion: conversation context summarized rather than full raw history retained indefinitely
- Analytics events: user IDs hashed (SHA-256 with salt), no PII in events
- Error logs: PII scrubbed before logging (email → hash, names → [REDACTED])

### Data Retention Policy

| Data Type | Retention Period | Deletion Method |
|---|---|---|
| User account + profile | Account lifetime + 90 days after deletion | Hard delete after 90 days |
| Journal entries | Account lifetime + 90 days | AES key destruction + hard delete |
| Mood entries | Account lifetime + 90 days | Hard delete |
| AI conversations | 90 days (default); user can set shorter | Hard delete |
| Session recordings | 30 days (default); user can delete sooner | Blob deletion |
| Payment records | 7 years (financial compliance) | Anonymize PII, retain financial data |
| Audit logs | 3 years | Archive to cold storage after 1 year |
| IP addresses in logs | 90 days | Auto-purge |

### Right to Erasure (GDPR Article 17)

```
User requests account deletion:
  1. User clicks "Delete Account" in Settings
  2. Confirmation dialog with consequences explained
  3. Account marked deleted_at (soft delete)
  4. PII anonymized immediately:
     - email → deleted_{userId}@deleted.itura.app
     - name → "Deleted User"
     - phone → null
     - avatar → null
     - journal content → AES key deleted (effectively destroys content)
  5. Session tokens invalidated
  6. 90 days later: hard delete of all records
  7. Analytics events: UUID reference remains (for cohort stats), no PII
  8. Community posts: content replaced with "[Post by a deleted user]"
     (post structure retained for thread continuity)
  9. Confirmation email sent to email address on file (before anonymization)
```

---

## 6. Audit Logging

### What Is Audited

| Event Category | Events |
|---|---|
| **Authentication** | Login (success/fail), logout, password reset, MFA enable/disable, account lockout |
| **Account Management** | Registration, profile update, email change, account deletion |
| **Access Control** | Role change, admin access, coach approval/rejection/suspension |
| **Data Access (PII)** | Journal read (by whom), mood data export, AI conversation access |
| **Financial** | Every payment, refund, payout, wallet credit/debit |
| **Content Moderation** | Post removal, user ban, report actioned |
| **Admin Actions** | Every admin action logged with admin ID, timestamp, affected resource |
| **Crisis Events** | AI crisis detection trigger, escalation actions |
| **Security Events** | Failed auth attempts, token revocation, suspicious IP |

### Audit Log Schema

```json
{
  "id": "aud_01H9AB...",
  "timestamp": "2026-05-22T10:30:45.123Z",
  "eventType": "auth.login.success",
  "category": "authentication",
  "actorId": "usr_01H7Y3...",     // who did it (hashed for non-admin view)
  "actorRole": "User",
  "targetId": "usr_01H7Y3...",    // what was affected
  "targetType": "account",
  "action": "login",
  "result": "success",
  "ipAddress": "105.112.x.x",    // hashed in exports
  "userAgent": "Mozilla/5.0...",
  "metadata": {
    "deviceName": "Chrome on Windows",
    "mfaUsed": false
  },
  "service": "auth-service",
  "correlationId": "req_01H9A1..."
}
```

### Audit Log Immutability

- Audit logs written to append-only log table (no UPDATE/DELETE permissions for service accounts)
- Logs replicated to Azure Monitor Log Analytics (immutable)
- After 90 days: archived to Azure Blob immutable storage (WORM — Write Once Read Many)
- Admin cannot delete audit logs (only Super Admin with dual authorization)

---

## 7. Fraud Prevention

### Payment Fraud Controls

| Control | Implementation |
|---|---|
| Velocity limits | Max 3 payments per hour per user; max ₦500,000 per day per user |
| Device fingerprinting | Track device hash per payment; alert on new device |
| Paystack Radar | AI-based fraud detection on Paystack's side |
| Manual review threshold | Payments > ₦100,000 flagged for manual review |
| Chargebacks | Monitor chargeback rate; > 1% triggers review |
| Coach payout verification | New bank accounts held 48 hours before first payout |

### Account Fraud Controls

| Control | Implementation |
|---|---|
| Email verification | Required before any paid actions |
| Phone verification | Required for coach accounts |
| Rate limiting on registration | 10 per IP per hour |
| Disposable email blocking | Blocklist of disposable email domains |
| Credential stuffing protection | CAPTCHA after 3 failed logins; lockout after 5 |
| Impossible travel detection | Login from 2 distant locations in < 2 hours → MFA required |

### Community Abuse Controls

| Control | Implementation |
|---|---|
| AI pre-moderation | Every post screened before publication |
| Link sharing limits | Links not allowed in first 7 days of account |
| New user rate limit | Max 5 posts/day for accounts < 7 days old |
| Spam detection | Duplicate content detection (cosine similarity > 0.9 = spam) |
| IP-based posting limits | 50 posts/day per IP across all accounts |

---

## 8. Rate Limiting & API Protection

### Rate Limiting Implementation

```csharp
// Redis sliding window rate limiter
public class RateLimitMiddleware
{
    private readonly RedisRateLimiter _limiter;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var key = GetRateLimitKey(context);
        var limit = GetUserLimit(context.User);
        var window = TimeSpan.FromMinutes(1);

        var (allowed, remaining, reset) = await _limiter.CheckAsync(key, limit, window);

        context.Response.Headers["X-RateLimit-Limit"] = limit.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
        context.Response.Headers["X-RateLimit-Reset"] = reset.ToString();

        if (!allowed)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsJsonAsync(new { error = "RATE_LIMITED" });
            return;
        }

        await next(context);
    }
}
```

### WAF Rules (Azure Front Door)

```
OWASP Core Rule Set 3.2:
  ├── SQL Injection prevention
  ├── XSS prevention
  ├── Remote File Inclusion
  ├── Remote Code Execution
  └── Protocol attack prevention

Custom Rules:
  ├── Block requests with suspicious User-Agent patterns
  ├── Block requests from known malicious IPs (AbuseIPDB integration)
  ├── Block oversized request bodies (> 1MB for most endpoints)
  ├── Geo-blocking (optional, configurable per endpoint)
  └── Bot mitigation (Microsoft Bot Manager rule set)
```

### API Security Headers

```csharp
// Security headers middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'");
    context.Response.Headers.Add("Permissions-Policy",
        "camera=(), microphone=(), geolocation=()");
    context.Response.Headers.Add("Strict-Transport-Security",
        "max-age=31536000; includeSubDomains; preload");
    await next();
});
```

---

## 9. NDPR Compliance

### Nigeria Data Protection Regulation Requirements

| Requirement | Itura Implementation |
|---|---|
| **Lawful basis** | Explicit consent obtained at registration; stored with timestamp |
| **Notice** | Privacy notice presented before data collection; version-tracked |
| **Data minimization** | Only necessary data collected; periodic data audit |
| **Data subject rights** | Access, correction, deletion, portability via Settings UI |
| **Data breach notification** | NITDA notified within 72 hours of confirmed breach |
| **DPO** | Designated Data Protection Officer appointed from Day 1 |
| **Data transfer** | Standard Contractual Clauses for international transfers |
| **DPIA** | Data Protection Impact Assessment completed before launch |
| **Retention** | Defined retention periods per data type |
| **Third-party processors** | Data Processing Agreements with all vendors |

### Consent Management

```
Registration flow:
  1. User sees Privacy Policy + Terms of Service links (required read)
  2. Checkbox: "I agree to the Privacy Policy and Terms of Service" (mandatory)
  3. Checkbox: "I agree to receive marketing communications" (optional)
  4. Timestamp + IP + version of policy stored with consent record

Consent record:
{
  "userId": "usr_01H7...",
  "consentType": "privacy_policy",
  "version": "1.2",
  "granted": true,
  "timestamp": "2026-05-22T08:15:00Z",
  "ipAddress": "105.112.x.x",
  "method": "web_registration"
}
```

---

## 10. GDPR Compliance

### User Rights Implementation

| Right | Endpoint | Implementation |
|---|---|---|
| Right to Access | `GET /api/v1/users/me/data-export` | JSON export of all personal data, generated async, emailed |
| Right to Rectification | `PUT /api/v1/users/me` | Profile update API |
| Right to Erasure | `DELETE /api/v1/users/me` | Full deletion pipeline (anonymize + soft delete + schedule hard delete) |
| Right to Portability | `GET /api/v1/users/me/data-export` | Machine-readable JSON format |
| Right to Object | Preference settings | Opt-out of marketing, analytics tracking |
| Right to Restrict Processing | `POST /api/v1/users/me/restrict-processing` | Freeze non-essential processing |

### Cookie Policy

```
Essential cookies (no consent needed):
  - Session token (HttpOnly, Secure, SameSite=Strict)
  - CSRF token

Analytics cookies (consent required):
  - PostHog analytics

Marketing cookies (consent required):
  - Google Analytics (future)
  - Facebook Pixel (future)
```

---

## 11. HIPAA-Aligned Considerations

Itura is not a covered HIPAA entity but handles sensitive health information. We adopt HIPAA-aligned controls voluntarily and contractually commit to them for corporate clients in healthcare.

| Control | Implementation |
|---|---|
| **PHI identification** | Mood data, journal entries, session notes, assessment responses classified as PHI |
| **Access controls** | Role-based; minimum necessary principle enforced |
| **Audit controls** | All PHI access logged (who, when, what, from where) |
| **Transmission security** | TLS 1.3 minimum; no unencrypted PHI transmission |
| **Encryption at rest** | AES-256 for PHI fields |
| **Business Associate Agreements** | Required from: Azure (BAA available), Agora (BAA available), SendGrid |
| **Workforce training** | Annual security training for all staff with PHI access |
| **Breach notification** | Internal: within 24 hours; affected individuals: within 60 days |
| **Minimum necessary** | Coaches see only their clients' data; corporate HR sees only aggregate anonymized data |
| **Contingency planning** | DR plan tested quarterly |

---

## 12. Threat Model

### STRIDE Analysis

| Threat | Vector | Mitigation |
|---|---|---|
| **Spoofing** | Account takeover via credential stuffing | MFA, rate limiting, lockout, impossible travel detection |
| **Tampering** | Journal content manipulation in transit | TLS 1.3; content hashed and signed at save |
| **Repudiation** | Dispute over payment/booking | Immutable audit logs; payment receipts |
| **Information Disclosure** | PII exposure via API | RBAC; data minimization; encryption |
| **Denial of Service** | API flooding | Azure Front Door DDoS; rate limiting; auto-scaling |
| **Elevation of Privilege** | JWT manipulation | RS256 (asymmetric); server-side validation; no sensitive data in querystring |

### Top Risks

| Risk | Likelihood | Impact | Control |
|---|---|---|---|
| Mass data breach (PII exposure) | Low | Critical | Encryption, pen testing, SIEM |
| AI companion harmful advice | Medium | High | Safety guardrails, crisis protocol, human escalation |
| Payment fraud (double charge) | Low | High | Idempotency keys, webhook signature validation |
| Session token theft (web) | Low | High | HttpOnly cookies, short TTL, revocation |
| Insider threat (admin access) | Low | High | Dual authorization for critical actions, audit log |
| Third-party breach (OpenAI/Agora) | Low | Medium | Contractual SLAs, data minimization, alternative ready |

---

## 13. Security Testing

### Testing Schedule

| Test | Frequency | Responsibility |
|---|---|---|
| SAST (Static Analysis) | Every CI build | SonarQube + CodeQL (automated) |
| DAST (Dynamic Analysis) | Weekly on staging | OWASP ZAP (automated) |
| Dependency scanning | Daily | Snyk (automated) |
| Container image scanning | Every build | Trivy (automated) |
| Penetration testing | Quarterly | External security firm |
| Social engineering test | Annually | External security firm |
| Business logic testing | Each feature launch | Internal security review |
| Red team exercise | Annually | External red team |

### Security Requirements for Every Feature

Before any feature is deployed to production:
- [ ] Threat model documented for new attack surfaces
- [ ] SAST scan passes with zero critical issues
- [ ] Input validation covers all user-controlled data
- [ ] Authorization checks on all new endpoints
- [ ] No secrets in code or config files
- [ ] New dependencies scanned for known CVEs
- [ ] Sensitive operations added to audit log

---

## 14. Incident Response

### Severity Levels

| Level | Definition | Response Time | Examples |
|---|---|---|---|
| P1 (Critical) | Data breach, platform down, active attack | 15 minutes | Mass PII exposure, ransomware, complete outage |
| P2 (High) | Significant service degradation, isolated breach | 1 hour | Payment failure spike, auth service down |
| P3 (Medium) | Partial functionality affected | 4 hours | Notification service failure, coach search down |
| P4 (Low) | Minor issue, workaround available | 24 hours | UI bug, slow endpoint |

### P1 Data Breach Response Procedure

```
Hour 0:
  1. On-call engineer receives PagerDuty alert
  2. Immediately convene war room (Slack: #incident-p1)
  3. CTO notified within 15 minutes
  4. Scope assessment: what data, how many users, how long?

Hour 0–4 (Containment):
  5. Isolate affected systems
  6. Rotate compromised credentials immediately
  7. Block attack vector
  8. Preserve evidence (logs, network captures)

Hour 4–72 (Assessment & Notification):
  9. Root cause analysis begins
  10. Legal counsel engaged
  11. NITDA notified within 72 hours (NDPR requirement)
  12. DPA notified if EU users affected (GDPR requirement)
  13. Affected users notified if required

Week 2+:
  14. Post-incident report (internal)
  15. Remediation implemented and tested
  16. Third-party security review if warranted
  17. Lessons learned documented and actioned
```

### Security Contacts

| Role | Contact Method |
|---|---|
| On-call Engineer | PagerDuty rotation |
| CTO | Direct phone (24/7) |
| Legal Counsel | Retainer firm |
| DPO | Designated officer |
| NITDA | regulatory@nitda.gov.ng |
| Azure Security | Azure Security Center |

---

*End of Security & Compliance Document*  
*Next: [AI_ML.md](./AI_ML.md)*
