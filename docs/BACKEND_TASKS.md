# ITURA — Backend Engineering Task Breakdown

**Document Version:** 1.0  
**Owner:** Engineering Lead  
**Last Updated:** May 2026  
**Stack:** .NET 8 / ASP.NET Core / PostgreSQL / Redis / RabbitMQ / MassTransit

---

## Task Structure

Each task follows this format:
- **Epic** → **Feature** → **User Story** → **Technical Tasks** → **Subtasks**

Priority: `P0` = Launch blocker | `P1` = MVP | `P2` = Post-MVP | `P3` = Nice-to-have  
Complexity: `XS` (< 1 day) | `S` (1–2 days) | `M` (3–4 days) | `L` (5–7 days) | `XL` (1–2 weeks)

---

## EPIC 1 — Authentication Module

**Epic Goal:** Secure, fast, standards-compliant authentication system supporting email, OAuth, MFA, and role-based access.

---

### FEATURE 1.1 — User Registration

**User Story:** As a new user, I want to register with email and password and receive a verification email.

#### Task BE-AUTH-001: Email Registration Endpoint

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | M |
| **Sprint** | 1 |
| **Dependencies** | Database setup, Email service |

**API Endpoint:**
```
POST /api/v1/auth/register
```

**Request Payload:**
```json
{
  "email": "user@example.com",
  "password": "SecureP@ss123",
  "fullName": "Amara Okafor",
  "timezone": "Africa/Lagos"
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "userId": "usr_01H7Y3...",
    "email": "user@example.com",
    "emailVerificationRequired": true,
    "message": "Check your email to verify your account."
  }
}
```

**Validation Rules:**
- Email: valid format, unique in database, max 255 chars
- Password: min 8 chars, 1 uppercase, 1 lowercase, 1 digit, 1 special char
- FirstName: required, 2–50 chars, no special characters
- Timezone: valid IANA timezone string

**Technical Subtasks:**
1. Create `RegisterCommand` + `RegisterCommandHandler` (MediatR)
2. Implement `RegisterValidator` (FluentValidation)
3. Hash password with BCrypt (cost factor 12)
4. Insert into `accounts` table + `user_profiles` table (cross-service event)
5. Generate 6-digit OTP, store hashed in `email_verifications`
6. Publish `UserRegisteredEvent` → triggers email notification
7. Return response (do NOT return token yet — verify email first)
8. Write unit tests for validator and handler
9. Write integration test for full registration flow

