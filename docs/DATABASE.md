# ITURA — Database Design

**Document Version:** 1.0  
**Owner:** Backend Engineering / DBA  
**Last Updated:** May 2026

---

## Table of Contents

1. [Database Strategy](#1-database-strategy)
2. [ERD Overview](#2-erd-overview)
3. [Core Table Schemas](#3-core-table-schemas)
4. [Indexing Strategy](#4-indexing-strategy)
5. [Partitioning Strategy](#5-partitioning-strategy)
6. [Audit Tables](#6-audit-tables)
7. [Soft Delete Strategy](#7-soft-delete-strategy)
8. [Multi-Tenant Strategy](#8-multi-tenant-strategy)
9. [Encryption Strategy](#9-encryption-strategy)
10. [Migration Strategy](#10-migration-strategy)

---

## 1. Database Strategy

### 1.1 Database-per-Service

Each microservice owns its own database. No direct cross-database joins. Data needed across services is fetched via API or published via events.

| Service | Database | Engine |
|---|---|---|
| auth-service | itura_auth | PostgreSQL 16 |
| user-service | itura_users | PostgreSQL 16 |
| coach-service | itura_coaches | PostgreSQL 16 |
| booking-service | itura_bookings | PostgreSQL 16 |
| payment-service | itura_payments | PostgreSQL 16 |
| journal-service | itura_journal | PostgreSQL 16 |
| mood-service | itura_mood | PostgreSQL 16 + TimescaleDB |
| community-service | itura_community | PostgreSQL 16 |
| notification-service | itura_notifications | PostgreSQL 16 |
| subscription-service | itura_subscriptions | PostgreSQL 16 |
| corporate-service | itura_corporate | PostgreSQL 16 |
| ai-service | itura_ai | MongoDB |
| analytics-service | itura_analytics | PostgreSQL 16 (TimescaleDB) |

### 1.2 Naming Conventions

| Convention | Rule |
|---|---|
| Table names | `snake_case`, plural (e.g., `users`, `coach_profiles`) |
| Column names | `snake_case` (e.g., `created_at`, `user_id`) |
| Primary keys | `id UUID DEFAULT gen_random_uuid()` |
| Foreign keys | `{referenced_table_singular}_id` (e.g., `user_id`, `coach_id`) |
| Timestamps | `created_at TIMESTAMPTZ`, `updated_at TIMESTAMPTZ` |
| Soft delete | `deleted_at TIMESTAMPTZ NULL` |
| Audit columns | `created_by UUID`, `updated_by UUID` |
| Boolean flags | `is_` prefix (e.g., `is_active`, `is_verified`) |
| Status enums | `status` column with PostgreSQL ENUM type |

### 1.3 Standard Base Columns (every table)

```sql
id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
updated_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
deleted_at  TIMESTAMPTZ NULL,          -- soft delete
created_by  UUID NULL,                 -- user/system that created
updated_by  UUID NULL                  -- user/system that last updated
```

---

## 2. ERD Overview

### Domain Relationships

```
USERS
  │
  ├──< MOOD_ENTRIES (1:N)
  ├──< JOURNAL_ENTRIES (1:N)
  ├──< BOOKINGS (1:N, as client)
  ├──< SUBSCRIPTIONS (1:N)
  ├──< WALLET (1:1)
  ├──< NOTIFICATIONS (1:N)
  ├──< COMMUNITY_POSTS (1:N)
  ├──< AI_CONVERSATIONS (1:N, via MongoDB)
  └──< CORPORATE_MEMBERSHIPS (1:N)

COACHES
  │
  ├──< COACH_AVAILABILITY (1:N)
  ├──< BOOKINGS (1:N, as provider)
  ├──< COACH_SPECIALTIES (1:N)
  ├──< REVIEWS (1:N)
  └──< COACH_EARNINGS (1:N)

BOOKINGS
  │
  ├── USER (N:1)
  ├── COACH (N:1)
  ├──< SESSIONS (1:1 or 1:N for group)
  └── PAYMENT_TRANSACTION (1:1)

SUBSCRIPTIONS
  │
  ├── USER (N:1)
  ├── SUBSCRIPTION_PLAN (N:1)
  └──< SUBSCRIPTION_INVOICES (1:N)

CORPORATE_ACCOUNTS
  │
  ├──< CORPORATE_MEMBERSHIPS (1:N → USERS)
  ├──< CORPORATE_INVOICES (1:N)
  └──< CORPORATE_SESSION_CREDITS (1:N)
```

---

## 3. Core Table Schemas

### 3.1 itura_auth — Authentication Service

```sql
-- accounts: core auth identity
CREATE TABLE accounts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email           VARCHAR(255) NOT NULL UNIQUE,
    email_verified  BOOLEAN NOT NULL DEFAULT FALSE,
    phone_number    VARCHAR(20) UNIQUE,
    phone_verified  BOOLEAN NOT NULL DEFAULT FALSE,
    password_hash   VARCHAR(60),                          -- BCrypt hash
    provider        VARCHAR(20) NOT NULL DEFAULT 'local', -- local | google | apple
    provider_id     VARCHAR(255),                         -- OAuth provider user ID
    role            VARCHAR(20) NOT NULL DEFAULT 'User',  -- User | Coach | Admin | Corporate
    status          VARCHAR(20) NOT NULL DEFAULT 'active', -- active | suspended | pending | deleted
    last_login_at   TIMESTAMPTZ,
    failed_login_count INT NOT NULL DEFAULT 0,
    locked_until    TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at      TIMESTAMPTZ
);

-- refresh_tokens
CREATE TABLE refresh_tokens (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id      UUID NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
    token_hash      VARCHAR(64) NOT NULL UNIQUE,          -- SHA-256 hash of token
    device_name     VARCHAR(100),
    ip_address      INET,
    expires_at      TIMESTAMPTZ NOT NULL,
    revoked_at      TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- mfa_configs
CREATE TABLE mfa_configs (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id      UUID NOT NULL UNIQUE REFERENCES accounts(id) ON DELETE CASCADE,
    method          VARCHAR(20) NOT NULL,                  -- totp | sms
    secret_encrypted TEXT,                                 -- AES-256 encrypted TOTP secret
    is_enabled      BOOLEAN NOT NULL DEFAULT FALSE,
    backup_codes    TEXT[],                                -- encrypted backup codes
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- email_verifications
CREATE TABLE email_verifications (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id      UUID NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
    code            VARCHAR(6) NOT NULL,
    expires_at      TIMESTAMPTZ NOT NULL,
    used_at         TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- password_resets
CREATE TABLE password_resets (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id      UUID NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
    token_hash      VARCHAR(64) NOT NULL,
    expires_at      TIMESTAMPTZ NOT NULL,
    used_at         TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- audit_log
CREATE TABLE auth_audit_logs (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id      UUID,
    event_type      VARCHAR(50) NOT NULL,  -- login | logout | password_reset | mfa_enabled
    ip_address      INET,
    user_agent      TEXT,
    result          VARCHAR(20) NOT NULL,  -- success | failure
    metadata        JSONB,
    occurred_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

---

### 3.2 itura_users — User Service

```sql
-- user_profiles
CREATE TABLE user_profiles (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id          UUID NOT NULL UNIQUE,             -- references auth.accounts.id
    tenant_id           UUID,                             -- NULL for individual users
    full_name           VARCHAR(100) NOT NULL,
    display_name        VARCHAR(100),
    avatar_url          VARCHAR(500),
    date_of_birth       DATE,
    age_bracket         VARCHAR(20),                       -- 13-17 | 18-25 | 26-35 | 36-50 | 50+
    gender              VARCHAR(20),
    country             VARCHAR(3),                        -- ISO 3166-1 alpha-3
    city                VARCHAR(100),
    timezone            VARCHAR(50) NOT NULL DEFAULT 'Africa/Lagos',
    language            VARCHAR(10) NOT NULL DEFAULT 'en',
    bio                 TEXT,
    wellness_level      INT NOT NULL DEFAULT 1,            -- 1-10 gamification level
    total_xp            INT NOT NULL DEFAULT 0,
    is_anonymous        BOOLEAN NOT NULL DEFAULT FALSE,    -- allow anonymous posting
    onboarding_completed BOOLEAN NOT NULL DEFAULT FALSE,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at          TIMESTAMPTZ
);

-- wellness_goals (from onboarding, multi-select)
CREATE TABLE wellness_goals (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     UUID NOT NULL REFERENCES user_profiles(id) ON DELETE CASCADE,
    goal        VARCHAR(50) NOT NULL,  -- manage_anxiety | prevent_burnout | grief | relationships | etc.
    priority    INT NOT NULL DEFAULT 1,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- wellness_assessments (PHQ-9/GAD-7 adapted)
CREATE TABLE wellness_assessments (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL REFERENCES user_profiles(id) ON DELETE CASCADE,
    assessment_type VARCHAR(20) NOT NULL DEFAULT 'onboarding', -- onboarding | monthly | manual
    responses       JSONB NOT NULL,         -- {question_id: answer_score}
    wellness_score  INT NOT NULL,           -- 0-100 composite score
    risk_level      VARCHAR(20) NOT NULL,   -- low | moderate | high | crisis
    taken_at        TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- user_preferences
CREATE TABLE user_preferences (
    id                          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id                     UUID NOT NULL UNIQUE REFERENCES user_profiles(id) ON DELETE CASCADE,
    mood_reminder_enabled       BOOLEAN NOT NULL DEFAULT TRUE,
    mood_reminder_time          TIME NOT NULL DEFAULT '08:00',
    journal_reminder_enabled    BOOLEAN NOT NULL DEFAULT TRUE,
    journal_reminder_time       TIME NOT NULL DEFAULT '20:00',
    push_enabled                BOOLEAN NOT NULL DEFAULT TRUE,
    email_enabled               BOOLEAN NOT NULL DEFAULT TRUE,
    sms_enabled                 BOOLEAN NOT NULL DEFAULT FALSE,
    marketing_emails_enabled    BOOLEAN NOT NULL DEFAULT TRUE,
    quiet_hours_start           TIME NOT NULL DEFAULT '22:00',
    quiet_hours_end             TIME NOT NULL DEFAULT '08:00',
    ai_tone                     VARCHAR(20) NOT NULL DEFAULT 'friendly', -- friendly | professional | spiritual | direct
    ai_companion_name           VARCHAR(30) NOT NULL DEFAULT 'Sera',
    theme                       VARCHAR(10) NOT NULL DEFAULT 'light',    -- light | dark | auto
    updated_at                  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- user_streaks
CREATE TABLE user_streaks (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id             UUID NOT NULL REFERENCES user_profiles(id) ON DELETE CASCADE,
    streak_type         VARCHAR(20) NOT NULL, -- mood | journal | wellness
    current_streak      INT NOT NULL DEFAULT 0,
    longest_streak      INT NOT NULL DEFAULT 0,
    last_activity_date  DATE,
    freeze_count        INT NOT NULL DEFAULT 0,
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(user_id, streak_type)
);

-- badges_earned
CREATE TABLE badges_earned (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     UUID NOT NULL REFERENCES user_profiles(id) ON DELETE CASCADE,
    badge_id    VARCHAR(50) NOT NULL,
    earned_at   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(user_id, badge_id)
);

-- xp_transactions
CREATE TABLE xp_transactions (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     UUID NOT NULL REFERENCES user_profiles(id) ON DELETE CASCADE,
    amount      INT NOT NULL,
    action      VARCHAR(50) NOT NULL,  -- mood_logged | journal_created | session_completed | etc.
    reference_id UUID,                 -- optional: ID of the triggering entity
    occurred_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

---

### 3.3 itura_coaches — Coach Service

```sql
-- coach_profiles
CREATE TABLE coach_profiles (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id          UUID NOT NULL UNIQUE,
    full_name           VARCHAR(100) NOT NULL,
    professional_title  VARCHAR(100),                    -- Licensed Psychologist | Certified Coach | etc.
    avatar_url          VARCHAR(500),
    bio                 TEXT,
    years_experience    INT,
    session_price_ngn   INT,                             -- in kobo
    session_price_usd   INT,                             -- in cents
    session_duration_min INT NOT NULL DEFAULT 50,
    accepts_video       BOOLEAN NOT NULL DEFAULT TRUE,
    accepts_voice       BOOLEAN NOT NULL DEFAULT TRUE,
    accepts_async       BOOLEAN NOT NULL DEFAULT TRUE,
    country             VARCHAR(3) NOT NULL DEFAULT 'NGA',
    languages           VARCHAR(10)[] NOT NULL DEFAULT ARRAY['en'],
    gender              VARCHAR(20),
    verification_status VARCHAR(20) NOT NULL DEFAULT 'pending', -- pending | under_review | verified | rejected | suspended
    verification_notes  TEXT,
    verified_at         TIMESTAMPTZ,
    verified_by         UUID,                            -- admin user ID
    rating              NUMERIC(3,2),                   -- computed, cached
    review_count        INT NOT NULL DEFAULT 0,         -- computed, cached
    total_sessions      INT NOT NULL DEFAULT 0,         -- computed
    is_featured         BOOLEAN NOT NULL DEFAULT FALSE,
    is_accepting_new_clients BOOLEAN NOT NULL DEFAULT TRUE,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at          TIMESTAMPTZ
);

-- coach_specialties
CREATE TABLE coach_specialties (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    coach_id    UUID NOT NULL REFERENCES coach_profiles(id) ON DELETE CASCADE,
    specialty   VARCHAR(50) NOT NULL  -- anxiety | depression | grief | couples | career | burnout | etc.
);
CREATE UNIQUE INDEX idx_coach_specialty ON coach_specialties(coach_id, specialty);

-- coach_credentials
CREATE TABLE coach_credentials (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    coach_id        UUID NOT NULL REFERENCES coach_profiles(id) ON DELETE CASCADE,
    credential_type VARCHAR(50) NOT NULL,    -- license | certificate | degree
    title           VARCHAR(200) NOT NULL,
    issuing_body    VARCHAR(200),
    issue_date      DATE,
    expiry_date     DATE,
    document_url    VARCHAR(500),            -- encrypted reference to blob storage
    is_verified     BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- coach_availability
CREATE TABLE coach_availability (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    coach_id    UUID NOT NULL REFERENCES coach_profiles(id) ON DELETE CASCADE,
    day_of_week SMALLINT NOT NULL,           -- 0=Sunday, 6=Saturday
    start_time  TIME NOT NULL,
    end_time    TIME NOT NULL,
    timezone    VARCHAR(50) NOT NULL DEFAULT 'Africa/Lagos',
    is_active   BOOLEAN NOT NULL DEFAULT TRUE,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- coach_blocked_times (exceptions: holidays, personal blocks)
CREATE TABLE coach_blocked_times (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    coach_id    UUID NOT NULL REFERENCES coach_profiles(id) ON DELETE CASCADE,
    start_at    TIMESTAMPTZ NOT NULL,
    end_at      TIMESTAMPTZ NOT NULL,
    reason      VARCHAR(100),
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- coach_reviews
CREATE TABLE coach_reviews (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    coach_id        UUID NOT NULL REFERENCES coach_profiles(id) ON DELETE CASCADE,
    user_id         UUID NOT NULL,
    booking_id      UUID NOT NULL UNIQUE,               -- one review per booking
    rating          SMALLINT NOT NULL CHECK (rating BETWEEN 1 AND 5),
    review_text     TEXT,
    is_anonymous    BOOLEAN NOT NULL DEFAULT FALSE,
    is_visible      BOOLEAN NOT NULL DEFAULT TRUE,
    flagged_at      TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- coach_session_notes (private to coach)
CREATE TABLE coach_session_notes (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    coach_id        UUID NOT NULL REFERENCES coach_profiles(id) ON DELETE CASCADE,
    booking_id      UUID NOT NULL,
    user_id         UUID NOT NULL,
    note_content    TEXT NOT NULL,               -- encrypted at application layer
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

---

### 3.4 itura_bookings — Booking Service

```sql
-- bookings
CREATE TABLE bookings (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id             UUID NOT NULL,
    coach_id            UUID NOT NULL,
    session_type        VARCHAR(20) NOT NULL,   -- video | voice | async_text | group
    status              VARCHAR(20) NOT NULL DEFAULT 'pending',
                        -- pending | confirmed | in_progress | completed | canceled | no_show
    scheduled_at        TIMESTAMPTZ NOT NULL,
    duration_minutes    INT NOT NULL DEFAULT 50,
    timezone            VARCHAR(50) NOT NULL,
    price_amount        INT NOT NULL,           -- in kobo/cents
    currency            VARCHAR(3) NOT NULL DEFAULT 'NGN',
    payment_status      VARCHAR(20) NOT NULL DEFAULT 'pending',
                        -- pending | paid | refunded | waived
    payment_reference   VARCHAR(100),           -- Paystack/Stripe reference
    session_credit_used BOOLEAN NOT NULL DEFAULT FALSE,
    coach_note          TEXT,                   -- pre-session note from user to coach
    cancellation_reason TEXT,
    canceled_at         TIMESTAMPTZ,
    canceled_by         VARCHAR(10),            -- user | coach | admin | system
    completed_at        TIMESTAMPTZ,
    reminder_24h_sent   BOOLEAN NOT NULL DEFAULT FALSE,
    reminder_1h_sent    BOOLEAN NOT NULL DEFAULT FALSE,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- group_bookings (for group sessions)
CREATE TABLE group_sessions (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    coach_id            UUID NOT NULL,
    title               VARCHAR(200) NOT NULL,
    description         TEXT,
    session_type        VARCHAR(20) NOT NULL DEFAULT 'group',
    max_participants    INT NOT NULL DEFAULT 20,
    current_count       INT NOT NULL DEFAULT 0,
    price_per_person    INT NOT NULL,
    currency            VARCHAR(3) NOT NULL DEFAULT 'NGN',
    scheduled_at        TIMESTAMPTZ NOT NULL,
    duration_minutes    INT NOT NULL DEFAULT 60,
    status              VARCHAR(20) NOT NULL DEFAULT 'open',
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE group_session_participants (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    group_session_id UUID NOT NULL REFERENCES group_sessions(id),
    user_id         UUID NOT NULL,
    booking_id      UUID NOT NULL,
    joined_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(group_session_id, user_id)
);
```

---

### 3.5 itura_payments — Payment Service

```sql
-- payment_transactions
CREATE TABLE payment_transactions (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id             UUID NOT NULL,
    type                VARCHAR(30) NOT NULL,
                        -- subscription | session | wallet_topup | refund | corporate
    amount              BIGINT NOT NULL,        -- in smallest currency unit (kobo/cents)
    currency            VARCHAR(3) NOT NULL DEFAULT 'NGN',
    status              VARCHAR(20) NOT NULL DEFAULT 'pending',
                        -- pending | succeeded | failed | refunded | disputed
    processor           VARCHAR(20) NOT NULL,   -- paystack | stripe
    processor_reference VARCHAR(200),           -- external payment ID
    processor_response  JSONB,                  -- full webhook payload (encrypted)
    idempotency_key     VARCHAR(100) NOT NULL UNIQUE,
    description         TEXT,
    metadata            JSONB,
    booking_id          UUID,
    subscription_id     UUID,
    refund_of           UUID REFERENCES payment_transactions(id),
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- wallets
CREATE TABLE wallets (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL UNIQUE,
    balance         BIGINT NOT NULL DEFAULT 0 CHECK (balance >= 0),
    currency        VARCHAR(3) NOT NULL DEFAULT 'NGN',
    session_credits INT NOT NULL DEFAULT 0 CHECK (session_credits >= 0),
    version         INT NOT NULL DEFAULT 0,     -- optimistic concurrency
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- wallet_transactions
CREATE TABLE wallet_transactions (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    wallet_id       UUID NOT NULL REFERENCES wallets(id),
    type            VARCHAR(20) NOT NULL,  -- credit | debit | session_credit | session_debit
    amount          BIGINT NOT NULL,
    balance_before  BIGINT NOT NULL,
    balance_after   BIGINT NOT NULL,
    reason          VARCHAR(100) NOT NULL,
    reference_id    UUID,                  -- payment_transaction_id or booking_id
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- coach_earnings
CREATE TABLE coach_earnings (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    coach_id        UUID NOT NULL,
    booking_id      UUID NOT NULL UNIQUE,
    gross_amount    BIGINT NOT NULL,        -- total session price
    commission_rate NUMERIC(5,2) NOT NULL, -- e.g., 20.00 for 20%
    commission_amount BIGINT NOT NULL,
    net_amount      BIGINT NOT NULL,        -- gross - commission
    currency        VARCHAR(3) NOT NULL DEFAULT 'NGN',
    payout_id       UUID REFERENCES coach_payouts(id),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- coach_payouts
CREATE TABLE coach_payouts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    coach_id        UUID NOT NULL,
    amount          BIGINT NOT NULL,
    currency        VARCHAR(3) NOT NULL DEFAULT 'NGN',
    status          VARCHAR(20) NOT NULL DEFAULT 'pending',
                    -- pending | processing | completed | failed
    bank_code       VARCHAR(10),
    account_number_encrypted TEXT,
    account_name    VARCHAR(100),
    processor_reference VARCHAR(200),
    processed_at    TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

---

### 3.6 itura_subscriptions — Subscription Service

```sql
-- subscription_plans
CREATE TABLE subscription_plans (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name                VARCHAR(50) NOT NULL UNIQUE,    -- free | pro | premium | executive
    display_name        VARCHAR(100) NOT NULL,
    price_ngn           INT NOT NULL DEFAULT 0,
    price_usd           INT NOT NULL DEFAULT 0,
    price_ngn_annual    INT,
    price_usd_annual    INT,
    billing_cycle       VARCHAR(20) NOT NULL DEFAULT 'monthly', -- monthly | annual
    features            JSONB NOT NULL,                -- feature flags for this plan
    session_credits     INT NOT NULL DEFAULT 0,        -- monthly session credits
    ai_message_limit    INT NOT NULL DEFAULT 5,        -- -1 for unlimited
    is_active           BOOLEAN NOT NULL DEFAULT TRUE,
    sort_order          INT NOT NULL DEFAULT 0,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- subscriptions
CREATE TABLE subscriptions (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id             UUID NOT NULL,
    plan_id             UUID NOT NULL REFERENCES subscription_plans(id),
    status              VARCHAR(20) NOT NULL DEFAULT 'active',
                        -- active | past_due | canceled | trialing | paused
    current_period_start TIMESTAMPTZ NOT NULL,
    current_period_end  TIMESTAMPTZ NOT NULL,
    cancel_at           TIMESTAMPTZ,                  -- scheduled cancellation
    canceled_at         TIMESTAMPTZ,
    trial_end           TIMESTAMPTZ,
    processor           VARCHAR(20) NOT NULL,          -- paystack | stripe
    processor_subscription_id VARCHAR(200),
    session_credits_remaining INT NOT NULL DEFAULT 0,
    session_credits_reset_at TIMESTAMPTZ,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- subscription_invoices
CREATE TABLE subscription_invoices (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    subscription_id     UUID NOT NULL REFERENCES subscriptions(id),
    user_id             UUID NOT NULL,
    amount              INT NOT NULL,
    currency            VARCHAR(3) NOT NULL DEFAULT 'NGN',
    status              VARCHAR(20) NOT NULL DEFAULT 'pending',
                        -- pending | paid | failed | voided
    payment_transaction_id UUID REFERENCES payment_transactions(id),
    billing_period_start TIMESTAMPTZ NOT NULL,
    billing_period_end  TIMESTAMPTZ NOT NULL,
    pdf_url             VARCHAR(500),
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

---

### 3.7 itura_mood — Mood Tracking Service

```sql
-- mood_entries (TimescaleDB hypertable for time-series)
CREATE TABLE mood_entries (
    id              UUID NOT NULL DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL,
    tenant_id       UUID,
    mood_score      SMALLINT NOT NULL CHECK (mood_score BETWEEN 1 AND 5),
    -- 1=very sad, 2=sad, 3=neutral, 4=happy, 5=very happy
    note            TEXT,
    triggers        VARCHAR(30)[],
    -- work | sleep | family | body | finances | relationships | other
    logged_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    PRIMARY KEY (id, logged_at)  -- required for TimescaleDB
);

-- Convert to TimescaleDB hypertable
SELECT create_hypertable('mood_entries', 'logged_at', chunk_time_interval => INTERVAL '1 month');

-- mood_insights (AI-generated)
CREATE TABLE mood_insights (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     UUID NOT NULL,
    period_start DATE NOT NULL,
    period_end   DATE NOT NULL,
    insight_text TEXT NOT NULL,
    patterns     JSONB,
    -- {best_day: "Friday", avg_score: 3.4, trigger_frequency: {...}}
    generated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- mood_streaks (denormalized for fast reads)
-- Stored in user_streaks table in itura_users
```

---

### 3.8 itura_journal — Journal Service

```sql
-- journal_entries
CREATE TABLE journal_entries (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL,
    tenant_id       UUID,
    title           VARCHAR(200),
    content_encrypted TEXT NOT NULL,                -- AES-256 encrypted
    content_word_count INT NOT NULL DEFAULT 0,
    template_id     UUID REFERENCES journal_templates(id),
    mood_score      SMALLINT,                       -- mood at time of writing
    emotion_tags    VARCHAR(30)[],                  -- anxious | sad | grateful | hopeful | etc.
    is_shared_with_coach BOOLEAN NOT NULL DEFAULT FALSE,
    is_favorite     BOOLEAN NOT NULL DEFAULT FALSE,
    written_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at      TIMESTAMPTZ
);

-- journal_templates
CREATE TABLE journal_templates (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name        VARCHAR(100) NOT NULL,
    description TEXT,
    prompts     JSONB NOT NULL,  -- [{order: 1, question: "What am I grateful for?"}]
    category    VARCHAR(50),    -- gratitude | cbt | reflection | grief | daily
    is_active   BOOLEAN NOT NULL DEFAULT TRUE,
    sort_order  INT NOT NULL DEFAULT 0,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- journal_coach_shares (explicit sharing with coach)
CREATE TABLE journal_coach_shares (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    journal_entry_id UUID NOT NULL REFERENCES journal_entries(id) ON DELETE CASCADE,
    coach_id        UUID NOT NULL,
    shared_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    revoked_at      TIMESTAMPTZ,
    UNIQUE(journal_entry_id, coach_id)
);
```

---

### 3.9 itura_community — Community Service

```sql
-- community_topics
CREATE TABLE community_topics (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name        VARCHAR(100) NOT NULL UNIQUE,
    slug        VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    icon        VARCHAR(50),
    color       VARCHAR(7),
    sort_order  INT NOT NULL DEFAULT 0,
    is_active   BOOLEAN NOT NULL DEFAULT TRUE,
    post_count  INT NOT NULL DEFAULT 0,       -- denormalized
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- community_posts
CREATE TABLE community_posts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL,
    tenant_id       UUID,
    topic_id        UUID NOT NULL REFERENCES community_topics(id),
    post_type       VARCHAR(20) NOT NULL DEFAULT 'story',
                    -- story | question | resource | milestone | prompt
    content         TEXT NOT NULL,
    title           VARCHAR(300),
    is_anonymous    BOOLEAN NOT NULL DEFAULT FALSE,
    anonymous_name  VARCHAR(50),                -- generated anon name
    status          VARCHAR(20) NOT NULL DEFAULT 'published',
                    -- published | pending_review | removed | hidden
    moderation_reason TEXT,
    reaction_count  INT NOT NULL DEFAULT 0,     -- denormalized
    reply_count     INT NOT NULL DEFAULT 0,     -- denormalized
    is_pinned       BOOLEAN NOT NULL DEFAULT FALSE,
    ai_flagged      BOOLEAN NOT NULL DEFAULT FALSE,
    ai_flag_reason  VARCHAR(100),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at      TIMESTAMPTZ
);

-- community_replies
CREATE TABLE community_replies (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    post_id     UUID NOT NULL REFERENCES community_posts(id) ON DELETE CASCADE,
    user_id     UUID NOT NULL,
    parent_id   UUID REFERENCES community_replies(id), -- for nested replies
    content     TEXT NOT NULL,
    is_anonymous BOOLEAN NOT NULL DEFAULT FALSE,
    anonymous_name VARCHAR(50),
    status      VARCHAR(20) NOT NULL DEFAULT 'published',
    reaction_count INT NOT NULL DEFAULT 0,
    ai_flagged  BOOLEAN NOT NULL DEFAULT FALSE,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at  TIMESTAMPTZ
);

-- community_reactions
CREATE TABLE community_reactions (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     UUID NOT NULL,
    target_type VARCHAR(10) NOT NULL,  -- post | reply
    target_id   UUID NOT NULL,
    reaction    VARCHAR(20) NOT NULL,  -- heart | hug | supportive | insightful
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(user_id, target_type, target_id)
);

-- content_reports
CREATE TABLE content_reports (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    reporter_id     UUID NOT NULL,
    content_type    VARCHAR(10) NOT NULL,  -- post | reply
    content_id      UUID NOT NULL,
    reason          VARCHAR(50) NOT NULL,
    description     TEXT,
    status          VARCHAR(20) NOT NULL DEFAULT 'pending',
                    -- pending | reviewed | actioned | dismissed
    reviewed_by     UUID,
    reviewed_at     TIMESTAMPTZ,
    action_taken    VARCHAR(50),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

---

### 3.10 itura_notifications — Notification Service

```sql
-- notifications
CREATE TABLE notifications (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL,
    type            VARCHAR(50) NOT NULL,
    -- session_reminder | booking_confirmed | mood_nudge | streak_risk | community_reply | etc.
    title           VARCHAR(200) NOT NULL,
    body            TEXT NOT NULL,
    data            JSONB,                          -- deep link info, entity IDs
    is_read         BOOLEAN NOT NULL DEFAULT FALSE,
    read_at         TIMESTAMPTZ,
    channels_sent   VARCHAR(20)[],                  -- push | email | sms | in_app
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- notification_deliveries (tracks per-channel delivery status)
CREATE TABLE notification_deliveries (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    notification_id UUID NOT NULL REFERENCES notifications(id) ON DELETE CASCADE,
    channel         VARCHAR(20) NOT NULL,           -- push | email | sms
    status          VARCHAR(20) NOT NULL DEFAULT 'pending',
                    -- pending | sent | delivered | failed | bounced
    provider_reference VARCHAR(200),
    sent_at         TIMESTAMPTZ,
    delivered_at    TIMESTAMPTZ,
    failed_at       TIMESTAMPTZ,
    failure_reason  TEXT,
    retry_count     INT NOT NULL DEFAULT 0,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- device_tokens (for push notifications)
CREATE TABLE device_tokens (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     UUID NOT NULL,
    token       TEXT NOT NULL UNIQUE,
    platform    VARCHAR(10) NOT NULL,  -- ios | android | web
    device_name VARCHAR(100),
    is_active   BOOLEAN NOT NULL DEFAULT TRUE,
    last_used   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

---

### 3.11 itura_corporate — Corporate Service

```sql
-- corporate_accounts
CREATE TABLE corporate_accounts (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    company_name        VARCHAR(200) NOT NULL,
    company_domain      VARCHAR(100) NOT NULL UNIQUE,  -- for email domain matching
    contact_name        VARCHAR(100) NOT NULL,
    contact_email       VARCHAR(255) NOT NULL,
    contact_phone       VARCHAR(20),
    plan_type           VARCHAR(20) NOT NULL DEFAULT 'starter',
    max_seats           INT NOT NULL DEFAULT 100,
    used_seats          INT NOT NULL DEFAULT 0,
    session_credits_pool INT NOT NULL DEFAULT 0,
    contract_start      DATE NOT NULL,
    contract_end        DATE,
    billing_cycle       VARCHAR(20) NOT NULL DEFAULT 'monthly',
    monthly_amount      BIGINT NOT NULL,
    currency            VARCHAR(3) NOT NULL DEFAULT 'NGN',
    status              VARCHAR(20) NOT NULL DEFAULT 'active',
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at          TIMESTAMPTZ
);

-- corporate_memberships
CREATE TABLE corporate_memberships (
    id                      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    corporate_account_id    UUID NOT NULL REFERENCES corporate_accounts(id),
    user_id                 UUID NOT NULL,
    role                    VARCHAR(20) NOT NULL DEFAULT 'employee', -- employee | admin
    session_credits_allocated INT NOT NULL DEFAULT 0,
    session_credits_used    INT NOT NULL DEFAULT 0,
    joined_at               TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    removed_at              TIMESTAMPTZ,
    UNIQUE(corporate_account_id, user_id)
);

-- corporate_wellness_pulse (anonymous survey responses)
CREATE TABLE wellness_pulse_surveys (
    id                      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    corporate_account_id    UUID NOT NULL REFERENCES corporate_accounts(id),
    survey_date             DATE NOT NULL,
    response_count          INT NOT NULL DEFAULT 0,
    avg_wellbeing_score     NUMERIC(3,2),
    avg_stress_score        NUMERIC(3,2),
    avg_engagement_score    NUMERIC(3,2),
    burnout_risk_high_pct   NUMERIC(5,2),
    aggregate_data          JSONB,              -- anonymized aggregate results
    created_at              TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

---

## 4. Indexing Strategy

### 4.1 Index Types and Usage

| Index Type | When to Use | Example |
|---|---|---|
| B-tree (default) | Equality and range queries on sortable columns | `user_id`, `created_at`, `status` |
| Hash | Equality-only lookups | `token_hash`, `email` |
| GIN | Array, JSONB, full-text search | `triggers[]`, `features JSONB` |
| BRIN | Large append-only time-series tables | `mood_entries.logged_at` |
| Partial | Index on subset of rows | `WHERE deleted_at IS NULL` |
| Composite | Multi-column queries | `(user_id, created_at DESC)` |

### 4.2 Key Indexes Per Service

```sql
-- auth-service
CREATE INDEX idx_accounts_email ON accounts(email);
CREATE INDEX idx_accounts_status ON accounts(status) WHERE deleted_at IS NULL;
CREATE INDEX idx_refresh_tokens_account ON refresh_tokens(account_id) WHERE revoked_at IS NULL;

-- user-service
CREATE INDEX idx_user_profiles_tenant ON user_profiles(tenant_id) WHERE tenant_id IS NOT NULL;
CREATE INDEX idx_user_streaks_user ON user_streaks(user_id);
CREATE INDEX idx_badges_earned_user ON badges_earned(user_id);

-- coach-service
CREATE INDEX idx_coach_profiles_status ON coach_profiles(verification_status) WHERE deleted_at IS NULL;
CREATE INDEX idx_coach_profiles_country ON coach_profiles(country) WHERE verification_status = 'verified';
CREATE INDEX idx_coach_specialties_specialty ON coach_specialties(specialty);
CREATE INDEX idx_coach_availability_coach ON coach_availability(coach_id, day_of_week) WHERE is_active = TRUE;
CREATE INDEX idx_coach_reviews_coach ON coach_reviews(coach_id, rating) WHERE is_visible = TRUE;

-- booking-service
CREATE INDEX idx_bookings_user ON bookings(user_id, scheduled_at DESC);
CREATE INDEX idx_bookings_coach ON bookings(coach_id, scheduled_at);
CREATE INDEX idx_bookings_status ON bookings(status, scheduled_at) WHERE status = 'confirmed';

-- mood-service (TimescaleDB)
CREATE INDEX idx_mood_entries_user_time ON mood_entries(user_id, logged_at DESC);
-- TimescaleDB automatically creates per-chunk indexes

-- journal-service
CREATE INDEX idx_journal_entries_user ON journal_entries(user_id, written_at DESC) WHERE deleted_at IS NULL;
CREATE INDEX idx_journal_entries_emotion ON journal_entries USING GIN(emotion_tags);

-- community-service
CREATE INDEX idx_community_posts_topic ON community_posts(topic_id, created_at DESC) 
    WHERE status = 'published' AND deleted_at IS NULL;
CREATE INDEX idx_community_posts_user ON community_posts(user_id, created_at DESC) 
    WHERE deleted_at IS NULL;
CREATE INDEX idx_community_reactions_target ON community_reactions(target_type, target_id);

-- payment-service
CREATE INDEX idx_payment_transactions_user ON payment_transactions(user_id, created_at DESC);
CREATE INDEX idx_payment_transactions_status ON payment_transactions(status, created_at) 
    WHERE status = 'pending';
CREATE UNIQUE INDEX idx_payment_idempotency ON payment_transactions(idempotency_key);
```

---

## 5. Partitioning Strategy

### 5.1 Mood Entries (TimescaleDB — Time Partitioning)

```sql
-- Already configured via create_hypertable above
-- TimescaleDB creates monthly chunks automatically
-- Query example: always include time filter for performance
SELECT * FROM mood_entries
WHERE user_id = $1 AND logged_at >= NOW() - INTERVAL '30 days'
ORDER BY logged_at DESC;
```

### 5.2 Notifications (Range Partitioning by Month)

```sql
CREATE TABLE notifications (
    -- ... columns ...
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
) PARTITION BY RANGE (created_at);

CREATE TABLE notifications_2026_01 PARTITION OF notifications
    FOR VALUES FROM ('2026-01-01') TO ('2026-02-01');

CREATE TABLE notifications_2026_02 PARTITION OF notifications
    FOR VALUES FROM ('2026-02-01') TO ('2026-03-01');
-- Auto-partitioning via pg_partman extension
```

### 5.3 Analytics Events (Range + Hash Partitioning)

```sql
-- Partition by month, then by user_id hash for even distribution
CREATE TABLE analytics_events (
    id          UUID NOT NULL,
    user_id     UUID NOT NULL,
    event_type  VARCHAR(50) NOT NULL,
    properties  JSONB,
    occurred_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
) PARTITION BY RANGE (occurred_at);
```

---

## 6. Audit Tables

Every critical table has a corresponding audit table tracking all changes:

```sql
-- Generic audit log pattern (applied to: users, coaches, bookings, payments, subscriptions)
CREATE TABLE user_profiles_audit (
    audit_id        UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    record_id       UUID NOT NULL,              -- references user_profiles.id
    operation       VARCHAR(10) NOT NULL,        -- INSERT | UPDATE | DELETE
    old_values      JSONB,                       -- previous row state
    new_values      JSONB,                       -- new row state
    changed_by      UUID,                        -- user/admin who made the change
    changed_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    ip_address      INET,
    user_agent      TEXT
);

-- Trigger to auto-populate audit log
CREATE OR REPLACE FUNCTION audit_trigger()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO user_profiles_audit(record_id, operation, old_values, new_values, changed_at)
    VALUES (
        COALESCE(NEW.id, OLD.id),
        TG_OP,
        CASE WHEN TG_OP != 'INSERT' THEN row_to_json(OLD) ELSE NULL END,
        CASE WHEN TG_OP != 'DELETE' THEN row_to_json(NEW) ELSE NULL END,
        NOW()
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER audit_user_profiles
    AFTER INSERT OR UPDATE OR DELETE ON user_profiles
    FOR EACH ROW EXECUTE FUNCTION audit_trigger();
```

---

## 7. Soft Delete Strategy

### Implementation

All user-facing entities use soft delete via `deleted_at TIMESTAMPTZ NULL`:

```sql
-- Soft delete: set deleted_at, never physically remove
UPDATE user_profiles SET deleted_at = NOW(), updated_by = $admin_id WHERE id = $user_id;

-- All queries filter soft-deleted rows via EF Core global query filter
builder.Entity<UserProfile>()
    .HasQueryFilter(x => x.DeletedAt == null);

-- Hard delete: only run by data retention job after 90 days
-- For GDPR right to erasure: replace PII with hashed values, then soft-delete
UPDATE user_profiles SET
    full_name = 'Deleted User',
    avatar_url = NULL,
    bio = NULL,
    date_of_birth = NULL,
    deleted_at = NOW()
WHERE id = $user_id;
```

### Retention Policy

| Data Type | Soft Delete | Hard Delete |
|---|---|---|
| User profiles | On account deletion | 90 days after soft delete |
| Journal entries | On user deletion | 90 days after soft delete |
| Community posts | On user deletion | Anonymized immediately, retained for community continuity |
| Mood entries | On user deletion | 90 days |
| Session recordings | On user request | Immediately on request |
| Payment records | Never soft-deleted | Retained 7 years (financial compliance) |
| Auth audit logs | Never deleted | Retained 3 years |

---

## 8. Multi-Tenant Strategy

### 8.1 Row-Level Security (PostgreSQL RLS)

```sql
-- Enable RLS on tenant-scoped tables
ALTER TABLE user_profiles ENABLE ROW LEVEL SECURITY;

-- Policy: users see only their tenant's data
-- (set via application-managed session variable)
CREATE POLICY tenant_isolation ON user_profiles
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID
        OR tenant_id IS NULL          -- global individual users
        OR current_setting('app.is_admin')::BOOLEAN = TRUE
    );

-- Application sets tenant context on connection:
-- SET LOCAL app.current_tenant_id = '{tenantId}';
-- SET LOCAL app.is_admin = 'false';
```

### 8.2 Tenant Context in .NET

```csharp
// ITenantContext scoped per request
public class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }
    public bool IsGlobalAdmin { get; private set; }

    public void Initialize(ClaimsPrincipal user)
    {
        TenantId = user.FindFirst("tenantId") is { } c ? Guid.Parse(c.Value) : null;
        IsGlobalAdmin = user.IsInRole("Admin");
    }
}

// EF Core interceptor sets PostgreSQL session variable
public class TenantDbConnectionInterceptor : DbConnectionInterceptor
{
    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(...)
    {
        var command = connection.CreateCommand();
        command.CommandText = $"SET LOCAL app.current_tenant_id = '{_tenantContext.TenantId ?? Guid.Empty}';";
        await command.ExecuteNonQueryAsync();
    }
}
```

---

## 9. Encryption Strategy

### 9.1 Data Classification

| Tier | Data | Encryption |
|---|---|---|
| **Tier 1 — Critical PII** | Journal entries, session notes, health assessment responses | AES-256-GCM at application layer |
| **Tier 2 — Sensitive PII** | Name, email, phone, bank account numbers | Column-level encryption via pgcrypto |
| **Tier 3 — Standard** | Coach profiles, booking details, mood scores | TDE (Transparent Data Encryption) via Azure |
| **Tier 4 — Public** | Community posts (public), coach public profiles | No additional encryption |

### 9.2 Journal Entry Encryption

```csharp
// AES-256-GCM encryption in JournalService
public class JournalEncryptionService
{
    private readonly byte[] _key; // from Azure Key Vault, rotated quarterly

    public string Encrypt(string plaintext)
    {
        using var aes = new AesGcm(_key, AesGcm.TagByteSizes.MaxSize);
        var nonce = RandomNumberGenerator.GetBytes(AesGcm.NonceByteSizes.MaxSize);
        var ciphertext = new byte[Encoding.UTF8.GetByteCount(plaintext)];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize];

        aes.Encrypt(nonce, Encoding.UTF8.GetBytes(plaintext), ciphertext, tag);

        // Store: nonce || tag || ciphertext (base64 encoded)
        return Convert.ToBase64String(nonce.Concat(tag).Concat(ciphertext).ToArray());
    }
}
```

---

## 10. Migration Strategy

### 10.1 EF Core Migrations

- One migration project per service
- Migrations run on service startup (in non-production environments)
- In production: migrations run as Kubernetes Job before deployment
- Migration naming: `{YYYYMMDD}_{Description}` (e.g., `20260522_AddWellnessLevel`)

### 10.2 Migration Safety Rules

| Rule | Detail |
|---|---|
| Never destructive in one step | Add new column → backfill → remove old column (3 separate deployments) |
| No blocking operations | Use `CREATE INDEX CONCURRENTLY`; avoid `ALTER TABLE` on large tables in single transaction |
| Always reversible | Every migration has a `Down()` method |
| Data migrations separate | DDL changes and data migrations in separate migration files |
| Test in staging first | All migrations applied to staging 24h before production |

---

*End of Database Design Document*  
*Next: [BACKEND_TASKS.md](./BACKEND_TASKS.md)*
