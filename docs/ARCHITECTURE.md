# ITURA — System Architecture Document

**Document Version:** 1.0  
**Status:** Engineering-Ready  
**Owner:** Engineering Lead / Principal Architect  
**Last Updated:** May 2026

---

## Table of Contents

1. [Architecture Philosophy](#1-architecture-philosophy)
2. [High-Level Architecture](#2-high-level-architecture)
3. [Microservices Architecture](#3-microservices-architecture)
4. [Event-Driven Architecture](#4-event-driven-architecture)
5. [Real-Time Communication Design](#5-real-time-communication-design)
6. [AI Integration Architecture](#6-ai-integration-architecture)
7. [Notification Architecture](#7-notification-architecture)
8. [Payment Architecture](#8-payment-architecture)
9. [Identity & Access Management](#9-identity--access-management)
10. [API Gateway Design](#10-api-gateway-design)
11. [Distributed Caching](#11-distributed-caching)
12. [Background Jobs](#12-background-jobs)
13. [Logging & Monitoring](#13-logging--monitoring)
14. [Security Architecture](#14-security-architecture)
15. [Multi-Tenant Architecture](#15-multi-tenant-architecture)
16. [Scalability Design](#16-scalability-design)
17. [Disaster Recovery](#17-disaster-recovery)
18. [Cloud Infrastructure](#18-cloud-infrastructure)

---

## 1. Architecture Philosophy

### 1.1 Guiding Principles

| Principle | Implementation |
|---|---|
| **Domain-Driven Design** | Each microservice owns a bounded context (Auth, User, Booking, etc.) |
| **CQRS** | Read and write models separated for high-read domains (mood history, analytics) |
| **Event Sourcing** | Critical state changes (payments, bookings) stored as immutable event streams |
| **Clean Architecture** | Domain → Application → Infrastructure layering in each service |
| **API-First** | All capabilities exposed as versioned REST APIs before UI is built |
| **Security by Design** | Zero-trust networking; least-privilege access at every layer |
| **Observability First** | Every service emits structured logs, metrics, and traces from Day 1 |
| **Fail Gracefully** | Circuit breakers, fallbacks, and degraded modes for every external dependency |

### 1.2 Architecture Decisions Record (Key Decisions)

| Decision | Choice | Rationale |
|---|---|---|
| Service communication | gRPC (sync) + RabbitMQ (async) | gRPC for low-latency internal calls; MQ for decoupled events |
| Primary database | PostgreSQL | ACID compliance, strong relational model for financial and health data |
| AI memory store | MongoDB | Flexible document schema for conversation history and AI context |
| Cache layer | Redis | Sub-millisecond lookups for sessions, rate limits, feature flags |
| Real-time | SignalR (Azure) | First-class .NET integration; scales via Redis backplane |
| Video | Agora / Daily.co | Purpose-built for video therapy; HIPAA-eligible |
| Container orchestration | Kubernetes (AKS) | Industry standard; Azure-native integration |

---

## 2. High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              CLIENT LAYER                                   │
│  ┌─────────────┐   ┌──────────────┐   ┌──────────────┐   ┌──────────────┐  │
│  │  Web App    │   │  Mobile iOS  │   │ Mobile Android│   │ 3rd Party API│  │
│  │ (Next.js)   │   │  (Flutter)   │   │  (Flutter)   │   │  Consumers   │  │
│  └──────┬──────┘   └──────┬───────┘   └──────┬───────┘   └──────┬───────┘  │
└─────────│────────────────│─────────────────│────────────────────│──────────┘
          │                │                 │                    │
          └────────────────┴─────────────────┴────────────────────┘
                                    │
                    ┌───────────────▼────────────────┐
                    │         Azure Front Door        │
                    │   (CDN + WAF + Load Balancer)   │
                    └───────────────┬────────────────┘
                                    │
                    ┌───────────────▼────────────────┐
                    │           API GATEWAY           │
                    │  ┌─────────────────────────┐   │
                    │  │  Rate Limiting           │   │
                    │  │  Auth Token Validation   │   │
                    │  │  Request Routing         │   │
                    │  │  API Versioning          │   │
                    │  │  Circuit Breaking        │   │
                    │  └─────────────────────────┘   │
                    └───────────────┬────────────────┘
                                    │
          ┌─────────────────────────┼──────────────────────────┐
          │                         │                          │
┌─────────▼──────────┐   ┌─────────▼──────────┐   ┌──────────▼─────────┐
│   CORE SERVICES    │   │  SUPPORT SERVICES  │   │  PLATFORM SERVICES │
│                    │   │                    │   │                    │
│  Auth Service      │   │  Notification Svc  │   │  AI Service        │
│  User Service      │   │  Email Service     │   │  Analytics Service │
│  Coach Service     │   │  SMS Service       │   │  Search Service    │
│  Booking Service   │   │  Push Service      │   │  Media Service     │
│  Session Service   │   │  Calendar Service  │   │  CDN Service       │
│  Payment Service   │   │                    │   │                    │
│  Journal Service   │   └────────────────────┘   └────────────────────┘
│  Mood Service      │
│  Community Service │
│  Subscription Svc  │
│  Corporate Service │
│  Admin Service     │
└─────────┬──────────┘
          │
┌─────────▼──────────────────────────────────────────────────────────────────┐
│                          DATA & MESSAGING LAYER                             │
│                                                                             │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌─────────────┐  │
│  │PostgreSQL│  │  Redis   │  │ MongoDB  │  │RabbitMQ  │  │ Azure Blob  │  │
│  │(Primary) │  │ (Cache)  │  │(AI/Docs) │  │(Events)  │  │  Storage    │  │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘  └─────────────┘  │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Microservices Architecture

### 3.1 Service Inventory

| Service | Port | Responsibility | Database | Language |
|---|---|---|---|---|
| `auth-service` | 5001 | Authentication, JWT, OAuth | PostgreSQL | .NET 8 |
| `user-service` | 5002 | User profiles, onboarding, settings | PostgreSQL | .NET 8 |
| `coach-service` | 5003 | Coach profiles, verification, availability | PostgreSQL | .NET 8 |
| `booking-service` | 5004 | Session booking, scheduling, calendar | PostgreSQL | .NET 8 |
| `session-service` | 5005 | Video/voice/text sessions, recordings | PostgreSQL + Blob | .NET 8 |
| `payment-service` | 5006 | Billing, subscriptions, payouts, wallet | PostgreSQL | .NET 8 |
| `ai-service` | 5007 | AI companion, sentiment, recommendations | MongoDB | .NET 8 |
| `journal-service` | 5008 | Journal entries, templates, encryption | PostgreSQL | .NET 8 |
| `mood-service` | 5009 | Mood tracking, analytics, patterns | PostgreSQL (partitioned) | .NET 8 |
| `community-service` | 5010 | Posts, groups, reactions, moderation | PostgreSQL | .NET 8 |
| `notification-service` | 5011 | Push, email, SMS, in-app notifications | PostgreSQL + Redis | .NET 8 |
| `subscription-service` | 5012 | Plans, entitlements, feature gates | PostgreSQL | .NET 8 |
| `corporate-service` | 5013 | Corporate accounts, team management | PostgreSQL | .NET 8 |
| `analytics-service` | 5014 | Event tracking, dashboards, reports | PostgreSQL + TimescaleDB | .NET 8 |
| `admin-service` | 5015 | Admin APIs for platform management | PostgreSQL | .NET 8 |
| `media-service` | 5016 | File uploads, avatar processing, content | Azure Blob | .NET 8 |
| `search-service` | 5017 | Coach search, content search | PostgreSQL + Elasticsearch | .NET 8 |

### 3.2 Service Communication Patterns

#### Synchronous Communication (gRPC)
Used for: Real-time user-facing requests requiring immediate responses

```
Client → API Gateway → booking-service
                            ↓ (gRPC)
                       coach-service (check availability)
                            ↓ (gRPC)
                       payment-service (process payment)
                            ↓ (gRPC)
                       notification-service (send confirmation)
```

#### Asynchronous Communication (RabbitMQ / MassTransit)
Used for: Background processing, cross-service notifications, eventual consistency

```
booking-service publishes: BookingConfirmedEvent
  → notification-service subscribes → sends email/push
  → analytics-service subscribes → tracks event
  → coach-service subscribes → updates coach schedule
  → calendar-service subscribes → generates ICS
```

### 3.3 Clean Architecture Per Service

Each service follows the same layered structure:

```
ServiceName/
├── Domain/
│   ├── Entities/           # Core business objects
│   ├── ValueObjects/       # Immutable domain primitives
│   ├── DomainEvents/       # Events raised by domain actions
│   ├── Repositories/       # Repository interfaces
│   └── Services/           # Domain services
│
├── Application/
│   ├── Commands/           # CQRS write operations (MediatR)
│   ├── Queries/            # CQRS read operations (MediatR)
│   ├── DTOs/               # Data Transfer Objects
│   ├── Validators/         # FluentValidation rules
│   ├── Mappings/           # AutoMapper profiles
│   └── Behaviors/          # Pipeline behaviors (logging, validation)
│
├── Infrastructure/
│   ├── Persistence/        # EF Core DbContext, migrations
│   ├── Repositories/       # Repository implementations
│   ├── ExternalServices/   # Third-party API clients
│   ├── Messaging/          # RabbitMQ publishers/consumers
│   └── Caching/            # Redis cache implementations
│
└── API/
    ├── Controllers/        # ASP.NET Core controllers
    ├── Middleware/         # Custom middleware
    ├── Filters/            # Action/exception filters
    └── Program.cs          # Service startup/DI config
```

### 3.4 CQRS Implementation

```csharp
// Command example: BookSession
public record BookSessionCommand(
    Guid UserId,
    Guid CoachId,
    DateTime StartTime,
    SessionType Type,
    Guid? WalletId
) : IRequest<BookSessionResult>;

public class BookSessionCommandHandler : IRequestHandler<BookSessionCommand, BookSessionResult>
{
    // Validates → Checks availability → Processes payment → Creates booking → Raises events
}

// Query example: GetMoodHistory
public record GetMoodHistoryQuery(
    Guid UserId,
    DateRange Range,
    int PageSize,
    int Page
) : IRequest<PagedResult<MoodEntryDto>>;

public class GetMoodHistoryQueryHandler : IRequestHandler<GetMoodHistoryQuery, PagedResult<MoodEntryDto>>
{
    // Reads from read-optimized view → Returns paginated results
}
```

---

## 4. Event-Driven Architecture

### 4.1 Message Broker Design

**Technology:** RabbitMQ with MassTransit abstraction layer

```
Exchange Topology:
├── itura.domain (topic exchange)
│   ├── user.registered → [notification-service, analytics-service]
│   ├── booking.confirmed → [notification-service, coach-service, analytics-service]
│   ├── booking.canceled → [notification-service, payment-service, analytics-service]
│   ├── session.completed → [payment-service, analytics-service, notification-service]
│   ├── payment.succeeded → [subscription-service, notification-service, analytics-service]
│   ├── payment.failed → [notification-service, user-service]
│   ├── mood.logged → [analytics-service, ai-service]
│   ├── crisis.detected → [notification-service, admin-service]
│   └── community.post.flagged → [moderation-service, admin-service]
│
└── itura.retry (dead letter exchange)
    └── Failed messages → retry with exponential backoff → DLQ after 3 attempts
```

### 4.2 Event Schemas

```json
// BookingConfirmedEvent
{
  "eventId": "evt_01H8X2K9...",
  "eventType": "booking.confirmed",
  "occurredAt": "2026-05-22T10:30:00Z",
  "version": "1.0",
  "payload": {
    "bookingId": "bkg_01H8X2...",
    "userId": "usr_01H7Y3...",
    "coachId": "cch_01H6Z4...",
    "startTime": "2026-05-25T14:00:00Z",
    "sessionType": "video",
    "durationMinutes": 50,
    "amountPaid": 15000,
    "currency": "NGN"
  },
  "metadata": {
    "correlationId": "req_01H9A1...",
    "causationId": "cmd_01H9B2...",
    "tenantId": null,
    "source": "booking-service"
  }
}
```

### 4.3 Outbox Pattern

To guarantee at-least-once delivery of domain events:

```
1. BookSession command executes within a DB transaction:
   a. Insert booking record
   b. Insert outbox event record (same transaction)
   c. Commit transaction

2. Outbox Relay (background job, every 5 seconds):
   a. Query unprocessed outbox events
   b. Publish to RabbitMQ
   c. Mark as processed
   d. Idempotency check on consumers
```

---

## 5. Real-Time Communication Design

### 5.1 SignalR Architecture

```
Client (Web/Mobile)
    │
    ▼ WebSocket / SSE / Long Poll
┌──────────────────────────────────────┐
│           SignalR Hub                │
│  ├── ChatHub (session messaging)     │
│  ├── NotificationHub (alerts)        │
│  ├── PresenceHub (online status)     │
│  └── SessionHub (video coordination) │
└──────────────────┬───────────────────┘
                   │
                   ▼ Scale-out
         ┌─────────────────────┐
         │  Redis Backplane     │
         │  (Pub/Sub across    │
         │   multiple pods)    │
         └─────────────────────┘
```

### 5.2 Hub Definitions

**ChatHub** — Async messaging between user and coach between sessions:
```
Methods:
  - SendMessage(recipientId, content, messageType)
  - MarkAsRead(messageId)
  - StartTyping(recipientId)
  - StopTyping(recipientId)

Groups:
  - conversation_{userId}_{coachId}

Events pushed to client:
  - MessageReceived
  - MessageRead
  - TypingIndicator
  - PresenceChanged
```

**NotificationHub** — Real-time in-app notifications:
```
Groups:
  - user_{userId}

Events pushed to client:
  - NotificationReceived (badge count, message)
  - SessionStarting (T-5 minute alert)
  - CommunityReply
  - BookingConfirmed
```

### 5.3 Video Session Architecture

```
User A (Web/Mobile)
    │ WebRTC
    ▼
┌─────────────────────────────────┐
│    Agora RTC / Daily.co         │
│    (Media Server / SFU)         │
│  - Video/Audio relay            │
│  - Recording (if enabled)       │
│  - Network adaptation           │
└─────────────────────────────────┘
    │ WebRTC
    ▼
Coach B (Web/Mobile)

Signaling layer:
  - Session token generated by booking-service
  - Agora token validation via session-service
  - Join/leave events tracked by session-service
  - Recording stored to Azure Blob (with consent)
```

---

## 6. AI Integration Architecture

### 6.1 AI Service Design

```
User Message
    │
    ▼
ai-service API
    │
    ├── 1. Safety Pre-Filter (Azure Content Safety)
    │      └── Block: self-harm, hate, violence prompts
    │
    ├── 2. Context Assembly
    │      ├── Retrieve conversation history (MongoDB)
    │      ├── Retrieve user mood summary (mood-service gRPC)
    │      ├── Retrieve wellness goals (user-service gRPC)
    │      └── Assemble system prompt + context window
    │
    ├── 3. Azure OpenAI GPT-4o
    │      └── Stream response tokens
    │
    ├── 4. Safety Post-Filter
    │      ├── Crisis keyword detection
    │      ├── Medical advice detection
    │      └── PII leakage detection
    │
    ├── 5. Crisis Protocol (if triggered)
    │      ├── Override response with safety message
    │      ├── Log event (anonymized)
    │      └── Publish CrisisDetectedEvent
    │
    └── 6. Response Delivery
           ├── Stream to client via SSE
           └── Save to conversation history (MongoDB)
```

### 6.2 Context Window Management

```
System Prompt (fixed, ~500 tokens):
  - Sera's persona and behavioral rules
  - Safety guardrails
  - User's name, wellness goals, current level

Dynamic Context (variable, up to 2,000 tokens):
  - Last 10 conversation turns (summarized if longer)
  - Today's mood log (score + note)
  - Current streak status
  - Recent journal themes (summary only, not raw content)

User Message: up to 500 tokens

Total context budget: ~3,000 tokens
Remaining: for response generation (~1,000 tokens)
```

### 6.3 Sentiment Analysis Pipeline

```
Input: User message or journal entry (with permission)
    │
    ▼
Azure AI Language → Sentiment score (Positive/Neutral/Negative, confidence)
    │
    ▼
Custom classifier → Emotional state (anxious, sad, angry, content, hopeful)
    │
    ▼
Stored in: mood-insights table (linked to user, timestamped)
    │
    ▼
Used by: AI response generation + Weekly mood insights + Coach dashboard
```

---

## 7. Notification Architecture

### 7.1 Notification Flow

```
Event Producer (any service)
    │ publishes NotificationRequestedEvent
    ▼
notification-service consumer
    │
    ├── Resolves user preferences (can they receive this type?)
    ├── Checks quiet hours
    ├── Enriches with user data (name, etc.)
    │
    └── Routes to appropriate channel(s):
         ├── Push → Firebase Cloud Messaging (Android) / APNs (iOS)
         ├── Email → SendGrid / Azure Communication Services
         ├── SMS → Termii (Nigeria) / Twilio (global)
         └── In-App → SignalR push to NotificationHub
```

### 7.2 Notification Template System

- Templates stored in database (editable by admin without deployment)
- Handlebars-style variable interpolation: `Hello {{user.fullName}}`
- Variants per channel (push = short, email = rich HTML)
- A/B testing support on notification copy
- Multi-language template variants

### 7.3 Delivery Guarantees

- Outbox pattern for notification events
- At-least-once delivery with idempotency key
- Failed deliveries retried 3x with exponential backoff
- Undelivered after 3 attempts → logged as `notification_failed`
- Bounce/unsubscribe webhooks from SendGrid update user preferences

---

## 8. Payment Architecture

### 8.1 Payment Flow

```
User initiates payment
    │
    ▼
payment-service API
    │
    ├── Validate: user, amount, currency, plan/session
    ├── Create payment intent (idempotency key)
    │
    ▼
Payment Processor Selection:
    ├── Nigerian users → Paystack
    └── International users → Stripe
    │
    ▼
Redirect to secure payment page (processor-hosted)
    │
    ▼ Webhook (HTTPS, signed)
payment-service webhook handler
    │
    ├── Verify webhook signature
    ├── Idempotency check (prevent duplicate processing)
    ├── Update payment record
    ├── Publish PaymentSucceededEvent or PaymentFailedEvent
    │
    ▼
Subscribers:
    ├── subscription-service → activate/renew subscription
    ├── booking-service → confirm booking
    ├── wallet-service → credit wallet
    └── notification-service → send receipt
```

### 8.2 Wallet Architecture

```
User Wallet:
  - Balance (NGN or USD)
  - Transaction history
  - Session credits (non-monetary)
  - Pending balance (refunds in processing)

Top-up flow:
  User → Paystack → PaymentSucceeded → wallet-service credits balance

Session payment:
  booking-service → deduct from wallet → create booking

Refund:
  Cancel request → payment-service → reverse transaction → credit wallet
  (card refund initiated if original payment was card)

Coach payout:
  Trigger: weekly, Friday
  Steps: Calculate earnings → create payout record → bank transfer via Paystack Transfer API
```

### 8.3 Financial Data Integrity

- All monetary values stored as integers (kobo/cents, not decimal)
- Database transactions for all balance changes (no partial updates)
- Audit trail: every wallet transaction logged with reason, amount, balance before/after
- Reconciliation job runs nightly: compare payment processor records vs database
- Alerts triggered if reconciliation discrepancy > ₦0

---

## 9. Identity & Access Management

### 9.1 Authentication Flow

```
User Login Request
    │
    ▼
auth-service
    │
    ├── Validate credentials (bcrypt hash compare)
    ├── Check account status (active/suspended/locked)
    ├── Check MFA requirement
    │
    ▼
Token Generation:
    ├── Access Token (JWT, RS256, 15-minute expiry)
    │   └── Claims: sub, email, role, tier, tenantId, jti
    └── Refresh Token (opaque, 30-day expiry, stored in DB)
    │
    ▼
Response:
    ├── Access token in response body
    └── Refresh token in HttpOnly Secure cookie (web)
        OR secure storage injection (mobile)
```

### 9.2 JWT Token Structure

```json
// Access Token Payload
{
  "sub": "usr_01H7Y3...",
  "email": "user@example.com",
  "role": "User",                    // User | Coach | Admin | Corporate
  "tier": "Pro",                     // Free | Pro | Premium | Executive
  "tenantId": null,                  // set for corporate users
  "permissions": ["mood:write", "journal:write", "booking:create"],
  "jti": "jwt_01H8X...",            // unique token ID for revocation
  "iat": 1716998400,
  "exp": 1716999300,                 // 15 minutes
  "iss": "https://auth.itura.app"
}
```

### 9.3 RBAC Permission Matrix

| Resource | User | Coach | Corporate Admin | Super Admin |
|---|---|---|---|---|
| Own profile | CRUD | CRUD | CRUD | CRUD |
| Other user profiles | Read (limited) | Read (clients only) | Read (own employees) | CRUD |
| Coach profiles | Read | CRUD (own) | Read | CRUD |
| Session booking | Create (own) | Read (own) | Assign to employees | CRUD |
| Journal entries | CRUD (own) | Read (if shared) | None | None |
| Community posts | CRUD (own) | CRUD (own) | Read | CRUD |
| Payments | Read (own) | Read (earnings) | Read (corporate) | CRUD |
| Admin panel | None | None | Limited dashboard | Full |
| User management | None | None | Own employees | All users |

### 9.4 Token Refresh Flow

```
Access token expires (15 min)
    │
    ▼
Client sends refresh token
    │
    ▼
auth-service:
    ├── Validate refresh token (lookup in DB, not expired, not revoked)
    ├── Check user account still active
    ├── Issue new access token
    ├── Rotate refresh token (new one issued, old invalidated)
    └── Return new token pair
```

### 9.5 OAuth 2.0 Social Login

```
User clicks "Continue with Google"
    │
    ▼
Frontend redirects to Google OAuth
    │
    ▼
Google returns authorization code
    │
    ▼
auth-service:
    ├── Exchange code for Google tokens
    ├── Verify ID token signature
    ├── Extract email, name, avatar
    ├── Find or create Itura user account
    ├── Issue Itura JWT tokens
    └── Return tokens to client
```

---

## 10. API Gateway Design

### 10.1 Gateway Responsibilities

| Function | Implementation |
|---|---|
| TLS Termination | Azure Front Door |
| WAF (Web Application Firewall) | Azure Front Door WAF Policy |
| DDoS Protection | Azure DDoS Standard |
| Global Load Balancing | Azure Front Door |
| Rate Limiting | YARP + Redis (sliding window) |
| Auth Token Validation | JWT validation middleware |
| Request Routing | YARP Reverse Proxy |
| API Versioning | URL-based (/api/v1/, /api/v2/) |
| Request Logging | Structured logging → Azure Monitor |
| Circuit Breaking | Polly circuit breaker |
| Request Correlation | X-Correlation-ID header injection |

### 10.2 Rate Limiting Rules

| Tier | Per User Rate Limit | Per IP Rate Limit |
|---|---|---|
| Anonymous | 20 req/min | 100 req/min |
| Free | 60 req/min | 200 req/min |
| Pro | 200 req/min | 500 req/min |
| Premium/Executive | 500 req/min | 1,000 req/min |
| Corporate Admin | 1,000 req/min | 2,000 req/min |
| Admin | Unlimited | Unlimited |

AI Companion: Additional rate limit:
- Free: 5 messages/day
- Pro: 50 messages/day
- Premium/Executive: Unlimited

### 10.3 Routing Configuration (YARP)

```yaml
routes:
  - routeId: auth-route
    clusterId: auth-cluster
    match:
      path: /api/v1/auth/{**catch-all}
    transforms:
      - PathRemovePrefix: /api/v1/auth

  - routeId: booking-route
    clusterId: booking-cluster
    match:
      path: /api/v1/bookings/{**catch-all}
    authorizationPolicy: AuthenticatedUser
    transforms:
      - RequestHeader: X-User-Id, {token.sub}
      - RequestHeader: X-User-Role, {token.role}
      - RequestHeader: X-User-Tier, {token.tier}
```

---

## 11. Distributed Caching

### 11.1 Redis Cache Strategy

| Cache Item | TTL | Invalidation Strategy |
|---|---|---|
| User session (JWT validation cache) | 15 min | Expire on token refresh/logout |
| User profile summary | 5 min | Write-through on profile update |
| Coach profile | 10 min | Write-through on coach update |
| Coach availability slots | 2 min | Write-through on booking |
| Subscription entitlements | 30 min | Write-through on subscription change |
| Feature flags | 5 min | TTL + admin-triggered flush |
| Rate limit counters | Sliding 1 min | TTL auto-expiry |
| Notification preferences | 15 min | Write-through on preference update |
| Community post counts | 5 min | Write-through on reaction |

### 11.2 Cache Patterns

**Cache-Aside (Lazy Loading):**
```
Read request →
  1. Check Redis
  2. Cache hit → return
  3. Cache miss → query DB → store in Redis → return
```

**Write-Through:**
```
Write request →
  1. Update DB
  2. Update Redis simultaneously
  (ensures cache is always fresh after writes)
```

**Cache Key Naming Convention:**
```
{service}:{entity}:{id}
Examples:
  user:profile:usr_01H7Y3
  coach:availability:cch_01H6Z4:2026-05-25
  subscription:entitlements:usr_01H7Y3
  ratelimit:usr_01H7Y3:ai_messages:2026-05-22
```

---

## 12. Background Jobs

### 12.1 Job Inventory (Hangfire)

| Job | Schedule | Description |
|---|---|---|
| MoodNudgeJob | Daily 8:00 AM (per TZ) | Send mood check-in reminders |
| StreakRiskAlertJob | Daily 7:00 PM | Alert users whose streak is at risk |
| WeeklySummaryJob | Every Sunday 6:00 PM | Generate and send weekly wellness report |
| SessionReminderJob | Every 15 min | Send T-24hr and T-1hr session reminders |
| CoachPayoutJob | Every Friday 10:00 AM | Process weekly coach payouts |
| SubscriptionRenewalJob | Daily 2:00 AM | Process due subscriptions |
| FailedPaymentRetryJob | Daily 9:00 AM | Retry failed subscription payments |
| ReportGenerationJob | Daily 2:00 AM | Generate admin/corporate analytics reports |
| OutboxRelayJob | Every 5 seconds | Relay outbox events to RabbitMQ |
| DataRetentionJob | Weekly Sunday 3:00 AM | Delete accounts/data per retention policy |
| ReconciliationJob | Daily 3:00 AM | Reconcile payment processor vs DB |
| AIInsightGenerationJob | Daily 6:00 AM | Generate daily AI mood insights |
| SearchIndexJob | Every 30 min | Update Elasticsearch coach search index |

### 12.2 Job Execution Environment

- Hangfire Server: Dedicated pod in Kubernetes
- Storage: PostgreSQL (job state persistence)
- Dashboard: Hangfire Dashboard (admin-only, internal)
- Retry policy: 3 retries with exponential backoff
- Job timeout: Configurable per job (default: 10 minutes)
- Distributed lock: Prevent duplicate execution across pods

---

## 13. Logging & Monitoring

### 13.1 Observability Stack

```
Application Logs
    │ (Serilog → structured JSON)
    ▼
Azure Monitor / Log Analytics
    │
    ├── Log search and alerting
    └── Connected to:
         ├── Grafana (dashboards)
         └── PagerDuty (on-call alerting)

Distributed Tracing
    │ (OpenTelemetry)
    ▼
Azure Application Insights
    │
    ├── Request traces (end-to-end across microservices)
    ├── Dependency calls (DB, Redis, external APIs)
    └── Exception tracking

Metrics
    │ (Prometheus exporter)
    ▼
Prometheus
    │
    └── Grafana dashboards:
         ├── API response times
         ├── Error rates
         ├── Queue depth
         ├── Cache hit rates
         └── Business metrics (bookings/hr, MAU, revenue)

Uptime Monitoring
    │
    └── Azure Monitor + StatusPage.io
```

### 13.2 Structured Log Format

```json
{
  "timestamp": "2026-05-22T10:30:45.123Z",
  "level": "Information",
  "message": "Session booking confirmed",
  "service": "booking-service",
  "correlationId": "req_01H9A1...",
  "userId": "usr_01H7Y3...",  // hashed in production for PII protection
  "bookingId": "bkg_01H8X2...",
  "durationMs": 145,
  "environment": "production"
}
```

### 13.3 Alert Rules

| Alert | Condition | Severity | Action |
|---|---|---|---|
| API Error Rate | > 1% over 5 min | P1 | PagerDuty page |
| P95 Latency | > 500ms over 5 min | P2 | Slack alert |
| Pod CrashLoop | Any pod restarting > 3x | P1 | PagerDuty page |
| Queue Depth | > 10,000 messages | P2 | Slack alert |
| DB CPU | > 80% for 5 min | P2 | Slack alert |
| Payment failure rate | > 5% over 15 min | P1 | PagerDuty page |
| Crisis detection | Any crisis event | P1 | Immediate notification to clinical team |
| Auth failure spike | > 100 failures/min from single IP | P1 | Auto-block IP + alert |

---

## 14. Security Architecture

See [SECURITY.md](./SECURITY.md) for full detail. Summary:

- Zero-trust networking between services (mTLS via Istio service mesh)
- All secrets in Azure Key Vault (no secrets in environment variables or code)
- WAF policy blocking OWASP Top 10
- All data encrypted at rest (AES-256) and in transit (TLS 1.3)
- JWT RS256 signing with rotating keys
- PII stored encrypted, access logged
- Quarterly penetration testing
- SAST/DAST in CI/CD pipeline

---

## 15. Multi-Tenant Architecture

### 15.1 Tenancy Model

Itura uses a **shared schema, tenant-aware** multi-tenancy model:

- Individual users: `tenantId = null` (global tenant)
- Corporate accounts: `tenantId = {corporateAccountId}`
- All data-access queries include tenant filter automatically
- Row-level security (PostgreSQL RLS) as defense-in-depth

### 15.2 Corporate Tenant Isolation

```
Corporate Account (Tenant):
  ├── Has its own HR admin users
  ├── Employees linked via tenantId
  ├── Corporate billing separate from individual billing
  ├── HR dashboard shows only own employees (enforced at DB level)
  ├── Feature flags configurable per tenant
  └── Custom branding support (white-label future state)
```

### 15.3 Global Tenant Context

```csharp
// ITenantContext injected into every service
public interface ITenantContext
{
    Guid? TenantId { get; }
    bool IsGlobalAdmin { get; }
}

// EF Core global query filter
protected override void OnModelCreating(ModelBuilder builder)
{
    builder.Entity<UserProfile>()
        .HasQueryFilter(x => x.TenantId == _tenantContext.TenantId
                          || _tenantContext.IsGlobalAdmin);
}
```

---

## 16. Scalability Design

### 16.1 Horizontal Scaling

Each microservice is a stateless pod that scales independently:

```yaml
# Kubernetes HPA example
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: booking-service-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: booking-service
  minReplicas: 2
  maxReplicas: 20
  metrics:
    - type: Resource
      resource:
        name: cpu
        target:
          type: Utilization
          averageUtilization: 70
    - type: Resource
      resource:
        name: memory
        target:
          type: Utilization
          averageUtilization: 80
```

### 16.2 Database Scaling

| Database | Scaling Strategy |
|---|---|
| PostgreSQL | Read replicas for query-heavy services; connection pooling via PgBouncer |
| Redis | Redis Cluster for distributed cache; Azure Cache for Redis Premium |
| MongoDB | Replica set + horizontal sharding on userId for AI conversation data |
| Message Queue | RabbitMQ cluster with 3 nodes; consumer groups scale horizontally |

### 16.3 Capacity Planning (Year 1)

| Component | Start | Peak (Year 1) |
|---|---|---|
| API Gateway pods | 2 | 10 |
| Auth service pods | 2 | 6 |
| Booking service pods | 2 | 8 |
| AI service pods | 2 | 12 (compute-intensive) |
| PostgreSQL | 4 vCPU, 16GB | 16 vCPU, 64GB + 2 read replicas |
| Redis | 2GB | 13GB Premium cluster |
| RabbitMQ | 3-node cluster | 3-node cluster (scale consumers) |

---

## 17. Disaster Recovery

### 17.1 Recovery Objectives

| Metric | Target |
|---|---|
| RTO (Recovery Time Objective) | < 1 hour |
| RPO (Recovery Point Objective) | < 15 minutes |
| Backup frequency | Hourly incremental, daily full |
| Backup retention | 30 days |
| Cross-region replication | Active-passive (West Africa → West Europe) |

### 17.2 Backup Strategy

| Data Store | Backup Method | Frequency | Retention |
|---|---|---|---|
| PostgreSQL | pg_dump + WAL shipping | Continuous WAL + daily full | 30 days |
| MongoDB | Mongodump + Azure Backup | Every 4 hours | 30 days |
| Redis | Azure Cache for Redis persistence | Every 15 min (AOF) | 7 days |
| Blob Storage | Azure GRS (geo-redundant storage) | Continuous replication | Indefinite |
| Application config | Git + Azure Key Vault backup | On change | Versioned |

### 17.3 Failover Procedure

```
1. Azure Monitor detects primary region unhealthy (3 consecutive health check failures)
2. PagerDuty alert triggers on-call engineer
3. Azure Front Door health probe marks primary unhealthy → auto-routes to DR region
4. AKS in DR region starts → Helm charts deploy services
5. PostgreSQL read replica in DR region promoted to primary
6. DNS TTL already low (60s) → propagates within 2 minutes
7. Post-incident: RCA within 24 hours
```

---

## 18. Cloud Infrastructure

### 18.1 Azure Services Used

| Service | Purpose |
|---|---|
| Azure Kubernetes Service (AKS) | Container orchestration |
| Azure Front Door | CDN, WAF, global load balancing |
| Azure PostgreSQL Flexible Server | Primary relational database |
| Azure Cache for Redis | Distributed caching |
| Azure Service Bus (backup) | Alternative to RabbitMQ for AZ resilience |
| Azure Blob Storage | Media files, recordings, exports |
| Azure Key Vault | Secrets, certificates, encryption keys |
| Azure OpenAI Service | GPT-4o for AI companion |
| Azure AI Language | Sentiment analysis |
| Azure AI Content Safety | Content moderation |
| Azure Communication Services | Email and SMS delivery |
| Azure Monitor + Log Analytics | Centralized logging and alerting |
| Azure Application Insights | Distributed tracing and APM |
| Azure Container Registry | Docker image registry |
| Azure DevOps | CI/CD pipelines |

### 18.2 Infrastructure as Code

All infrastructure defined in Terraform:

```hcl
# AKS Cluster
resource "azurerm_kubernetes_cluster" "itura" {
  name                = "itura-aks-prod"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  dns_prefix          = "itura"

  default_node_pool {
    name            = "system"
    node_count      = 3
    vm_size         = "Standard_D4s_v3"
    os_disk_size_gb = 100
    type            = "VirtualMachineScaleSets"
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    network_plugin    = "azure"
    load_balancer_sku = "standard"
  }

  oms_agent {
    log_analytics_workspace_id = azurerm_log_analytics_workspace.main.id
  }
}
```

### 18.3 Environment Strategy

| Environment | Purpose | Infrastructure |
|---|---|---|
| Development | Local dev + integration testing | Docker Compose (local) |
| Staging | Pre-production validation | AKS (single-node, reduced) |
| Production | Live platform | AKS (multi-node, HA, multi-AZ) |
| DR | Disaster recovery standby | AKS (minimal, activates on failover) |

---

*End of Architecture Document*  
*Next: [TECH_STACK.md](./TECH_STACK.md)*