**Security:**
- Rate limit: 10 registrations per IP per hour
- CAPTCHA on web (reCAPTCHA v3) for accounts created via API
- No PII in error messages (don't confirm if email exists on registration)

**Database Impact:**
- INSERT into `accounts`, `user_profiles`, `email_verifications`

---

#### Task BE-AUTH-002: Email Verification

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | S |
| **Sprint** | 1 |

**API Endpoint:**
```
POST /api/v1/auth/verify-email
```

**Request:**
```json
{ "email": "user@example.com", "code": "847291" }
```

**Subtasks:**
1. Validate OTP: lookup by email, check not expired (10 min TTL), check not used
2. Mark email as verified in `accounts`
3. Mark OTP as used
4. Issue JWT access token + refresh token (first login after verification)
5. Return token pair
6. Handle resend OTP endpoint with rate limiting (max 3 resends per hour)

---

#### Task BE-AUTH-003: JWT Token Issuance

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | M |
| **Sprint** | 1 |
| **Dependencies** | RSA key generation in Azure Key Vault |

**Subtasks:**
1. Configure RS256 signing: load private key from Azure Key Vault
2. Build JWT claims builder (sub, email, role, tier, tenantId, jti, iat, exp)
3. Access token: 15-minute expiry
4. Generate opaque refresh token (256-bit random, SHA-256 hash stored in DB)
5. Store refresh token in DB with device info and IP
6. For web: set refresh token in HttpOnly Secure SameSite=Strict cookie
7. For mobile: return refresh token in response body
8. Implement token rotation on each refresh

---

#### Task BE-AUTH-004: Login Endpoint

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | M |
| **Sprint** | 1 |

**API Endpoint:**
```
POST /api/v1/auth/login
```

**Subtasks:**
1. Validate credentials (constant-time BCrypt compare)
2. Check account status (suspended → 403 with reason)
3. Check email verification status
4. Enforce failed login lockout (5 failures → 15-min lock; 10 → 24-hr lock)
5. Check MFA requirement → return `mfa_required` if enabled
6. Issue token pair
7. Log login event to `auth_audit_logs`
8. Return token pair

---

#### Task BE-AUTH-005: OAuth 2.0 (Google)

| Attribute | Value |
|---|---|
| **Priority** | P1 |
| **Complexity** | L |
| **Sprint** | 2 |

**Subtasks:**
1. Configure Google OAuth consent screen and credentials
2. Implement authorization code flow
3. Exchange code for Google ID token
4. Verify ID token signature using Google's JWKS endpoint
5. Extract email, name, picture from claims
6. Find or create Itura account (link by email)
7. Issue Itura token pair
8. Handle account conflict (same email registered with password)

---

#### Task BE-AUTH-006: Password Reset

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | S |
| **Sprint** | 1 |

**Endpoints:**
```
POST /api/v1/auth/forgot-password   → sends reset email
POST /api/v1/auth/reset-password    → applies new password
```

**Subtasks:**
1. Rate limit: 3 reset requests per email per hour
2. Generate secure token (256-bit), hash and store with 1-hour expiry
3. Send email with reset link (embed token)
4. On reset: validate token, hash new password, invalidate ALL refresh tokens
5. Audit log the event

---

#### Task BE-AUTH-007: Multi-Factor Authentication (TOTP)

| Attribute | Value |
|---|---|
| **Priority** | P1 |
| **Complexity** | L |
| **Sprint** | 3 |

**Endpoints:**
```
POST /api/v1/auth/mfa/setup        → returns QR code data
POST /api/v1/auth/mfa/verify-setup → confirms and enables MFA
POST /api/v1/auth/mfa/verify       → validates TOTP during login
POST /api/v1/auth/mfa/disable      → disables MFA (requires password)
```

**Subtasks:**
1. Generate TOTP secret (Base32), encrypt with AES-256 before storing
2. Return QR code data URI for authenticator app
3. Verify first TOTP code to confirm setup
4. Generate and store 8 backup codes (hashed)
5. During login with MFA: return `mfa_challenge` state in login response
6. Accept TOTP on `/mfa/verify` → issue full token

---

#### Task BE-AUTH-008: Token Refresh & Revocation

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | S |
| **Sprint** | 1 |

**Endpoints:**
```
POST /api/v1/auth/refresh   → issue new access token
POST /api/v1/auth/logout    → revoke refresh token
POST /api/v1/auth/logout-all → revoke all sessions
```

---

## EPIC 2 — User Module

---

### FEATURE 2.1 — User Onboarding

#### Task BE-USER-001: Onboarding Flow

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | M |
| **Sprint** | 2 |

**Endpoints:**
```
POST /api/v1/users/onboarding/goals         → save wellness goals
POST /api/v1/users/onboarding/assessment    → save wellness assessment
GET  /api/v1/users/onboarding/status        → check onboarding completion
```

**Subtasks:**
1. Create `SaveWellnessGoalsCommand` → insert into `wellness_goals`
2. Create `SaveWellnessAssessmentCommand` → calculate composite score → insert into `wellness_assessments`
3. Risk level calculation: Low (80+), Moderate (60–79), High (40–59), Crisis (<40)
4. If risk level = Crisis → trigger crisis protocol notification
5. Mark `onboarding_completed = true` on `user_profiles`
6. Publish `OnboardingCompletedEvent` → AI service initializes user context

---

#### Task BE-USER-002: User Profile CRUD

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | M |
| **Sprint** | 2 |

**Endpoints:**
```
GET    /api/v1/users/me           → get own profile
PUT    /api/v1/users/me           → update profile
DELETE /api/v1/users/me           → soft delete account (GDPR)
GET    /api/v1/users/me/avatar    → get avatar upload URL
PUT    /api/v1/users/me/avatar    → process and save avatar
```

**Subtasks:**
1. `GetCurrentUserQuery` → return profile DTO (exclude sensitive fields)
2. `UpdateProfileCommand` → validate and update allowed fields
3. Avatar upload: generate SAS URL for direct Azure Blob upload → confirm + process (resize to 200x200, 400x400)
4. Account deletion: anonymize PII → soft delete → publish `AccountDeletedEvent` → schedule hard delete (90 days)

---

#### Task BE-USER-003: User Preferences

| Attribute | Value |
|---|---|
| **Priority** | P1 |
| **Complexity** | S |
| **Sprint** | 2 |

**Endpoints:**
```
GET /api/v1/users/me/preferences
PUT /api/v1/users/me/preferences
```

**Subtasks:**
1. CRUD for notification preferences, AI tone, quiet hours, theme
2. Publish `PreferencesUpdatedEvent` → notification-service updates delivery rules
3. Cache preferences in Redis (15-min TTL, write-through)

---

### FEATURE 2.2 — Gamification

#### Task BE-USER-004: XP & Level System

| Attribute | Value |
|---|---|
| **Priority** | P1 |
| **Complexity** | M |
| **Sprint** | 4 |

**Subtasks:**
1. `AwardXpCommand(userId, amount, action, referenceId)` → MediatR handler
2. Insert into `xp_transactions`
3. Update `user_profiles.total_xp`
4. Check level-up thresholds → update `wellness_level` if crossed
5. Publish `XpAwardedEvent` + `LevelUpEvent` (if applicable)
6. Notification-service sends "You leveled up!" push
7. `GetLeaderboardQuery(period)` → top 50 by XP in Redis sorted set
8. Leaderboard privacy: only shown with user consent (opt-in setting)

---

#### Task BE-USER-005: Streak Management

| Attribute | Value |
|---|---|
| **Priority** | P1 |
| **Complexity** | M |
| **Sprint** | 3 |

**Subtasks:**
1. `RecordStreakActivityCommand(userId, streakType, date)` → upsert `user_streaks`
2. Streak logic: consecutive days; grace period check (freeze available?)
3. `CheckStreakAtRiskJob` → daily job; users whose last activity was yesterday
4. Publish `StreakRiskEvent` → notification-service sends push at 7pm
5. `BreakStreakCommand` → reset `current_streak`, preserve `longest_streak`
6. Freeze deduction: Pro = 1/week, Premium = 2/week

---

#### Task BE-USER-006: Badge System

| Attribute | Value |
|---|---|
| **Priority** | P1 |
| **Complexity** | M |
| **Sprint** | 4 |

**Subtasks:**
1. Badge definitions table (seed data): 30+ badges with trigger conditions
2. `BadgeEvaluationService` — subscribes to events and evaluates badge criteria
3. Triggered by: `MoodLoggedEvent`, `JournalCreatedEvent`, `SessionCompletedEvent`, etc.
4. `AwardBadgeCommand` → insert into `badges_earned` (idempotent)
5. Publish `BadgeEarnedEvent` → push notification + in-app notification
6. `GET /api/v1/users/me/badges` → return earned badges + progress on next badges

---

## EPIC 3 — Coach Module

---

#### Task BE-COACH-001: Coach Registration & Profile

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | L |
| **Sprint** | 3 |

**Endpoints:**
```
POST   /api/v1/coaches/apply          → submit coach application
GET    /api/v1/coaches/me             → get own coach profile
PUT    /api/v1/coaches/me             → update coach profile
GET    /api/v1/coaches/{id}           → public coach profile
GET    /api/v1/coaches                → search/list coaches (paginated)
```

**Subtasks:**
1. `ApplyAsCoachCommand` → create coach profile with `verification_status = pending`
2. Validate credentials list (at least 1 credential required)
3. Document upload: generate SAS URL for credential documents
4. Publish `CoachApplicationSubmittedEvent` → admin notification + email to coach
5. Search endpoint with filters: specialty, language, price range, availability
6. Elasticsearch index update job (every 30 min) for coach search

---

#### Task BE-COACH-002: Coach Verification Workflow

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | M |
| **Sprint** | 4 |

**Endpoints (Admin only):**
```
GET    /api/v1/admin/coaches/pending         → list pending applications
GET    /api/v1/admin/coaches/{id}/review     → get application details
POST   /api/v1/admin/coaches/{id}/approve    → approve coach
POST   /api/v1/admin/coaches/{id}/reject     → reject with reason
POST   /api/v1/admin/coaches/{id}/suspend    → suspend verified coach
```

**Subtasks:**
1. Admin review queue: filter by status = pending, order by application date
2. `ApproveCoachCommand` → set `verified`, set `verified_at/by`, publish event
3. `RejectCoachCommand` → set `rejected`, store reason, notify coach
4. Coach receives email with outcome and next steps
5. Verified coaches appear in public search immediately

---

#### Task BE-COACH-003: Availability Management

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | L |
| **Sprint** | 4 |

**Endpoints:**
```
GET    /api/v1/coaches/me/availability                → get recurring schedule
POST   /api/v1/coaches/me/availability                → add availability block
DELETE /api/v1/coaches/me/availability/{id}           → remove availability block
GET    /api/v1/coaches/{id}/available-slots?date=...  → get open slots on a date
POST   /api/v1/coaches/me/blocked-times               → block specific dates/times
```

**Subtasks:**
1. Recurring availability: `coach_availability` table (weekly schedule)
2. Available slots calculation: recurring schedule - bookings - blocked times
3. Slots generated in coach's timezone, returned in user's timezone
4. Cache available slots in Redis (2-min TTL, invalidate on booking/block)
5. gRPC endpoint: `CheckSlotAvailability` consumed by booking-service

---

## EPIC 4 — Booking Module

---

#### Task BE-BOOK-001: Session Booking

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | XL |
| **Sprint** | 5 |

**Endpoints:**
```
POST   /api/v1/bookings                    → create booking
GET    /api/v1/bookings/{id}               → get booking details
GET    /api/v1/bookings/me                 → list my bookings (paginated)
POST   /api/v1/bookings/{id}/cancel        → cancel booking
POST   /api/v1/bookings/{id}/reschedule    → reschedule booking
```

**Booking Flow (MassTransit Saga):**
```
1. ValidateRequest → check user, coach, slot availability
2. ReserveSlot → temporarily lock slot (Redis, 10-min TTL)
3. InitiatePayment → call payment-service (gRPC)
4. AwaitPaymentConfirmation (webhook driven)
5. ConfirmBooking → update status, release slot reservation
6. NotifyParties → publish BookingConfirmedEvent
7. Compensate on failure → release slot, refund payment
```

**Subtasks:**
1. Implement `BookingStateMachine` (MassTransit saga)
2. Slot reservation lock in Redis with TTL
3. `CreateBookingCommand` → validate availability, calculate price, initiate saga
4. Payment callback handler (webhook → complete saga)
5. Cancellation: free if > 24hr before; 50% refund if 2-24hr; no refund < 2hr
6. Rescheduling: free if > 24hr before session; limited to 2 times per booking
7. ICS calendar file generation on confirmation
8. Google Calendar API integration (optional, user consent required)

---

#### Task BE-BOOK-002: Session Reminders

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | S |
| **Sprint** | 6 |

**Subtasks:**
1. `SessionReminderJob` runs every 15 minutes
2. Query bookings with `scheduled_at` in next 24hr+1min and `reminder_24h_sent = false`
3. Publish `SessionReminderEvent(24h)` → notification-service
4. Mark `reminder_24h_sent = true`
5. Repeat for 1-hour reminder
6. Send SMS if user has SMS enabled (important: 1hr before is urgent)

---

## EPIC 5 — Payment Module

---

#### Task BE-PAY-001: Paystack Integration

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | L |
| **Sprint** | 5 |

**Endpoints:**
```
POST   /api/v1/payments/initialize          → initialize payment session
POST   /api/v1/payments/verify/{reference}  → verify payment (client-side)
POST   /api/v1/webhooks/paystack            → Paystack webhook handler
```

**Subtasks:**
1. Paystack SDK wrapper service
2. Initialize transaction: create idempotency key, call Paystack API, return payment URL
3. Webhook handler: validate X-Paystack-Signature HMAC, parse event, dispatch to handler
4. Event handlers: charge.success, charge.failed, subscription.create, transfer.success
5. Idempotency: check `payment_transactions` table before processing webhook (prevent duplicates)
6. Publish `PaymentSucceededEvent` or `PaymentFailedEvent`

---

#### Task BE-PAY-002: Stripe Integration (Global)

| Attribute | Value |
|---|---|
| **Priority** | P1 |
| **Complexity** | L |
| **Sprint** | 6 |

**Subtasks:**
1. Stripe SDK wrapper service
2. Create PaymentIntent for one-time payments
3. Create Stripe Customer on user registration
4. Subscription creation with plan pricing
5. Webhook handler: validate Stripe-Signature header
6. Handle: payment_intent.succeeded, invoice.paid, customer.subscription.deleted

---

#### Task BE-PAY-003: Wallet System

| Attribute | Value |
|---|---|
| **Priority** | P1 |
| **Complexity** | L |
| **Sprint** | 6 |

**Endpoints:**
```
GET    /api/v1/wallet                      → get wallet balance + credits
POST   /api/v1/wallet/topup                → initiate wallet top-up
GET    /api/v1/wallet/transactions         → transaction history (paginated)
```

**Subtasks:**
1. Create wallet on user registration (event subscriber)
2. Balance updates using optimistic locking (version column)
3. Atomic debit with `UPDATE wallets SET balance = balance - $amount, version = version + 1 WHERE id = $id AND version = $currentVersion AND balance >= $amount`
4. If 0 rows affected → concurrency conflict → retry up to 3 times
5. Session credit system: separate integer counter, not monetary

---

#### Task BE-PAY-004: Coach Payout System

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | L |
| **Sprint** | 7 |

**Endpoints:**
```
GET    /api/v1/coaches/me/earnings          → earnings summary
GET    /api/v1/coaches/me/payouts           → payout history
POST   /api/v1/coaches/me/bank-account      → save bank account
POST   /api/v1/admin/payouts/process        → trigger payout batch (admin)
```

**Subtasks:**
1. Earnings calculated on `session.completed` event
2. Commission deduction based on coach tier
3. Weekly payout job: aggregate unpaid earnings → create payout record → call Paystack Transfer API
4. Bank account encrypted storage (AES-256, decrypted only at payout time)
5. Payout status webhook from Paystack → update `coach_payouts.status`
6. Failure handling: email coach + admin, retry next week

---

## EPIC 6 — AI Assistant Module

---

#### Task BE-AI-001: Conversational AI Companion (Sera)

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | XL |
| **Sprint** | 3–4 |

**Endpoints:**
```
POST   /api/v1/ai/conversations                     → start or continue conversation
GET    /api/v1/ai/conversations                     → list conversation sessions
GET    /api/v1/ai/conversations/{id}/messages       → get conversation history
DELETE /api/v1/ai/conversations/{id}                → delete conversation
```

**Subtasks:**
1. Conversation management (MongoDB): create/retrieve conversation document
2. Context assembly: fetch last 10 turns + user mood summary + wellness goals
3. System prompt builder: compose dynamic system prompt with user context
4. Azure OpenAI GPT-4o streaming call (SSE response to client)
5. Pre-filter: Azure AI Content Safety check on user message
6. Post-filter: crisis keyword detection (multi-pattern regex + semantic check)
7. Crisis protocol: if triggered → override response → publish `CrisisDetectedEvent`
8. Rate limiting per tier (5/50/unlimited daily messages)
9. Token counting and context window management
10. Conversation summary job: compress old messages to reduce context size
11. Store AI response in MongoDB after streaming completes

---

#### Task BE-AI-002: AI Rate Limiting

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | S |
| **Sprint** | 4 |

**Subtasks:**
1. Redis sorted set: `ratelimit:ai:{userId}:{date}` → count daily messages
2. Check limit before processing → 429 response if exceeded with remaining count and reset time
3. Different limits per subscription tier (from subscription-service gRPC call)
4. Admin override capability

---

#### Task BE-AI-003: Journaling Prompts Generation

| Attribute | Value |
|---|---|
| **Priority** | P1 |
| **Complexity** | M |
| **Sprint** | 5 |

**Endpoints:**
```
GET /api/v1/ai/journal-prompts    → returns 3 personalized prompts
```

**Subtasks:**
1. Fetch user's current mood and recent journal themes
2. Call GPT-4o with prompt generation instructions
3. Return 3 diverse, personalized prompts
4. Cache prompts per user per day (Redis, 24hr TTL)

---

## EPIC 7 — Journaling Module

---

#### Task BE-JRN-001: Journal CRUD

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | M |
| **Sprint** | 3 |

**Endpoints:**
```
POST   /api/v1/journal/entries                → create entry
GET    /api/v1/journal/entries                → list entries (paginated)
GET    /api/v1/journal/entries/{id}           → get entry
PUT    /api/v1/journal/entries/{id}           → update entry
DELETE /api/v1/journal/entries/{id}           → soft delete entry
GET    /api/v1/journal/templates              → list templates
```

**Subtasks:**
1. AES-256-GCM encryption of `content` before saving
2. Decryption on read (key fetched from Azure Key Vault)
3. Word count calculation (stored, used for XP award threshold)
4. Emotion tag validation (from allowed enum)
5. Free tier: enforce 3-entry-per-week limit (query count in current week)
6. Publish `JournalEntryCreatedEvent` → XP award, streak update

---

#### Task BE-JRN-002: Coach Sharing

| Attribute | Value |
|---|---|
| **Priority** | P1 |
| **Complexity** | S |
| **Sprint** | 5 |

**Endpoints:**
```
POST   /api/v1/journal/entries/{id}/share/{coachId}   → share with coach
DELETE /api/v1/journal/entries/{id}/share/{coachId}   → revoke share
GET    /api/v1/coaches/me/shared-journals              → coach: get shared journals
```

**Subtasks:**
1. Insert into `journal_coach_shares`
2. Notify coach via `JournalSharedEvent`
3. Coach can read entry (decrypted) only when share is active and not revoked
4. Revoke: set `revoked_at`, coach loses access immediately

---

## EPIC 8 — Community Module

---

#### Task BE-COM-001: Posts & Replies

| Attribute | Value |
|---|---|
| **Priority** | P1 |
| **Complexity** | L |
| **Sprint** | 6–7 |

**Endpoints:**
```
POST   /api/v1/community/posts              → create post
GET    /api/v1/community/posts              → feed (topic filter, pagination)
GET    /api/v1/community/posts/{id}         → get post + replies
PUT    /api/v1/community/posts/{id}         → edit post
DELETE /api/v1/community/posts/{id}         → soft delete
POST   /api/v1/community/posts/{id}/replies → reply to post
POST   /api/v1/community/posts/{id}/react   → add reaction
POST   /api/v1/community/posts/{id}/report  → report post
```

**Subtasks:**
1. AI pre-moderation on post creation (Azure AI Content Safety + custom classifier)
2. Confidence score < 0.9 on safe → hold for human review
3. Auto-reject: self-harm, hate speech, spam content
4. Anonymous post handling: generate and store anonymous name
5. Reaction upsert (only 1 reaction type per user per post; update if different)
6. Denormalized counters: atomic `UPDATE community_posts SET reaction_count = reaction_count + 1`
7. Infinite scroll pagination (cursor-based, not offset)
8. Hot feed algorithm: (reactions * 2 + replies) / (hours_since_posted + 2)^1.5

---

## EPIC 9 — Notifications Module

---

#### Task BE-NOT-001: Notification Service Infrastructure

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | L |
| **Sprint** | 6 |

**Subtasks:**
1. MassTransit consumer for `NotificationRequestedEvent`
2. Preference check: does user want this notification type?
3. Quiet hours check: skip push if in quiet window
4. Channel routing: push (FCM/APNs) + email (SendGrid) + SMS (Termii) + in-app
5. Template rendering: fetch template, interpolate variables
6. Delivery tracking: insert into `notification_deliveries`
7. Retry policy: 3 attempts, exponential backoff (1min, 5min, 15min)
8. Bounce/failure handling: disable channel for user on hard bounce

---

#### Task BE-NOT-002: Push Notification Integration

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | M |
| **Sprint** | 6 |

**Endpoints:**
```
POST /api/v1/users/me/device-tokens    → register FCM/APNs token
DELETE /api/v1/users/me/device-tokens/{token} → unregister token
```

**Subtasks:**
1. Store device tokens (with platform: ios/android/web)
2. Firebase Admin SDK for FCM (Android + Web)
3. Apple APNs HTTP/2 integration for iOS
4. Handle token refresh: if FCM returns `InvalidRegistration` → delete token
5. Multi-device support: send to all active tokens for a user

---

## EPIC 10 — Admin Module

---

#### Task BE-ADMIN-001: Admin Dashboard APIs

| Attribute | Value |
|---|---|
| **Priority** | P0 |
| **Complexity** | L |
| **Sprint** | 7–8 |

**Endpoints:**
```
GET  /api/v1/admin/dashboard/overview     → key metrics
GET  /api/v1/admin/users                  → paginated user list with search
POST /api/v1/admin/users/{id}/suspend     → suspend user
POST /api/v1/admin/users/{id}/restore     → restore user
GET  /api/v1/admin/coaches/pending        → verification queue
GET  /api/v1/admin/moderation/queue       → content moderation queue
POST /api/v1/admin/moderation/{id}/action → approve/remove content
GET  /api/v1/admin/payments/overview      → payment reconciliation
GET  /api/v1/admin/analytics/revenue      → revenue breakdown
```

**Subtasks:**
1. Admin role guard on all endpoints (policy: `RequireRole("Admin")`)
2. Overview: cross-service data aggregation (cached, refreshed every 5 min)
3. User search: name, email, status, tier, registration date filter
4. Suspension: update `accounts.status`, invalidate all sessions (delete refresh tokens)
5. Moderation queue: pending posts from AI flag + user reports
6. Revenue analytics: subscription MRR, session revenue, refunds by period

---

## EPIC 11 — Analytics Module

---

#### Task BE-ANA-001: Event Tracking Pipeline

| Attribute | Value |
|---|---|
| **Priority** | P1 |
| **Complexity** | L |
| **Sprint** | 7 |

**Subtasks:**
1. MassTransit consumer: subscribe to all domain events → transform to analytics events
2. Insert into `analytics_events` (TimescaleDB)
3. No PII in analytics events (use UUIDs, not emails/names)
4. `GET /api/v1/admin/analytics/events` → query with filters
5. Pre-aggregated views: daily MAU, DAU, session counts, revenue
6. Scheduled job: refresh materialized views nightly
7. PostHog SDK integration for product analytics (event forwarding)

---

## API Standards (All Endpoints)

### Response Format

```json
// Success
{
  "success": true,
  "data": { ... },
  "meta": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 150,
    "totalPages": 8
  }
}

// Error
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "The request is invalid.",
    "details": [
      { "field": "email", "message": "Email is required." }
    ],
    "traceId": "req_01H9A1..."
  }
}
```

### Error Codes

| HTTP | Code | Meaning |
|---|---|---|
| 400 | `VALIDATION_ERROR` | Request body/query fails validation |
| 401 | `UNAUTHORIZED` | No valid token |
| 403 | `FORBIDDEN` | Token valid but insufficient permissions |
| 404 | `NOT_FOUND` | Resource not found |
| 409 | `CONFLICT` | Resource already exists |
| 422 | `BUSINESS_RULE_VIOLATION` | Request valid but violates business rule |
| 429 | `RATE_LIMITED` | Too many requests |
| 500 | `INTERNAL_ERROR` | Unexpected server error |

### Security Requirements (All Endpoints)

- All endpoints: HTTPS only (TLS 1.3)
- All authenticated endpoints: validate JWT, check `jti` not revoked
- All write endpoints: CSRF protection for web clients
- All admin endpoints: additional IP allowlist validation
- All payment endpoints: idempotency key required
- All PII endpoints: access logged to audit trail

---

*End of Backend Task Breakdown*  
*Next: [FRONTEND_TASKS.md](./FRONTEND_TASKS.md)*
