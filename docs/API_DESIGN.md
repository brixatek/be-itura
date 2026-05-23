# ITURA — API Design Standards

**Document Version:** 1.0  
**Owner:** Backend Engineering  
**Last Updated:** May 2026  
**Standard:** REST · OpenAPI 3.1 · JSON

---

## Table of Contents

1. [API Design Principles](#1-api-design-principles)
2. [URL Naming Conventions](#2-url-naming-conventions)
3. [Authentication](#3-authentication)
4. [Versioning Strategy](#4-versioning-strategy)
5. [Request Standards](#5-request-standards)
6. [Response Formats](#6-response-formats)
7. [Error Handling](#7-error-handling)
8. [Pagination](#8-pagination)
9. [Filtering & Sorting](#9-filtering--sorting)
10. [Rate Limiting](#10-rate-limiting)
11. [Full Endpoint Reference](#11-full-endpoint-reference)
12. [Example Payloads](#12-example-payloads)
13. [OpenAPI Specification](#13-openapi-specification)

---

## 1. API Design Principles

| Principle | Rule |
|---|---|
| **Resource-oriented** | URLs represent resources (nouns), not actions (verbs) |
| **Stateless** | Every request contains all information needed to process it |
| **Consistent** | Same patterns everywhere: casing, errors, pagination |
| **Predictable** | Reading the URL tells you what will happen |
| **Versioned** | Breaking changes always in a new version |
| **Documented** | Every endpoint has OpenAPI spec before implementation |
| **Secure by default** | All endpoints authenticated unless explicitly public |

---

## 2. URL Naming Conventions

### Base URL
```
Production:  https://api.itura.app/api/v1
Staging:     https://api.staging.itura.app/api/v1
Development: http://localhost:5000/api/v1
```

### URL Structure
```
/api/{version}/{resource}/{id}/{sub-resource}

Examples:
  /api/v1/users/me
  /api/v1/users/me/preferences
  /api/v1/coaches/{coachId}/availability
  /api/v1/bookings/{bookingId}
  /api/v1/journal/entries/{entryId}
  /api/v1/community/posts/{postId}/replies
```

### Conventions

| Rule | Correct | Incorrect |
|---|---|---|
| Lowercase with hyphens | `/coach-profiles` | `/CoachProfiles` `/coach_profiles` |
| Plural nouns | `/users`, `/bookings` | `/user`, `/booking` |
| No trailing slash | `/api/v1/coaches` | `/api/v1/coaches/` |
| Nested for ownership | `/users/me/preferences` | `/preferences?userId=...` |
| `me` for current user | `/users/me` | `/users/current` or `/me` |
| Filter via query params | `/coaches?specialty=anxiety` | `/coaches/anxiety` |
| Action as sub-resource | `/bookings/{id}/cancel` | `/cancel-booking/{id}` |

### HTTP Methods

| Method | Use | Body | Idempotent |
|---|---|---|---|
| GET | Read resource | None | Yes |
| POST | Create resource / trigger action | JSON | No |
| PUT | Full replace | JSON | Yes |
| PATCH | Partial update | JSON | No |
| DELETE | Delete resource | None | Yes |

---

## 3. Authentication

### Bearer Token
```
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Token Refresh
```
Cookie: refresh_token=... (web clients, HttpOnly)
POST /api/v1/auth/refresh  (mobile clients, send refresh token in body)
```

### Public Endpoints (no auth required)
```
POST   /api/v1/auth/register
POST   /api/v1/auth/login
POST   /api/v1/auth/verify-email
POST   /api/v1/auth/forgot-password
POST   /api/v1/auth/reset-password
POST   /api/v1/auth/refresh
GET    /api/v1/coaches                     (public discovery, limited fields)
GET    /api/v1/coaches/{id}                (public profile)
GET    /api/v1/subscription-plans          (pricing page)
POST   /api/v1/webhooks/paystack           (Paystack webhook, signature verified)
POST   /api/v1/webhooks/stripe             (Stripe webhook, signature verified)
GET    /api/v1/health                      (health check)
```

---

## 4. Versioning Strategy

### URL Versioning (chosen approach)

```
/api/v1/...    Current stable
/api/v2/...    Next major version (when breaking changes required)
```

### Version Lifecycle

| Version | Status | End-of-Life |
|---|---|---|
| v1 | Active | Minimum 12 months after v2 launch |
| v2 | Future | TBD |

### Breaking vs Non-Breaking Changes

**Non-breaking (no version bump needed):**
- Adding new optional fields to responses
- Adding new optional query parameters
- Adding new endpoints
- New enum values (clients must handle unknown values)

**Breaking (requires version bump):**
- Removing or renaming fields
- Changing field types
- Changing HTTP methods
- Removing endpoints
- Changing authentication mechanism

---

## 5. Request Standards

### Headers (all requests)

```
Content-Type: application/json
Accept: application/json
Authorization: Bearer {token}
X-Request-ID: {uuid}              # client-generated, for tracing
X-Client-Version: 1.0.0           # app version
X-Platform: web|ios|android       # client platform
```

### Request Body Format
```json
{
  "camelCaseFields": "always",
  "noSnakeCase": true
}
```

### Idempotency Keys (payments, subscriptions)
```
Idempotency-Key: {client-generated-uuid}
```
Same key → same operation returns cached result (prevents duplicate charges).

---

## 6. Response Formats

### Success Response (Single Resource)

```json
HTTP/1.1 200 OK
Content-Type: application/json

{
  "success": true,
  "data": {
    "id": "usr_01H7Y3KRJM2NPNQF0YMJHRTM6",
    "email": "amara@example.com",
    "fullName": "Amara Okafor",
    "wellnessLevel": 3,
    "totalXp": 650,
    "createdAt": "2026-05-01T08:00:00Z"
  }
}
```

### Success Response (List Resource)

```json
HTTP/1.1 200 OK

{
  "success": true,
  "data": [
    { "id": "...", "name": "..." },
    { "id": "...", "name": "..." }
  ],
  "meta": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 150,
    "totalPages": 8,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

### Success Response (Cursor Pagination)

```json
{
  "success": true,
  "data": [ ... ],
  "meta": {
    "cursor": "eyJpZCI6IjAxSFgifQ==",
    "hasNextPage": true,
    "pageSize": 20
  }
}
```

### Created Response

```json
HTTP/1.1 201 Created
Location: /api/v1/bookings/bkg_01H8X2K9

{
  "success": true,
  "data": {
    "id": "bkg_01H8X2K9",
    "status": "confirmed",
    ...
  }
}
```

### No Content Response

```
HTTP/1.1 204 No Content
(empty body)
```

### Field Naming
- All JSON fields: `camelCase`
- Dates: ISO 8601 UTC: `"2026-05-22T14:30:00Z"`
- Money: integers in smallest unit: `"amountKobo": 1500000` (= ₦15,000)
- UUIDs: string format: `"id": "usr_01H7Y3..."`
- ID prefix convention: `usr_`, `cch_`, `bkg_`, `pay_`, `jrn_`, `mde_` etc.

---

## 7. Error Handling

### Error Response Format

```json
HTTP/1.1 {status_code}
Content-Type: application/json

{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "The request contains invalid data.",
    "details": [
      {
        "field": "email",
        "code": "INVALID_FORMAT",
        "message": "Email must be a valid email address."
      },
      {
        "field": "password",
        "code": "TOO_SHORT",
        "message": "Password must be at least 8 characters."
      }
    ],
    "traceId": "req_01H9A1B2C3D4E5F6"
  }
}
```

### Error Codes

| HTTP | Code | Description |
|---|---|---|
| 400 | `VALIDATION_ERROR` | Request body/params fail validation |
| 400 | `INVALID_REQUEST` | Malformed JSON or missing required field |
| 401 | `UNAUTHORIZED` | No token or expired token |
| 401 | `TOKEN_EXPIRED` | Token valid but expired |
| 401 | `MFA_REQUIRED` | MFA code required to complete login |
| 403 | `FORBIDDEN` | Token valid, insufficient permissions |
| 403 | `ACCOUNT_SUSPENDED` | Account suspended by admin |
| 403 | `EMAIL_NOT_VERIFIED` | Email verification required |
| 403 | `SUBSCRIPTION_REQUIRED` | Feature requires paid subscription |
| 404 | `NOT_FOUND` | Resource not found |
| 409 | `ALREADY_EXISTS` | Resource with same unique key exists |
| 409 | `SLOT_UNAVAILABLE` | Booking slot no longer available |
| 422 | `INSUFFICIENT_BALANCE` | Wallet balance too low |
| 422 | `RATE_LIMIT_DAILY_AI` | Daily AI message limit exceeded |
| 422 | `COACH_NOT_VERIFIED` | Coach not yet verified |
| 429 | `RATE_LIMITED` | Too many requests |
| 500 | `INTERNAL_ERROR` | Unexpected server error |
| 503 | `SERVICE_UNAVAILABLE` | Maintenance or dependency failure |

### Domain-specific field error codes

```
REQUIRED            → Field is required but missing
INVALID_FORMAT      → Field value doesn't match expected format
TOO_SHORT           → Value shorter than minimum
TOO_LONG            → Value longer than maximum
OUT_OF_RANGE        → Numeric value out of allowed range
NOT_UNIQUE          → Value must be unique but already exists
INVALID_ENUM        → Value not in allowed enum set
```

---

## 8. Pagination

### Offset Pagination (admin/reporting queries)
```
GET /api/v1/admin/users?page=2&pageSize=20

Response meta:
{
  "page": 2,
  "pageSize": 20,
  "totalCount": 450,
  "totalPages": 23
}
```

### Cursor Pagination (feeds, timelines)
```
GET /api/v1/community/posts?cursor=eyJpZCI6Ijk4&pageSize=20

First request: no cursor
Response: { "meta": { "cursor": "eyJpZCI6IjAxSFgifQ==", "hasNextPage": true }}

Next request: GET /api/v1/community/posts?cursor=eyJpZCI6IjAxSFgifQ==
```

**Why cursor pagination for feeds:**
- Consistent results even as new items are added
- No "page drift" (items appearing on multiple pages or skipped)
- Better performance (no COUNT(*) query needed)

---

## 9. Filtering & Sorting

### Filter Syntax
```
GET /api/v1/coaches?specialty=anxiety&language=en&minPrice=5000&maxPrice=30000

GET /api/v1/mood/entries?from=2026-05-01&to=2026-05-31&moodScore=1,2

GET /api/v1/bookings?status=confirmed&upcoming=true
```

### Sort Syntax
```
GET /api/v1/coaches?sort=rating&order=desc
GET /api/v1/journal/entries?sort=writtenAt&order=desc

Defaults:
  - Most resources: sort=createdAt, order=desc
  - Coach search: sort=relevance (weighted: rating + availability)
  - Community feed: sort=createdAt (new) or sort=score (trending)
```

---

## 10. Rate Limiting

### Rate Limit Headers

```
X-RateLimit-Limit: 200          # requests allowed in window
X-RateLimit-Remaining: 150      # remaining requests
X-RateLimit-Reset: 1716998460   # Unix timestamp when window resets
Retry-After: 60                 # seconds to wait (on 429 response)
```

### Rate Limit Strategy (Sliding Window)

```
General API:
  Free tier:    60 req/min per user
  Pro:          200 req/min per user
  Premium:      500 req/min per user

AI Companion:
  Free:         5 messages/day
  Pro:          50 messages/day
  Premium:      Unlimited

Auth endpoints:
  Login:        10 attempts/15min per IP
  Register:     10 registrations/hour per IP
  OTP resend:   3 requests/hour per email
  Password reset: 3 requests/hour per email

Payment:
  Initialize:   10 requests/min per user
```

---

## 11. Full Endpoint Reference

### Authentication Service

```
POST   /api/v1/auth/register                    Register new user
POST   /api/v1/auth/verify-email                Verify email OTP
POST   /api/v1/auth/resend-verification         Resend verification OTP
POST   /api/v1/auth/login                       Login with email/password
POST   /api/v1/auth/oauth/google                Google OAuth login
POST   /api/v1/auth/oauth/apple                 Apple OAuth login
POST   /api/v1/auth/refresh                     Refresh access token
POST   /api/v1/auth/logout                      Revoke current session
POST   /api/v1/auth/logout-all                  Revoke all sessions
POST   /api/v1/auth/forgot-password             Request password reset
POST   /api/v1/auth/reset-password              Apply new password
POST   /api/v1/auth/mfa/setup                   Initiate MFA setup
POST   /api/v1/auth/mfa/confirm-setup           Confirm and enable MFA
POST   /api/v1/auth/mfa/verify                  Verify MFA during login
DELETE /api/v1/auth/mfa                         Disable MFA
```

### User Service

```
GET    /api/v1/users/me                         Get current user profile
PUT    /api/v1/users/me                         Update profile
DELETE /api/v1/users/me                         Delete account (GDPR)
GET    /api/v1/users/me/avatar/upload-url       Get pre-signed upload URL
POST   /api/v1/users/me/avatar                  Confirm avatar upload
GET    /api/v1/users/me/preferences             Get notification/UI preferences
PUT    /api/v1/users/me/preferences             Update preferences
GET    /api/v1/users/me/streaks                 Get all streaks
GET    /api/v1/users/me/badges                  Get earned badges + progress
GET    /api/v1/users/me/xp-history              Get XP transaction log
GET    /api/v1/users/me/level                   Get wellness level + XP
GET    /api/v1/users/me/data-export             Request GDPR data export
POST   /api/v1/users/onboarding/goals           Save wellness goals
POST   /api/v1/users/onboarding/assessment      Save wellness assessment
```

### Coach Service

```
POST   /api/v1/coaches/apply                    Apply as coach
GET    /api/v1/coaches                          List/search coaches (public)
GET    /api/v1/coaches/{id}                     Get coach public profile
GET    /api/v1/coaches/me                       Get own coach profile
PUT    /api/v1/coaches/me                       Update own profile
GET    /api/v1/coaches/me/clients               List active clients
GET    /api/v1/coaches/me/earnings              Get earnings summary
GET    /api/v1/coaches/me/earnings/breakdown    Detailed session earnings
GET    /api/v1/coaches/me/payouts               Payout history
POST   /api/v1/coaches/me/bank-account          Save bank account
PUT    /api/v1/coaches/me/bank-account          Update bank account
GET    /api/v1/coaches/me/availability          Get weekly schedule
POST   /api/v1/coaches/me/availability          Add availability block
DELETE /api/v1/coaches/me/availability/{id}     Remove availability block
GET    /api/v1/coaches/{id}/available-slots     Get open slots for date
POST   /api/v1/coaches/me/blocked-times         Block specific time
DELETE /api/v1/coaches/me/blocked-times/{id}    Unblock time
GET    /api/v1/coaches/me/reviews               Get own reviews
```

### Booking Service

```
POST   /api/v1/bookings                         Create booking
GET    /api/v1/bookings                         List my bookings
GET    /api/v1/bookings/{id}                    Get booking detail
POST   /api/v1/bookings/{id}/cancel             Cancel booking
POST   /api/v1/bookings/{id}/reschedule         Reschedule booking
POST   /api/v1/bookings/{id}/rate               Rate and review session
GET    /api/v1/bookings/{id}/session-token      Get video session token
GET    /api/v1/group-sessions                   List available group sessions
POST   /api/v1/group-sessions/{id}/join         Join a group session
```

### Payment Service

```
POST   /api/v1/payments/initialize              Initialize payment (returns URL)
GET    /api/v1/payments/verify/{reference}      Verify payment status
GET    /api/v1/payments/history                 List transaction history
GET    /api/v1/wallet                           Get wallet balance + credits
POST   /api/v1/wallet/topup                     Initiate wallet top-up
GET    /api/v1/wallet/transactions              Wallet transaction history
POST   /api/v1/webhooks/paystack                Paystack webhook
POST   /api/v1/webhooks/stripe                  Stripe webhook
```

### AI Service

```
POST   /api/v1/ai/conversations                 Send message (streaming)
GET    /api/v1/ai/conversations                 List conversation sessions
GET    /api/v1/ai/conversations/{id}/messages   Get conversation messages
DELETE /api/v1/ai/conversations/{id}            Delete conversation
GET    /api/v1/ai/journal-prompts               Get personalized journal prompts
GET    /api/v1/ai/wellness-recommendations      Get personalized recommendations
```

### Journal Service

```
POST   /api/v1/journal/entries                  Create journal entry
GET    /api/v1/journal/entries                  List entries (paginated)
GET    /api/v1/journal/entries/{id}             Get entry
PUT    /api/v1/journal/entries/{id}             Update entry
DELETE /api/v1/journal/entries/{id}             Delete entry (soft)
POST   /api/v1/journal/entries/{id}/share       Share with coach
DELETE /api/v1/journal/entries/{id}/share/{coachId}   Revoke share
GET    /api/v1/journal/templates                List templates
GET    /api/v1/journal/entries/search           Search entries by keyword
```

### Mood Service

```
POST   /api/v1/mood/entries                     Log mood entry
GET    /api/v1/mood/entries                     List mood history (paginated)
GET    /api/v1/mood/today                       Check if mood logged today
GET    /api/v1/mood/summary                     Get summary stats
GET    /api/v1/mood/insights                    Get AI-generated insights
```

### Community Service

```
GET    /api/v1/community/topics                 List all topics
GET    /api/v1/community/posts                  Get posts feed
POST   /api/v1/community/posts                  Create post
GET    /api/v1/community/posts/{id}             Get post with replies
PUT    /api/v1/community/posts/{id}             Edit own post
DELETE /api/v1/community/posts/{id}             Delete own post
POST   /api/v1/community/posts/{id}/replies     Reply to post
POST   /api/v1/community/posts/{id}/react       React to post
DELETE /api/v1/community/posts/{id}/react       Remove reaction
POST   /api/v1/community/posts/{id}/report      Report post
GET    /api/v1/community/groups                 List support groups
POST   /api/v1/community/groups/{id}/join       Join group
```

### Subscription Service

```
GET    /api/v1/subscription-plans               List available plans (public)
GET    /api/v1/subscriptions/me                 Get current subscription
POST   /api/v1/subscriptions                    Create subscription
POST   /api/v1/subscriptions/me/upgrade         Upgrade plan
POST   /api/v1/subscriptions/me/cancel          Cancel subscription
POST   /api/v1/subscriptions/me/resume          Resume paused subscription
GET    /api/v1/subscriptions/me/invoices        List invoices
GET    /api/v1/subscriptions/me/entitlements    Check feature access
```

### Notification Service

```
GET    /api/v1/notifications                    List in-app notifications
GET    /api/v1/notifications/unread-count       Get unread count
POST   /api/v1/notifications/mark-read         Mark specific as read
POST   /api/v1/notifications/mark-all-read     Mark all as read
POST   /api/v1/device-tokens                    Register push device token
DELETE /api/v1/device-tokens/{token}            Remove push device token
```

### Admin Service

```
GET    /api/v1/admin/dashboard                  Dashboard overview metrics
GET    /api/v1/admin/users                      List all users
GET    /api/v1/admin/users/{id}                 Get user detail
POST   /api/v1/admin/users/{id}/suspend         Suspend user
POST   /api/v1/admin/users/{id}/restore         Restore user
GET    /api/v1/admin/coaches/pending            Coach verification queue
POST   /api/v1/admin/coaches/{id}/approve       Approve coach
POST   /api/v1/admin/coaches/{id}/reject        Reject coach
POST   /api/v1/admin/coaches/{id}/suspend       Suspend coach
GET    /api/v1/admin/moderation/queue           Content moderation queue
POST   /api/v1/admin/moderation/{id}/approve    Approve flagged content
POST   /api/v1/admin/moderation/{id}/remove     Remove content
GET    /api/v1/admin/payments                   Payment overview
POST   /api/v1/admin/payouts/process            Trigger payout batch
GET    /api/v1/admin/analytics/users            User analytics
GET    /api/v1/admin/analytics/revenue          Revenue analytics
GET    /api/v1/admin/analytics/engagement       Engagement analytics
GET    /api/v1/admin/audit-logs                 Platform audit logs
```

---

## 12. Example Payloads

### Register User

**Request:**
```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "email": "amara@example.com",
  "password": "SecureP@ss123",
  "fullName": "Amara Okafor",
  "timezone": "Africa/Lagos"
}
```

**Response 201:**
```json
{
  "success": true,
  "data": {
    "userId": "usr_01H7Y3KRJM",
    "email": "amara@example.com",
    "emailVerificationRequired": true,
    "message": "Check your email for a verification code."
  }
}
```

---

### Login

**Request:**
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "amara@example.com",
  "password": "SecureP@ss123",
  "deviceName": "iPhone 15 Pro"
}
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJSUzI1NiJ9...",
    "tokenType": "Bearer",
    "expiresIn": 900,
    "user": {
      "id": "usr_01H7Y3KRJM",
      "email": "amara@example.com",
      "fullName": "Amara Okafor",
      "role": "User",
      "tier": "Free",
      "onboardingCompleted": true,
      "wellnessLevel": 3
    }
  }
}
```

---

### Log Mood Entry

**Request:**
```http
POST /api/v1/mood/entries
Authorization: Bearer eyJhbGciOiJSUzI1NiJ9...
Content-Type: application/json

{
  "moodScore": 3,
  "note": "Feeling okay today, had a good morning but afternoon got stressful.",
  "triggers": ["work", "sleep"]
}
```

**Response 201:**
```json
{
  "success": true,
  "data": {
    "id": "mde_01H8X2K9JM",
    "moodScore": 3,
    "moodLabel": "Neutral",
    "note": "Feeling okay today...",
    "triggers": ["work", "sleep"],
    "loggedAt": "2026-05-22T11:30:00Z",
    "streakDay": 7,
    "xpEarned": 10
  }
}
```

---

### Create Booking

**Request:**
```http
POST /api/v1/bookings
Authorization: Bearer ...
Content-Type: application/json
Idempotency-Key: bkg-req-01H9ABCDEF

{
  "coachId": "cch_01H6Z4MNOP",
  "scheduledAt": "2026-05-25T14:00:00Z",
  "durationMinutes": 50,
  "sessionType": "video",
  "paymentMethod": "wallet",
  "couponCode": null,
  "useSessionCredit": false,
  "coachNote": "I'd like to focus on work-related anxiety."
}
```

**Response 201:**
```json
{
  "success": true,
  "data": {
    "id": "bkg_01H8X2K9JM",
    "status": "confirmed",
    "coach": {
      "id": "cch_01H6Z4MNOP",
      "name": "Dr. Chinelo Obi",
      "avatarUrl": "https://cdn.itura.app/avatars/cch_01H6Z4MNOP.jpg"
    },
    "scheduledAt": "2026-05-25T14:00:00Z",
    "durationMinutes": 50,
    "sessionType": "video",
    "amountKobo": 1500000,
    "currency": "NGN",
    "calendarToken": "cal_01H8X2...",
    "icsDownloadUrl": "https://api.itura.app/api/v1/bookings/bkg_01H8X2K9JM/calendar.ics"
  }
}
```

---

### Send AI Companion Message (Streaming)

**Request:**
```http
POST /api/v1/ai/conversations
Authorization: Bearer ...
Content-Type: application/json
Accept: text/event-stream

{
  "conversationId": "conv_01H9AB",
  "message": "I've been feeling really anxious about my presentation tomorrow. I can't stop thinking about it."
}
```

**Response (SSE Stream):**
```
Content-Type: text/event-stream

data: {"type":"token","content":"I"}
data: {"type":"token","content":" hear"}
data: {"type":"token","content":" you"}
data: {"type":"token","content":","}
data: {"type":"token","content":" Amara"}
...
data: {"type":"complete","messageId":"msg_01H9AC","conversationId":"conv_01H9AB"}
```

---

### Get Coaches with Filters

**Request:**
```http
GET /api/v1/coaches?specialty=anxiety&language=en&minPrice=5000&maxPrice=30000&sessionType=video&sort=rating&order=desc&page=1&pageSize=12
```

**Response 200:**
```json
{
  "success": true,
  "data": [
    {
      "id": "cch_01H6Z4MNOP",
      "fullName": "Dr. Chinelo Obi",
      "professionalTitle": "Licensed Clinical Psychologist",
      "avatarUrl": "https://cdn.itura.app/avatars/...",
      "specialties": ["anxiety", "depression", "burnout"],
      "languages": ["en", "ig"],
      "sessionPriceNgn": 15000,
      "sessionDurationMin": 50,
      "rating": 4.9,
      "reviewCount": 127,
      "isAvailableToday": true,
      "gender": "female",
      "yearsExperience": 8,
      "nextAvailableSlot": "2026-05-22T15:00:00Z"
    }
  ],
  "meta": {
    "page": 1,
    "pageSize": 12,
    "totalCount": 47,
    "totalPages": 4,
    "hasNextPage": true
  }
}
```

---

## 13. OpenAPI Specification

All endpoints are documented in OpenAPI 3.1 format. The spec is:
- Auto-generated from code using **Swashbuckle** (ASP.NET Core)
- Available at: `https://api.itura.app/swagger` (dev/staging only)
- Published to: `docs/openapi.yaml` (version controlled)

### OpenAPI Spec Snippet

```yaml
openapi: 3.1.0
info:
  title: Itura API
  version: 1.0.0
  description: Mental Wellness & Emotional Wellbeing Platform API
  contact:
    name: Itura Engineering
    email: engineering@itura.app
  license:
    name: Proprietary

servers:
  - url: https://api.itura.app/api/v1
    description: Production
  - url: https://api.staging.itura.app/api/v1
    description: Staging

security:
  - BearerAuth: []

components:
  securitySchemes:
    BearerAuth:
      type: http
      scheme: bearer
      bearerFormat: JWT

  schemas:
    ApiResponse:
      type: object
      required: [success]
      properties:
        success:
          type: boolean
        data:
          type: object
        meta:
          $ref: '#/components/schemas/PaginationMeta'

    ApiError:
      type: object
      properties:
        success:
          type: boolean
          example: false
        error:
          type: object
          properties:
            code:
              type: string
              example: VALIDATION_ERROR
            message:
              type: string
            details:
              type: array
              items:
                type: object
                properties:
                  field:
                    type: string
                  code:
                    type: string
                  message:
                    type: string
            traceId:
              type: string

    MoodEntry:
      type: object
      properties:
        id:
          type: string
          example: mde_01H8X2K9JM
        moodScore:
          type: integer
          minimum: 1
          maximum: 5
        moodLabel:
          type: string
          enum: [very_sad, sad, neutral, happy, very_happy]
        note:
          type: string
          maxLength: 280
        triggers:
          type: array
          items:
            type: string
        loggedAt:
          type: string
          format: date-time
        xpEarned:
          type: integer

paths:
  /mood/entries:
    post:
      summary: Log a mood entry
      tags: [Mood]
      security:
        - BearerAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              required: [moodScore]
              properties:
                moodScore:
                  type: integer
                  minimum: 1
                  maximum: 5
                note:
                  type: string
                  maxLength: 280
                triggers:
                  type: array
                  items:
                    type: string
                    enum: [work, sleep, family, body, finances, relationships, other]
      responses:
        '201':
          description: Mood logged successfully
          content:
            application/json:
              schema:
                allOf:
                  - $ref: '#/components/schemas/ApiResponse'
                  - properties:
                      data:
                        $ref: '#/components/schemas/MoodEntry'
        '400':
          $ref: '#/components/responses/ValidationError'
        '401':
          $ref: '#/components/responses/Unauthorized'
        '409':
          description: Mood already logged today
```

---

*End of API Design Document*  
*Next: [DEVOPS.md](./DEVOPS.md)*
