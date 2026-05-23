# ITURA — Product Requirements Document (PRD)

**Document Version:** 1.0  
**Status:** Engineering-Ready  
**Owner:** Product Management  
**Last Updated:** May 2026

---

## Table of Contents

1. [Product Overview](#1-product-overview)
2. [Core Features](#2-core-features)
3. [MVP Scope](#3-mvp-scope)
4. [Feature Prioritization](#4-feature-prioritization)
5. [Product Flows](#5-product-flows)
6. [Retention Features](#6-retention-features)
7. [Gamification Strategy](#7-gamification-strategy)
8. [AI Strategy](#8-ai-strategy)
9. [Notifications Strategy](#9-notifications-strategy)
10. [Community Features](#10-community-features)
11. [Subscription Features](#11-subscription-features)
12. [Emotional Engagement Features](#12-emotional-engagement-features)
13. [Admin Features](#13-admin-features)
14. [Analytics Requirements](#14-analytics-requirements)

---

## 1. Product Overview

### 1.1 Product Vision

Itura is the daily emotional operating system for people who want to feel better, think clearer, and live with intention. It combines professional mental health support with AI-powered companionship, community connection, and habit-forming wellness tools in a single platform designed for daily use.

### 1.2 Product Principles

| Principle | Description |
|---|---|
| **2-Minute Value** | Every feature must deliver emotional value within 2 minutes of use |
| **Calm Over Anxiety** | UI/UX must reduce, not increase, anxiety |
| **Zero Judgment** | No feature should make a user feel ashamed of their mental state |
| **Progressive Depth** | Features offer a simple entry point with depth for those who want it |
| **Human + AI** | AI handles availability and scale; humans handle depth and empathy |
| **Privacy Default** | The most private option should always be the default |
| **Earn Daily Trust** | The product must re-earn user trust every single day |

### 1.3 Product Surfaces

| Surface | Technology | Target Users |
|---|---|---|
| Web App | Next.js 14 | Desktop professionals, corporate users |
| Mobile App (iOS) | Flutter | Primary daily use surface |
| Mobile App (Android) | Flutter | Primary daily use surface |
| Admin Portal | Next.js | Platform administrators |
| Coach Portal | Next.js (integrated) | Coaches and therapists |
| Corporate Dashboard | Next.js (integrated) | HR managers |
| Public API | REST / Webhook | Enterprise integrations |

---

## 2. Core Features

### Feature Map

```
ITURA PLATFORM
│
├── DAILY WELLNESS
│   ├── Mood Tracker
│   ├── Daily Journal
│   ├── Guided Breathing
│   ├── Gratitude Practice
│   └── Daily Affirmations
│
├── PROFESSIONAL SUPPORT
│   ├── Coach/Therapist Discovery
│   ├── Session Booking (Video/Voice/Text)
│   ├── Group Sessions
│   └── Async Messaging
│
├── AI COMPANION
│   ├── Conversational AI (Sera)
│   ├── Mood-Aware Responses
│   ├── Crisis Detection
│   └── Wellness Recommendations
│
├── COMMUNITY
│   ├── Topic Forums
│   ├── Support Groups
│   ├── Wellness Challenges
│   └── Coach Q&As
│
├── GROWTH & GAMIFICATION
│   ├── Streak System
│   ├── Badges & Achievements
│   ├── Wellness Level
│   └── Challenges
│
├── CONTENT LIBRARY
│   ├── Guided Meditations
│   ├── Wellness Articles
│   ├── Podcasts
│   └── Workshops
│
├── RELATIONSHIP WELLNESS
│   ├── Couples Dashboard
│   ├── Joint Check-Ins
│   └── Relationship Exercises
│
└── CORPORATE
    ├── Team Dashboard
    ├── Wellness Analytics
    ├── Bulk Sessions
    └── EAP Features
```

---

## 3. MVP Scope

### MVP Definition

The MVP must prove 3 core hypotheses:
1. Users will log their mood daily if the experience is fast and rewarding
2. Users will engage with an AI companion for emotional support
3. Users will book therapy/coaching sessions through the platform

### MVP Feature Set (Sprint 1–8 / Month 1–4)

| # | Feature | Priority | Sprint |
|---|---|---|---|
| 1 | User Registration & Login (Email + Google) | P0 | 1 |
| 2 | Onboarding Questionnaire & Wellness Assessment | P0 | 1 |
| 3 | Daily Mood Check-In (emoji + note + tags) | P0 | 1–2 |
| 4 | Mood History Dashboard (7/30/90 day view) | P0 | 2 |
| 5 | Personal Journal (rich text) | P0 | 2–3 |
| 6 | AI Companion (Sera) — text conversation | P0 | 3–4 |
| 7 | Crisis Detection & Safety Protocol | P0 | 4 |
| 8 | Coach Profile & Discovery | P0 | 3 |
| 9 | Coach Availability & Booking Flow | P0 | 4–5 |
| 10 | Video Session (Agora/Daily.co integration) | P0 | 5 |
| 11 | Payment Integration (Paystack + Stripe) | P0 | 5 |
| 12 | Subscription Plans (Free, Pro, Premium) | P0 | 5–6 |
| 13 | Push Notifications (session reminders, mood nudge) | P0 | 6 |
| 14 | Email Notifications (transactional) | P0 | 6 |
| 15 | Basic Community Feed (read + post) | P1 | 6–7 |
| 16 | Coach Rating & Review (post-session) | P0 | 7 |
| 17 | User Profile & Settings | P0 | 7 |
| 18 | Admin Dashboard (users, coaches, payments) | P0 | 7–8 |
| 19 | Coach Payout Management | P0 | 8 |
| 20 | Streak System (mood & journal streaks) | P1 | 8 |

### Post-MVP (Month 5–12)

| Feature | Timeline |
|---|---|
| Gamification (badges, achievements, levels) | Month 5–6 |
| Community groups and moderation | Month 5–6 |
| Async messaging with coaches | Month 6 |
| Corporate wellness dashboard | Month 7–8 |
| Group sessions | Month 7–8 |
| Content library (meditations, articles) | Month 6–7 |
| Couples features | Month 9–10 |
| Advanced mood analytics & AI insights | Month 8–9 |
| Offline mode (mobile) | Month 10 |
| Local language support | Month 11–12 |

---

## 4. Feature Prioritization

### MoSCoW Analysis

#### Must Have (MVP — Launch Blockers)

- User authentication and secure account management
- Daily mood check-in with streak tracking
- Personal encrypted journal
- AI emotional companion (Sera) with crisis detection
- Coach/therapist marketplace with verified profiles
- Video + async session booking and delivery
- Subscription billing (Paystack for Africa, Stripe globally)
- Push and email notifications
- Basic community feed
- Admin panel for platform management
- Coach verification and payout system

#### Should Have (Month 3–6)

- Gamification: badges, achievements, level system
- Advanced community: groups, challenges, Q&As
- Corporate wellness dashboard
- Group coaching sessions
- Content library (meditations, articles, podcasts)
- Mood insights with AI pattern analysis
- Async messaging between sessions
- Coach session notes system

#### Could Have (Month 6–12)

- Couples/relationship wellness module
- Family dashboard and parenting tools
- Multilingual support (Yoruba, Igbo, Hausa, Pidgin)
- Offline journaling and mood tracking
- Wearable data integration (Apple Health, Google Fit)
- AI voice companion
- In-app wellness challenges with community leaderboard
- Referral program with rewards

#### Won't Have (Year 1)

- Insurance billing integration
- Prescription management
- Clinical trial management system
- Medical records integration
- Telehealth prescriptions
- Native smartwatch app

---

## 5. Product Flows

### 5.1 Onboarding Flow

```
Step 1: Welcome Screen
  → Brand introduction + value proposition
  → "Get Started" CTA

Step 2: Account Creation
  → Email + Password | or | Google OAuth
  → Phone number (optional, for SMS reminders)

Step 3: Email Verification
  → 6-digit OTP sent to email
  → Resend option after 60 seconds

Step 4: Personal Details
  → First name, age bracket, location (country/city)
  → Profile photo (optional)

Step 5: Wellness Goals (multi-select)
  → Manage anxiety
  → Improve sleep
  → Process grief
  → Strengthen relationships
  → Prevent burnout
  → Build emotional resilience
  → Other

Step 6: Primary Concerns (multi-select)
  → Work stress
  → Relationship issues
  → Depression
  → Academic pressure
  → Family conflict
  → Financial anxiety
  → Loss and grief
  → Self-esteem

Step 7: Wellness Assessment (6 quick questions)
  → Adapted PHQ-9 and GAD-7 prompts
  → Results feed into AI personalization
  → Users NOT shown a clinical score — shown a wellness level

Step 8: Meet Sera (AI Companion)
  → Brief intro animation
  → First AI message based on assessment results
  → User responds → first conversation

Step 9: Personalized Dashboard
  → Tailored to goals and assessment
  → First mood check-in prompted
  → Coach recommendations shown
  → Day 1 streak started
```

---

### 5.2 Mood Tracking Flow

```
Home Screen → Mood Check-In Card
  → "How are you feeling today, [Name]?" 

Step 1: Mood Selection
  → 5 emoji options (Very Sad → Very Happy)
  → 1-tap selection

Step 2: Optional Context
  → "What's on your mind?" (280 char, optional)
  → Trigger tags multi-select (optional):
    [Work] [Sleep] [Family] [Body] [Finances] [Relationships] [Other]

Step 3: Submission
  → Animated confirmation ("Got it, thanks for checking in")
  → Streak counter update (+1 flame icon)
  → AI generates a short empathetic response

Step 4: Insight (if 7+ days of data)
  → "Your mood has been lower on Mondays — want to talk to Sera?"
  → CTA to AI companion or journaling
```

---

### 5.3 Session Booking Flow

```
Step 1: Find a Coach
  → Search bar with filters:
    - Specialty (anxiety, depression, grief, couples, career)
    - Session type (video, voice, text)
    - Language (English, Yoruba, Igbo, etc.)
    - Price range
    - Availability (this week, next week)
    - Gender preference
    - Rating (4+ stars)

Step 2: Coach Profile
  → Photo, name, title, credentials
  → Specialties and approach
  → Session pricing
  → Rating + number of reviews
  → Review excerpts (anonymous)
  → Availability preview (next 3 open slots)
  → "Book Session" CTA

Step 3: Select Time Slot
  → Calendar view with available slots highlighted
  → Timezone auto-detected
  → Session duration options (25 min / 50 min)

Step 4: Session Type
  → Video (default)
  → Voice only
  → Async text (slower, cheaper)

Step 5: Payment
  → Summary: coach, date/time, duration, price
  → Apply coupon code (optional)
  → Use wallet credits (if available)
  → Pay with card (Paystack/Stripe)
  → Or pay with subscription session credit

Step 6: Confirmation
  → Confirmation screen + booking reference
  → Calendar event generated (ICS download + Google Calendar option)
  → Email + push notification sent
  → Coach notified immediately
  → "Add a note for your coach" optional prompt
```

---

### 5.4 AI Companion Flow

```
Entry Points:
  → Home Screen "Chat with Sera" card
  → Post mood check-in suggestion
  → Late-night notification
  → Manual navigation

Conversation Interface:
  → WhatsApp-style chat bubbles
  → Typing indicator (3 dots)
  → Message timestamps
  → Scroll to history
  → "Sera is remembering your previous conversations..."

AI Behaviors:
  → Opens with contextual greeting based on time + last mood
  → Responds within 2–3 seconds
  → Adapts tone to user's emotional state
  → Asks follow-up questions (doesn't just respond)
  → Suggests: journaling, breathing, coach booking based on context

Crisis Protocol:
  → Detects keywords: suicide, self-harm, end it all, can't go on, hopeless
  → Pauses normal conversation
  → Shows: "I hear you, and I'm concerned about your safety."
  → Displays: crisis line number + "Talk to a human" CTA
  → Logs event for clinical review (anonymized)
  → Cannot be dismissed without user acknowledgment
```

---

## 6. Retention Features

### 6.1 Habit Loop Design

Every retention mechanism follows the **Hook Model** (Trigger → Action → Variable Reward → Investment):

| Habit | Trigger | Action | Reward | Investment |
|---|---|---|---|---|
| Daily mood check-in | 8am push notification | 3-tap mood log | Streak flame + AI response | Mood history builds |
| Journaling | Evening nudge | Write journal entry | Insight unlock | Reflection history grows |
| AI companion | Anxiety keyword in journal | Open Sera chat | Emotional relief | Conversation history |
| Community | New reply on post | Read + reply | Social validation | Community reputation |
| Session booking | Weekly wellness report | Book a coach | Professional support | Therapeutic relationship |

### 6.2 Streak System

| Streak Type | What Counts | Reset Behavior |
|---|---|---|
| Mood Streak | 1 mood log per day | Resets if missed; grace period (1 freeze/week for Pro+) |
| Journal Streak | 1 journal entry per day | Resets if missed |
| Wellness Streak | Any wellness activity per day | Most forgiving; includes meditations, breathing |
| Session Streak | Session booked this week | Weekly reset |

**Streak Rewards:**
- 3-day streak: "Consistent!" badge
- 7-day streak: Unlock custom AI companion name
- 14-day streak: 10% off next session
- 30-day streak: Free month upgrade to next tier
- 100-day streak: "Wellness Champion" status + community recognition

### 6.3 Weekly Wellness Summary

Every Sunday, users receive:
- Mood trend for the week (chart)
- Journal entry count
- AI conversations had
- Sessions completed
- Streak status
- Personalized insight ("Your best mood days are Fridays")
- Suggested focus for next week
- Coach recommendation if no session in 30 days

### 6.4 Re-Engagement Campaigns

| Day Since Last Visit | Trigger | Message |
|---|---|---|
| Day 3 | Push + Email | "We miss you. Your streak is at risk 🔥" |
| Day 7 | Push + Email + SMS | "Sera has been thinking about you. How are you?" |
| Day 14 | Email | Personalized wellness report for past 2 weeks |
| Day 30 | Email + In-App | "Your wellness journey doesn't end here." + offer |
| Day 60 | Email | Win-back offer: 1 free premium session |

---

## 7. Gamification Strategy

### 7.1 Wellness Level System

Users progress through 10 wellness levels based on total XP accumulated:

| Level | Name | XP Required | Perks |
|---|---|---|---|
| 1 | Seedling | 0 | Starting state |
| 2 | Sprout | 200 | Unlock custom journal cover |
| 3 | Root | 500 | Unlock AI companion themes |
| 4 | Bud | 1,000 | Unlock community group creation |
| 5 | Bloom | 2,000 | 10% off next session |
| 6 | Branch | 4,000 | Access to premium content library |
| 7 | Canopy | 7,000 | Monthly free session credit |
| 8 | Forest | 12,000 | Priority coach matching |
| 9 | Elder | 20,000 | Beta feature access |
| 10 | Lighthouse | 35,000 | Ambassador status + exclusive badge |

### 7.2 XP Earning Actions

| Action | XP Earned |
|---|---|
| Daily mood check-in | 10 XP |
| Journal entry (> 50 words) | 20 XP |
| Complete a guided meditation | 15 XP |
| AI companion conversation (> 5 messages) | 10 XP |
| Book a coaching session | 50 XP |
| Complete a session | 100 XP |
| Post in community | 10 XP |
| Reply to community post | 5 XP |
| Complete wellness challenge | 50–200 XP |
| 7-day streak bonus | 50 XP |
| Rate a coach | 10 XP |
| Refer a friend who signs up | 100 XP |

### 7.3 Badge System

#### Consistency Badges
- Spark (3-day streak)
- Flame (7-day streak)
- Inferno (30-day streak)
- Eternal Flame (100-day streak)

#### Wellness Badges
- First Step (First mood check-in)
- Journaler (10 journal entries)
- Storyteller (50 journal entries)
- Deep Diver (100 journal entries)
- Session Seeker (First coaching session)
- Growth Seeker (10 sessions completed)

#### Community Badges
- Connector (First community post)
- Supporter (50 community replies)
- Pillar (Received 100 reactions)
- Champion (Completed 3 challenges)

#### Special Badges
- Midnight Owl (AI conversation after midnight)
- Early Bird (Mood check-in before 7am for 7 days)
- Couples Wellness (Complete couples module)
- Corporate Hero (30-day corporate wellness streak)

### 7.4 Wellness Challenges

Monthly challenges that users (individual + community) can join:

| Challenge | Duration | Description | Reward |
|---|---|---|---|
| 30-Day Gratitude | 30 days | 1 gratitude journal per day | 200 XP + badge |
| Mood Awareness Week | 7 days | Log mood twice daily | 100 XP + streak freeze |
| Community Care | 7 days | Reply to 5 community posts | Community badge |
| Corporate Wellness Month | 30 days | Team-based daily wellness activity | Corporate leaderboard |
| Anxiety Toolkit | 14 days | Try a new coping technique daily | Exclusive content unlock |

---

## 8. AI Strategy

### 8.1 AI Companion — Sera

**Persona:**
- Name: Sera (customizable by user)
- Personality: Warm, curious, non-judgmental, gently challenging
- Tone options: Friendly (default), Professional, Spiritual, Direct
- Approach: Motivational Interviewing + CBT-informed + Positive Psychology

**Core Capabilities:**

| Capability | Description |
|---|---|
| Empathic listening | Acknowledges and validates emotions before responding |
| Pattern recognition | References past conversations ("Last week you mentioned...") |
| Goal alignment | Connects conversations to user's stated wellness goals |
| Journaling prompts | Suggests personalized writing prompts based on mood |
| Coping suggestions | Recommends specific techniques (breathing, grounding, CBT) |
| Coach bridging | Suggests booking a coach when issues exceed AI scope |
| Crisis detection | Identifies crisis signals and activates safety protocol |
| Cultural sensitivity | Adapts to Nigerian cultural context and references |

**AI Model Stack:**
- Primary: GPT-4o (OpenAI) via Azure OpenAI Service
- Fine-tuned: On mental wellness conversation datasets + RAFT (RAG fine-tuning)
- Safety layer: Custom pre/post processing for harmful content detection
- Memory: Conversation history stored in MongoDB, summarized for context injection

### 8.2 AI Personalization Engine

| Signal | Source | Use |
|---|---|---|
| Mood history | Mood tracker | Tone calibration, proactive check-ins |
| Journal content | Journal (with permission) | Conversation context, prompt generation |
| Session history | Booking data | Coach recommendations |
| Time of day | System | Greeting style, content recommendations |
| Wellness goals | Onboarding | Goal-aligned responses |
| Trigger tags | Mood tracker | Pattern insights |
| Subscription tier | Account | Feature access |

### 8.3 AI Safety Framework

| Risk | Control |
|---|---|
| Harmful advice | System prompt guardrails + output filtering |
| Clinical diagnosis | Explicit rejection with coaching toward professional help |
| Crisis / suicidality | Multi-signal detection + mandatory safety response |
| Personal data leakage | No PII in model prompts; anonymized context only |
| Bias / cultural harm | Diverse training data + human review of flagged outputs |
| Dependency/attachment | Weekly prompts encouraging human connection |

### 8.4 AI Moderation (Community)

- Automated pre-publish content screening
- Hate speech, self-harm content, medical misinformation detection
- Confidence-scored → low confidence flagged for human review
- High-risk content auto-held + user notified
- Appeals handled by human moderators within 24 hours

---

## 9. Notifications Strategy

### 9.1 Notification Types

| Type | Channel | Purpose |
|---|---|---|
| Daily mood nudge | Push | Habit formation (configurable time) |
| Streak at risk | Push | Retention (sent if no check-in by 7pm) |
| Session reminder | Push + Email + SMS | 24hr and 1hr before session |
| Session confirmation | Email | Immediate after booking |
| Coach message | Push + In-app | Async message received |
| AI response | In-app | Response ready in chat |
| Weekly summary | Email | Every Sunday evening |
| Community reply | Push + In-app | Reply on user's post |
| Payout processed | Email (coach) | Payment cleared |
| Subscription renewal | Email | 7 days before renewal |
| System alerts | Email | Security, account actions |

### 9.2 Notification Personalization Rules

```
IF user is on Free tier AND has not checked in for 2 days
  → Send re-engagement push: "Your mood tracker misses you"

IF user completed session AND has NOT booked next session within 7 days
  → Send: "Ready for your next session with [Coach Name]?"

IF user mood average drops below threshold (3/5) for 5 consecutive days
  → Send: "We've noticed you've been having a tough stretch. Sera is here."

IF user streak > 7 days AND has not upgraded
  → Send: "You're on a 7-day streak! Unlock more with Pro."
```

### 9.3 Do Not Disturb

- Users set quiet hours (default: 10pm–8am)
- No push notifications during quiet hours except crisis alerts
- Crisis alerts bypass all DND settings

### 9.4 Notification Preferences

Users can configure per-channel, per-type preferences:

| Setting | Options |
|---|---|
| Daily mood nudge | On/Off + time selection |
| Streak alerts | On/Off |
| Session reminders | On/Off + timing (24hr/2hr/30min) |
| Community notifications | On/Off + frequency (instant/daily digest) |
| Marketing emails | On/Off |
| SMS notifications | On/Off |

---

## 10. Community Features

### 10.1 Community Structure

```
Community
├── Topics (public channels)
│   ├── Anxiety & Stress
│   ├── Depression & Low Mood
│   ├── Grief & Loss
│   ├── Relationships
│   ├── Work & Burnout
│   ├── Student Life
│   ├── Parenting
│   ├── Spiritual Wellness
│   └── General Wellness
│
├── Support Groups (moderated, invite-join)
│   ├── Anxiety Warriors (open join)
│   ├── Grief Together (moderated join)
│   ├── Sobriety Support (invite-only)
│   └── Corporate Burnout (open join)
│
└── Events
    ├── Coach Q&A (weekly live sessions)
    ├── Wellness Challenges
    └── Community Meetups (virtual)
```

### 10.2 Post Types

| Type | Description |
|---|---|
| Story | Personal experience share (up to 1,000 chars) |
| Question | Ask the community for advice |
| Resource | Share an article, book, technique |
| Milestone | Celebrate progress ("30 days streak!") |
| Prompt | Community responds to a daily wellness prompt |

### 10.3 Anonymity Controls

- Anonymous posting option on every post
- Anonymous name generated (e.g., "Quiet Canopy", "Calm River")
- Anonymous identity consistent within a thread (same post = same anon name)
- Coaches cannot see real names behind anonymous posts

### 10.4 Moderation Rules

| Rule | Enforcement |
|---|---|
| No medical advice (prescriptions, diagnoses) | AI filter + human review |
| No self-harm content | AI filter → auto-remove + safety message |
| No harassment or hate speech | AI filter → auto-remove + user warning |
| No spam or promotional content | AI filter → auto-remove |
| Trigger warnings | Prompt shown for sensitive topics; users can add TW |
| No sharing of personal contact information | AI filter |

---

## 11. Subscription Features

### 11.1 Feature Access by Tier

| Feature | Free | Pro | Premium | Executive |
|---|---|---|---|---|
| Mood check-in | ✅ (7-day history) | ✅ (unlimited) | ✅ | ✅ |
| Journal | ✅ (3/week) | ✅ (unlimited) | ✅ | ✅ |
| AI companion | ✅ (5 msg/day) | ✅ (50 msg/day) | ✅ (unlimited) | ✅ (unlimited) |
| Community (read) | ✅ | ✅ | ✅ | ✅ |
| Community (post) | ❌ | ✅ | ✅ | ✅ |
| Session credits | 1 (signup) | 1/month | 2/month | 4/month |
| Group sessions | ❌ | ❌ | ✅ (2/month) | ✅ (unlimited) |
| Couples features | ❌ | ❌ | ✅ | ✅ |
| Content library | ❌ | Basic | Full | Full |
| Mood analytics | Basic | Full | Full + AI insights | Full + AI |
| Streak freeze | ❌ | 1/week | 2/week | Unlimited |
| Priority support | ❌ | ❌ | ✅ | ✅ |
| Concierge | ❌ | ❌ | ❌ | ✅ |
| API access | ❌ | ❌ | ❌ | ✅ |

### 11.2 Subscription Management

- Stripe + Paystack webhooks manage subscription lifecycle
- Grace period: 3 days on failed payment before downgrade
- Downgrade preserves data (mood history, journals) — only restricts features
- Annual subscribers get 15–25% discount depending on tier
- Free tier never expires; users can return after canceling paid

### 11.3 Corporate Subscription

- HR admin creates corporate account
- Sets employee seat count
- Employees invited via email domain or unique code
- Corporate seats assigned employee Pro tier (minimum)
- HR admin sees anonymous aggregate data only
- Individual employees retain full privacy

---

## 12. Emotional Engagement Features

### 12.1 Guided Breathing Exercises

- Box breathing (4-4-4-4)
- 4-7-8 breathing (anxiety relief)
- Coherence breathing (stress regulation)
- Visual guide (animated circle expands/contracts)
- Background ambient sounds (optional)
- Duration: 1, 3, 5 minutes
- Accessible without login (discovery/acquisition)

### 12.2 Grounding Exercises

- 5-4-3-2-1 technique (sensory grounding)
- Body scan (3-minute guided audio)
- Progressive muscle relaxation
- Available in AI companion ("Let's try a grounding exercise together")
- Also accessible as standalone cards in Daily Wellness tab

### 12.3 Daily Affirmations

- Personalized affirmations based on:
  - User's wellness goals
  - Current mood
  - Day of week / time of day
- Delivered as home screen card or notification
- User can favorite and collect affirmations
- Shareable as cards (social sharing with Itura watermark)

### 12.4 Gratitude Practice

- Daily gratitude prompt (3 things I'm grateful for)
- Gratitude history visible as a "sunshine wall"
- Weekly gratitude review in Sunday summary
- Coach can assign gratitude homework between sessions
- Couples version: share gratitudes with partner

### 12.5 CBT Tools

- Thought record templates (Situation → Thought → Feeling → Evidence For/Against → Balanced Thought)
- Cognitive distortion identifier (AI-assisted)
- Behavioral activation planner
- Worry time scheduling
- Accessible via journal templates and AI companion

---

## 13. Admin Features

### 13.1 Super Admin Dashboard

**Overview Panel:**
- Total users (registered, active, paying)
- Total coaches (pending, verified, active, suspended)
- Revenue (today, MTD, YTD) with trend chart
- Sessions (booked, completed, canceled)
- Mood check-ins today
- AI conversations today
- Active subscriptions by tier

**User Management:**
- Search by name, email, phone, status
- View user profile, subscription, activity, session history
- Actions: Suspend, Restore, Force logout, Delete (GDPR), Send email
- View audit log for any user

**Coach Management:**
- Verification queue (new applications)
- View coach credentials, documents, interview status
- Approve / Reject / Request more info
- Monitor coach activity (sessions, ratings, complaints)
- Manage coach payout schedule
- Suspend/ban coach with reason

**Content Moderation:**
- Community post queue (flagged + AI-uncertain)
- View reported content with reporter reason
- Actions: Approve, Remove, Warn user, Ban user
- Appeals queue
- Bulk moderation tools

**Financial Management:**
- Subscription revenue by tier and period
- Session commission ledger
- Coach payout queue (approve/schedule)
- Corporate billing management
- Refund processing
- Fraud flags and review queue

**System Settings:**
- Feature flags (enable/disable features per tier/region)
- AI model configuration (toggle models, safety level)
- Notification template management
- Subscription plan pricing management
- Platform maintenance mode

### 13.2 Coach Admin Portal

- Personal profile management
- Calendar and availability management
- Client list (active, past)
- Session history with notes
- Earnings dashboard (by period, by session)
- Bank account / mobile money management
- Performance metrics (rating, completion rate, rebooking rate)
- Client messaging (async)
- Resource sharing with clients

---

## 14. Analytics Requirements

### 14.1 Product Analytics

**Events to Track:**

| Event | Properties |
|---|---|
| `user_registered` | method, referral_source, device |
| `onboarding_completed` | goals, assessment_score, time_to_complete |
| `mood_logged` | mood_score, has_note, trigger_tags, streak_day |
| `journal_entry_created` | word_count, template_used, mood_at_time |
| `ai_conversation_started` | entry_point, user_tier |
| `ai_message_sent` | message_length, sentiment, response_time |
| `crisis_detected` | keyword_type (anonymized), action_taken |
| `coach_searched` | filters_used, results_count |
| `session_booked` | coach_id, session_type, price, payment_method |
| `session_completed` | duration, coach_id, user_rating |
| `subscription_upgraded` | from_tier, to_tier, trigger |
| `subscription_canceled` | tier, reason, days_active |
| `community_post_created` | topic, is_anonymous, post_type |
| `badge_earned` | badge_id, trigger_event |
| `streak_broken` | streak_length, last_activity_type |

### 14.2 Business Analytics (Admin)

| Report | Frequency | Audience |
|---|---|---|
| Daily Active Users | Daily | Product, Leadership |
| Revenue by stream | Daily | Finance, Leadership |
| Cohort retention | Weekly | Product |
| Feature adoption | Weekly | Product |
| Coach performance | Weekly | Operations |
| Corporate wellness summaries | Monthly | Corporate clients |
| Platform health scorecard | Monthly | Leadership |

### 14.3 Analytics Stack

| Tool | Purpose |
|---|---|
| PostHog (self-hosted) | Product analytics, funnels, cohorts |
| Azure Monitor | Infrastructure and API metrics |
| Grafana | Real-time dashboards |
| Metabase | Business intelligence and SQL reports |
| Custom admin dashboard | Revenue, users, coaches |

### 14.4 Data Privacy in Analytics

- No PII in analytics events (use UUID references only)
- Journal content never logged to analytics
- AI conversation content never logged (only metadata)
- Mood scores tracked as ranges, not exact values in aggregate analytics
- GDPR-compliant analytics: user can request deletion of analytics data

---

*End of Product Requirements Document*  
*Next: [ARCHITECTURE.md](./ARCHITECTURE.md)*
