# ITURA — Business Requirements Document (BRD)

**Document Version:** 1.0  
**Status:** Approved for Engineering  
**Owner:** Product & Strategy  
**Last Updated:** May 2026  
**Classification:** Confidential

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Vision & Mission](#2-vision--mission)
3. [Problem Statement](#3-problem-statement)
4. [Objectives](#4-objectives)
5. [Business Goals](#5-business-goals)
6. [Market Opportunity](#6-market-opportunity)
7. [User Personas](#7-user-personas)
8. [Stakeholders](#8-stakeholders)
9. [Functional Requirements](#9-functional-requirements)
10. [Non-Functional Requirements](#10-non-functional-requirements)
11. [User Journeys](#11-user-journeys)
12. [User Stories](#12-user-stories)
13. [Acceptance Criteria](#13-acceptance-criteria)
14. [Monetization Strategy](#14-monetization-strategy)
15. [KPIs](#15-kpis)
16. [Compliance Requirements](#16-compliance-requirements)
17. [Risk Assessment](#17-risk-assessment)
18. [Future Expansion Plans](#18-future-expansion-plans)

---

## 1. Executive Summary

Itura is a next-generation Mental Wellness and Emotional Wellbeing Platform that serves as the daily emotional operating system for individuals, couples, families, students, professionals, and corporate teams across Africa and globally. The platform transcends the traditional therapy-booking model by providing a holistic, AI-powered, community-driven ecosystem that encourages habitual daily engagement.

The mental health treatment gap in Africa exceeds 90%, meaning fewer than 1 in 10 people who need care receive it. Itura addresses this crisis through a deeply integrated technology platform combining licensed professional care, AI companionship, peer community support, gamified wellness tools, and corporate wellbeing solutions — all in a single, affordable, culturally responsive platform.

The platform will be built on a microservices architecture using .NET 8, Next.js, and Flutter, hosted on Azure Kubernetes Service, and will support millions of concurrent users from launch. Revenue is generated through a freemium-to-premium subscription model, a marketplace commission on coaching sessions, corporate wellness licensing, and AI companion subscriptions.

**Investment Requirement:** $1.2M seed (engineering + operations + marketing)  
**Break-Even Point:** Month 18  
**Projected Year 1 ARR:** $2M  
**Projected Year 3 ARR:** $50M  

---

## 2. Vision & Mission

### Vision
To become the most trusted daily emotional wellness companion for 100 million people across Africa and emerging markets by 2030, eliminating the stigma of mental health care and making evidence-based emotional support accessible, affordable, and culturally relevant.

### Mission
To empower every person to build emotional resilience, find professional support when needed, and belong to a community that understands their mental health journey — through technology that feels human, intelligent, and deeply caring.

### Brand Promise
Itura promises every user three things:
- **Safety:** Your emotional data is private, protected, and never weaponized
- **Accessibility:** Quality wellness support available at every income level
- **Continuity:** Tools that grow with you and meet you where you are, every day

### Core Values

| Value | Description |
|---|---|
| **Empathy First** | Every product decision is filtered through its emotional impact on users |
| **Radical Accessibility** | No user should be priced out of mental wellness support |
| **Cultural Authenticity** | Content, coaching, and community reflect lived African experience |
| **Privacy by Design** | Emotional and health data protected with the highest standards |
| **Evidence-Based** | All wellness interventions grounded in clinical and behavioral research |
| **Daily Habit Formation** | Platform designed for habitual daily use, not crisis-only intervention |
| **Inclusive Safety** | Safe for all genders, ages, sexual orientations, and mental health journeys |

---

## 3. Problem Statement

### 3.1 The African Mental Health Crisis

The scale of unmet mental health need in Africa is a public health emergency:

- **Treatment Gap:** Over 90% of Africans who need mental health treatment receive none (WHO 2023)
- **Provider Shortage:** Nigeria has approximately 1 psychiatrist per 1,000,000 people vs. the recommended ratio of 1 per 10,000
- **Cost Barrier:** A single therapy session costs ₦30,000–₦80,000 ($18–$50) — unaffordable for most Nigerians
- **Stigma:** 72% of Nigerians surveyed cite social stigma as the primary reason for not seeking mental health help
- **Digital Gap:** Existing global platforms (BetterHelp, Headspace, Calm) are designed for Western audiences and priced in USD
- **Corporate Neglect:** Less than 5% of Nigerian companies have active Employee Assistance Programs (EAPs)
- **Youth Crisis:** 64% of African youth aged 15–24 report persistent feelings of anxiety or depression with no access to help

### 3.2 The Product Gap

| Problem | Current Solutions | Itura's Solution |
|---|---|---|
| High therapy cost | No affordable alternatives | Tiered pricing; AI companion fills the gap |
| Cultural mismatch | Western-centric platforms | Afrocentric coaches, content, and language |
| Episodic engagement | Users only engage during crisis | Daily habit loops, streaks, gamification |
| Fragmented tools | Separate apps for different needs | Unified ecosystem: therapy + AI + community |
| No peer community | Isolated mental health journey | Moderated peer support community |
| No 24/7 support | Human availability limited | AI companion available round the clock |
| Corporate neglect | No B2B wellness infrastructure | Full corporate wellness suite and EAP |
| No offline support | All platforms require connectivity | Offline journal and mood tracking |

### 3.3 Business Opportunity

The convergence of smartphone proliferation (Africa now has 650M smartphone users), growing middle class, post-COVID mental health awareness, and the failure of existing platforms to serve African users creates a once-in-a-generation opportunity for a culturally-intelligent, technology-first mental wellness platform.

---

## 4. Objectives

### 4.1 Strategic Objectives (Year 1)

| # | Objective | Metric | Timeline |
|---|---|---|---|
| S1 | Launch production-ready MVP | Go-live event | Month 6 |
| S2 | Onboard verified coaches | 500 coaches | Month 12 |
| S3 | Achieve monthly active user base | 50,000 MAU | Month 12 |
| S4 | Close corporate wellness contracts | 10 enterprise clients | Month 12 |
| S5 | Achieve subscription ARR | $2M ARR | Month 12 |
| S6 | Establish daily engagement | DAU/MAU ≥ 40% | Month 9 |

### 4.2 Operational Objectives

| # | Objective | Metric |
|---|---|---|
| O1 | Platform reliability | 99.9% uptime SLA |
| O2 | API performance | < 200ms P95 response time |
| O3 | Data compliance | NDPR + GDPR compliant from Day 1 |
| O4 | Coach verification | < 48 hours turnaround |
| O5 | User support response | < 4 hours average first response |
| O6 | AI response quality | > 4.2/5.0 user satisfaction rating |

### 4.3 Product Objectives

| # | Objective |
|---|---|
| P1 | Build a mood tracking system with scientifically validated instruments (PHQ-9, GAD-7 adaptations) |
| P2 | Build an AI emotional companion that passes a basic empathy Turing test |
| P3 | Design a gamification system that drives ≥ 5 daily touch points per user |
| P4 | Launch a community with robust AI moderation to prevent harmful content |
| P5 | Enable video, voice, and async text sessions with coaches |
| P6 | Build a white-label corporate wellness dashboard |

---

## 5. Business Goals

### 5.1 Revenue Goals

| Period | ARR Target | Revenue Streams |
|---|---|---|
| Year 1 | $2M | Subscriptions (60%), Commissions (25%), Corporate (15%) |
| Year 2 | $15M | Subscriptions (50%), Corporate (30%), Content (10%), AI (10%) |
| Year 3 | $50M | Subscriptions (40%), Corporate (35%), Marketplace (15%), Data (10%) |

### 5.2 User Growth Goals

| Period | MAU | Paying Users | Corporate Seats |
|---|---|---|---|
| Month 6 (MVP) | 5,000 | 500 | 0 |
| Month 12 | 50,000 | 8,000 | 2,000 |
| Month 24 | 500,000 | 80,000 | 25,000 |
| Month 36 | 5,000,000 | 750,000 | 250,000 |

### 5.3 Engagement Goals

| Metric | Year 1 Target | Year 3 Target |
|---|---|---|
| DAU/MAU Ratio | 40% | 55% |
| Day-7 Retention | 60% | 70% |
| Day-30 Retention | 40% | 50% |
| Avg Session Length | 8 minutes | 15 minutes |
| Sessions per Week (active user) | 4 | 6 |
| NPS Score | ≥ 60 | ≥ 75 |

---

## 6. Market Opportunity

### 6.1 Total Addressable Market

| Segment | Global TAM (2025) | Africa TAM (2025) | CAGR |
|---|---|---|---|
| Digital Mental Health Apps | $38B | $3.2B | 24% |
| Online Therapy & Coaching | $12B | $1.1B | 28% |
| Employee Wellness Programs | $61B | $4.8B | 18% |
| Meditation & Mindfulness | $9B | $0.8B | 22% |
| **Combined TAM** | **$120B** | **$9.9B** | **23%** |

### 6.2 Serviceable Addressable Market (SAM)

- Nigeria smartphone users with mental health awareness: ~18M
- Africa-wide reachable users Year 1: ~5M
- Corporate employees with EAP needs in Nigeria: ~2M

### 6.3 Competitive Landscape

| Competitor | Type | Strengths | Weaknesses |
|---|---|---|---|
| BetterHelp | Therapy marketplace | Scale, brand | USD pricing, no AI, no community |
| Calm | Mindfulness | Content quality | No therapy, no Africa focus |
| Headspace | Mindfulness | Clinical partnerships | No coaches, expensive |
| Talkspace | Therapy | Insurance integration | US-only, no gamification |
| Youper | AI mood tracking | AI-first design | No community, limited coaches |
| Wysa | AI wellness | Clinical validation | No video sessions |
| **Itura** | **Full ecosystem** | **Afrocentric, AI + Human, gamified, affordable, corporate-ready** | **New entrant, brand building needed** |

### 6.4 Itura's Unfair Advantages

1. **Cultural Intelligence:** First platform built ground-up for African users with local coaches, local languages, and local pricing
2. **Ecosystem Depth:** Only platform combining therapy + AI + journal + mood + community + corporate in one product
3. **Pricing Accessibility:** Naira/Cedi/Shilling pricing at 80% lower cost than Western competitors
4. **AI + Human Hybrid:** AI companion fills 24/7 gaps that human coaches cannot cover
5. **Corporate Wedge:** B2B enterprise contracts provide revenue stability and user acquisition pipeline

---

## 7. User Personas

### Persona 1 — Amara (The Anxious Professional)

| Attribute | Detail |
|---|---|
| Age | 28 |
| Gender | Female |
| Location | Lagos, Nigeria |
| Occupation | Marketing Manager, fintech startup |
| Income | ₦450,000/month |
| Digital Fluency | High |
| Mental Health Awareness | Moderate |
| **Goals** | Manage workplace anxiety, prevent burnout, maintain work-life balance |
| **Pain Points** | Cannot afford weekly therapy; stigma of colleagues knowing; no time for in-person appointments |
| **Platform Use** | Daily 5-min mood check-in, weekly journaling, monthly coach session |
| **Willingness to Pay** | ₦5,000–₦15,000/month |
| **Key Feature** | AI companion for daily emotional regulation + async messaging with coach |

---

### Persona 2 — Chidi (The Burned-Out Student)

| Attribute | Detail |
|---|---|
| Age | 21 |
| Gender | Male |
| Location | Ibadan, Nigeria |
| Occupation | 400-level Engineering student |
| Income | ₦30,000/month (allowance) |
| Digital Fluency | Very High |
| Mental Health Awareness | Low-Moderate |
| **Goals** | Manage academic stress, build confidence, find community |
| **Pain Points** | Cannot afford therapy at all; fear of judgment; university counselor unavailable |
| **Platform Use** | Gamified daily mood tracking, community forums, guided meditations |
| **Willingness to Pay** | ₦1,000–₦3,000/month |
| **Key Feature** | Free tier with gamification; peer community; AI companion |

---

### Persona 3 — Ngozi & Emeka (The Struggling Couple)

| Attribute | Detail |
|---|---|
| Ages | 32 (F) & 35 (M) |
| Location | Abuja, Nigeria |
| Occupations | Nurse & Civil Servant |
| Situation | 2 years married; communication breakdown; considering separation |
| **Goals** | Improve communication, rebuild emotional intimacy, understand each other better |
| **Pain Points** | Cultural taboo around couples therapy; cost; no Afrocentric couples therapists online |
| **Platform Use** | Couples dashboard, joint journaling, relationship wellness exercises, bi-weekly couples coach |
| **Willingness to Pay** | ₦20,000/month (couples plan) |
| **Key Feature** | Shared couples profile, relationship check-ins, Afrocentric couples coach matching |

---

### Persona 4 — Fatima (The Corporate HR Manager)

| Attribute | Detail |
|---|---|
| Age | 41 |
| Location | Lagos, Nigeria |
| Occupation | HR Director, 800-person manufacturing company |
| **Goals** | Reduce employee absenteeism from burnout, meet compliance requirements, improve team productivity |
| **Pain Points** | No budget-friendly EAP solution for Nigerian workforce; no data on employee wellbeing |
| **Platform Use** | Corporate dashboard, team wellness reports, bulk session credits, anonymous pulse surveys |
| **Budget** | ₦500,000–₦2,000,000/month corporate contract |
| **Key Feature** | Anonymous team wellness analytics, HR dashboard, bulk corporate subscriptions |

---

### Persona 5 — Tunde (The Midlife Crisis Executive)

| Attribute | Detail |
|---|---|
| Age | 48 |
| Location | Victoria Island, Lagos |
| Occupation | C-Suite Executive |
| **Goals** | Executive performance coaching, stress management, legacy planning, marriage counseling |
| **Pain Points** | Fear of vulnerability; privacy concerns; needs flexible scheduling; wants premium service |
| **Platform Use** | 1-on-1 premium executive coaching, confidential journaling, priority AI companion |
| **Willingness to Pay** | $100–$300/month |
| **Key Feature** | Premium white-glove tier, verified executive coaches, ironclad privacy guarantees |

---

### Persona 6 — Adaeze (The Grieving Parent)

| Attribute | Detail |
|---|---|
| Age | 52 |
| Location | Enugu, Nigeria |
| Situation | Recently lost a child; experiencing complicated grief |
| Digital Fluency | Low-Moderate |
| **Goals** | Process grief, find community of those who understand loss, access counseling |
| **Pain Points** | Needs culturally sensitive grief counseling; low digital literacy; rural broadband issues |
| **Platform Use** | Guided grief support content, AI companion for late-night emotional processing, grief support group |
| **Willingness to Pay** | ₦2,000–₦5,000/month |
| **Key Feature** | Simple UI, offline-capable tools, grief-specialized coach matching |

---

## 8. Stakeholders

### Internal Stakeholders

| Stakeholder | Role | Interest |
|---|---|---|
| CEO / Founder | Vision & Direction | Platform success, investor satisfaction, mission delivery |
| CTO / Engineering Lead | Technical Delivery | Architecture quality, team velocity, platform reliability |
| CPO / Product Manager | Product Definition | Feature quality, user satisfaction, roadmap execution |
| CFO | Financial Performance | Revenue growth, cost management, unit economics |
| Head of Marketing | Growth | User acquisition, brand awareness, conversion rates |
| Head of Partnerships | B2B | Corporate client acquisition, coach recruitment |

### External Stakeholders

| Stakeholder | Role | Interest |
|---|---|---|
| Users (Individual) | Primary Customers | Emotional support, privacy, value |
| Coaches & Therapists | Service Providers | Client acquisition, fair commission, scheduling tools |
| Corporate Clients | B2B Customers | Employee ROI, compliance, reporting |
| Investors | Funders | Revenue growth, market share, exit opportunity |
| NITDA / Regulators | Compliance | Data protection, healthcare standards compliance |
| Mental Health Associations | Validation Partners | Clinical validity, ethical AI use |
| Payment Processors | Infrastructure | Transaction success, fraud prevention |

---

## 9. Functional Requirements

### 9.1 Authentication & Identity Management

| ID | Requirement | Priority |
|---|---|---|
| FR-A01 | Email/password registration with email verification | Critical |
| FR-A02 | OAuth 2.0 social login (Google, Apple) | High |
| FR-A03 | Phone number OTP verification | High |
| FR-A04 | JWT + Refresh token authentication | Critical |
| FR-A05 | Multi-factor authentication (MFA) | High |
| FR-A06 | Biometric authentication (mobile) | Medium |
| FR-A07 | Session management and concurrent session limits | High |
| FR-A08 | Password reset via email/SMS | Critical |
| FR-A09 | Role-based access control (User, Coach, Admin, Corporate) | Critical |

### 9.2 User Profile & Onboarding

| ID | Requirement | Priority |
|---|---|---|
| FR-U01 | Personalized onboarding questionnaire (wellness goals, concerns, preferences) | Critical |
| FR-U02 | User profile with avatar, bio, wellness goals | High |
| FR-U03 | Wellness assessment (PHQ-9, GAD-7 adaptations) during onboarding | High |
| FR-U04 | Privacy settings (data sharing controls, profile visibility) | Critical |
| FR-U05 | Language preference (English, Pidgin, Yoruba, Igbo, Hausa) | Medium |
| FR-U06 | Notification preference management | High |
| FR-U07 | Account deletion with data export | Critical (GDPR) |

### 9.3 Coach & Therapist Module

| ID | Requirement | Priority |
|---|---|---|
| FR-C01 | Coach profile creation (credentials, specialties, experience, bio, photo) | Critical |
| FR-C02 | Professional credential verification workflow | Critical |
| FR-C03 | Coach calendar and availability management | Critical |
| FR-C04 | Coach rating and review system | High |
| FR-C05 | Coach search with filters (specialty, language, price, availability, gender) | Critical |
| FR-C06 | Coach earnings dashboard and payout management | High |
| FR-C07 | Coach-client messaging (async) | Critical |
| FR-C08 | Coach session notes (private, visible only to coach) | High |
| FR-C09 | Coach performance analytics | Medium |

### 9.4 Booking & Sessions Module

| ID | Requirement | Priority |
|---|---|---|
| FR-B01 | Session booking flow (coach selection → time slot → payment → confirmation) | Critical |
| FR-B02 | Video session (WebRTC/Agora/Daily.co integration) | Critical |
| FR-B03 | Voice-only session option | High |
| FR-B04 | Async text messaging sessions | High |
| FR-B05 | Group session support (up to 20 participants) | Medium |
| FR-B06 | Session reminders (email, push, SMS) | High |
| FR-B07 | Session rescheduling and cancellation with refund policy | High |
| FR-B08 | Session recording (with user consent) | Medium |
| FR-B09 | Post-session feedback and rating | High |
| FR-B10 | Waitlist for fully-booked coaches | Medium |

### 9.5 AI Emotional Companion

| ID | Requirement | Priority |
|---|---|---|
| FR-AI01 | 24/7 conversational AI companion (text-based) | Critical |
| FR-AI02 | Sentiment analysis of user input and adaptive responses | Critical |
| FR-AI03 | Crisis detection and escalation to human support | Critical (Safety) |
| FR-AI04 | AI-generated journaling prompts based on mood | High |
| FR-AI05 | Personalized wellness recommendations | High |
| FR-AI06 | Conversation history and continuity across sessions | High |
| FR-AI07 | AI personality customization (tone: clinical, friendly, spiritual) | Medium |
| FR-AI08 | AI safety guardrails (no harmful advice, suicide protocol) | Critical (Safety) |

### 9.6 Mood Tracking

| ID | Requirement | Priority |
|---|---|---|
| FR-M01 | Daily mood check-in (1-click, emoji-based + optional note) | Critical |
| FR-M02 | Mood history visualization (daily, weekly, monthly charts) | High |
| FR-M03 | Mood trigger tagging (work, relationships, sleep, etc.) | High |
| FR-M04 | Mood-based insights and pattern recognition | High |
| FR-M05 | PHQ-9 / GAD-7 adapted periodic assessments | Medium |
| FR-M06 | Mood streaks and consistency rewards | High |

### 9.7 Journaling

| ID | Requirement | Priority |
|---|---|---|
| FR-J01 | Rich text journal editor | Critical |
| FR-J02 | AI-assisted journaling prompts | High |
| FR-J03 | Emotion tagging on journal entries | High |
| FR-J04 | Journal entry search and filtering | Medium |
| FR-J05 | Journal templates (gratitude, CBT thought records, daily reflection) | High |
| FR-J06 | Private journal (encrypted, coach cannot read without permission) | Critical (Privacy) |
| FR-J07 | Journal sharing with coach (optional, user-controlled) | High |
| FR-J08 | Offline journal writing with sync | Medium |

### 9.8 Community Module

| ID | Requirement | Priority |
|---|---|---|
| FR-CM01 | Topic-based community forums (anxiety, grief, relationships, etc.) | High |
| FR-CM02 | Anonymous post option | Critical (Privacy) |
| FR-CM03 | Post reactions and threaded replies | High |
| FR-CM04 | Community moderation (AI + human) | Critical (Safety) |
| FR-CM05 | Peer support groups (invite-only or open) | High |
| FR-CM06 | Content reporting and appeals process | High |
| FR-CM07 | Community challenges (30-day wellness challenge) | Medium |
| FR-CM08 | Coach-led community Q&A sessions | Medium |

### 9.9 Payments & Wallet

| ID | Requirement | Priority |
|---|---|---|
| FR-P01 | Subscription billing (monthly/annual) via Paystack + Stripe | Critical |
| FR-P02 | Session payment (pay-per-session) | Critical |
| FR-P03 | In-app wallet with top-up and session credit system | High |
| FR-P04 | Refund processing (automated for cancellations) | High |
| FR-P05 | Corporate bulk billing and invoicing | High |
| FR-P06 | Coach payout to bank account / mobile money | High |
| FR-P07 | Transaction history and receipts | High |
| FR-P08 | Currency support (NGN, GHS, KES, USD, GBP) | High |
| FR-P09 | Promo codes and discount management | Medium |

### 9.10 Notifications

| ID | Requirement | Priority |
|---|---|---|
| FR-N01 | Push notifications (iOS + Android) | Critical |
| FR-N02 | Email notifications (transactional + marketing) | Critical |
| FR-N03 | SMS notifications (session reminders, OTP) | High |
| FR-N04 | In-app notification center | High |
| FR-N05 | Notification preference management | High |
| FR-N06 | Do-not-disturb and quiet hours | Medium |

### 9.11 Corporate Wellness Module

| ID | Requirement | Priority |
|---|---|---|
| FR-CW01 | Corporate account creation and team management | High |
| FR-CW02 | Anonymous employee wellness dashboard | High |
| FR-CW03 | Bulk session credit allocation | High |
| FR-CW04 | Employee onboarding via company email domain | High |
| FR-CW05 | Team wellness pulse surveys | Medium |
| FR-CW06 | Burnout risk scoring (aggregate, anonymous) | Medium |
| FR-CW07 | HR-facing analytics and reporting | High |
| FR-CW08 | EAP integration and referral workflows | Medium |

### 9.12 Admin Module

| ID | Requirement | Priority |
|---|---|---|
| FR-AD01 | Super admin dashboard | Critical |
| FR-AD02 | User management (view, suspend, delete, restore) | Critical |
| FR-AD03 | Coach verification and approval workflow | Critical |
| FR-AD04 | Content moderation queue | Critical |
| FR-AD05 | Payment reconciliation and payout management | High |
| FR-AD06 | Platform analytics (users, revenue, engagement) | High |
| FR-AD07 | Subscription and plan management | High |
| FR-AD08 | System health monitoring | High |
| FR-AD09 | Audit log viewer | High |

---

## 10. Non-Functional Requirements

### 10.1 Performance

| Requirement | Target |
|---|---|
| API response time (P50) | < 100ms |
| API response time (P95) | < 200ms |
| API response time (P99) | < 500ms |
| Page load time (web, LCP) | < 2.5 seconds |
| App cold start (mobile) | < 3 seconds |
| Concurrent video sessions | 10,000+ |
| Database query time (P95) | < 50ms |

### 10.2 Scalability

| Requirement | Target |
|---|---|
| Registered users | 10M+ |
| Concurrent users | 500,000+ |
| Messages per second | 50,000+ |
| Horizontal scale | Auto-scaling via Kubernetes HPA |
| Session booking throughput | 10,000 bookings/hour |

### 10.3 Availability & Reliability

| Requirement | Target |
|---|---|
| Platform uptime SLA | 99.9% (< 8.7 hours/year downtime) |
| RTO (Recovery Time Objective) | < 1 hour |
| RPO (Recovery Point Objective) | < 15 minutes |
| Database replication | Synchronous multi-region |
| Backup frequency | Hourly incremental, daily full |

### 10.4 Security

| Requirement | Standard |
|---|---|
| Data encryption at rest | AES-256 |
| Data encryption in transit | TLS 1.3 |
| Authentication | OAuth 2.0 + JWT with RS256 |
| PII storage | Encrypted, access-controlled, audit-logged |
| Penetration testing | Quarterly |
| Dependency scanning | Daily automated scans |

### 10.5 Usability

| Requirement | Target |
|---|---|
| WCAG Accessibility | 2.1 Level AA |
| Supported languages | English (launch), + 4 local languages (Year 2) |
| Minimum supported iOS | iOS 14+ |
| Minimum supported Android | Android 8.0+ |
| Minimum supported browsers | Chrome 90+, Firefox 88+, Safari 14+, Edge 90+ |
| Offline support | Mood tracking, journaling (mobile) |

---

## 11. User Journeys

### Journey 1 — First-Time User (Individual)

```
Discovery (Social/Referral/Organic)
    ↓
Landing Page → "Start Your Wellness Journey" CTA
    ↓
Registration (Email or Google OAuth)
    ↓
Onboarding Questionnaire (5 questions: goals, concerns, preferences)
    ↓
Wellness Assessment (adapted PHQ-9 / GAD-7, 3 min)
    ↓
Personalized Dashboard Generated
    ↓
Daily Mood Check-In (First time)
    ↓
AI Companion Introduction ("Hi, I'm Sera — your wellness companion")
    ↓
Coach Recommendation (based on onboarding answers)
    ↓
Book First Session (or explore free tier)
    ↓
Return Next Day (streak started, nudge notification)
```

### Journey 2 — Coach/Therapist Onboarding

```
Apply as Coach (form submission)
    ↓
Document Upload (license, certificates, ID)
    ↓
Admin Review & Verification (< 48 hours)
    ↓
Video Interview (optional, for premium coach status)
    ↓
Profile Creation & Preview
    ↓
Calendar Setup (availability blocks)
    ↓
Pricing Configuration (per session rate)
    ↓
Go Live (profile visible to users)
    ↓
First Booking Received → Notification
    ↓
Session Conducted → Rating Received
    ↓
Payout Processed (weekly)
```

### Journey 3 — Corporate Wellness Setup

```
HR Contact Discovery (B2B outreach or inbound)
    ↓
Demo & Proposal
    ↓
Contract Signed (monthly/annual)
    ↓
Corporate Account Created
    ↓
Employee Invitation Sent (bulk email)
    ↓
Employees Register with Company Email Domain
    ↓
HR Dashboard: Anonymous Wellness Overview
    ↓
Monthly Wellness Report Generated
    ↓
Session Credits Allocated to Employees
    ↓
Renewal / Upsell Discussion
```

---

## 12. User Stories

### Authentication

| ID | Story | Priority |
|---|---|---|
| US-A01 | As a new user, I want to register with my email and password so that I can create an account | Critical |
| US-A02 | As a user, I want to sign in with Google so I don't have to remember a password | High |
| US-A03 | As a user, I want to reset my password via email if I forget it | Critical |
| US-A04 | As a user, I want to enable 2FA so my account is more secure | High |
| US-A05 | As an admin, I want to force password reset for compromised accounts | High |

### Mood Tracking

| ID | Story | Priority |
|---|---|---|
| US-MT01 | As a user, I want to log my mood in < 30 seconds each day so it doesn't feel like a burden | Critical |
| US-MT02 | As a user, I want to see my mood history in a chart so I can identify patterns | High |
| US-MT03 | As a user, I want to tag what triggered my mood so I can understand my emotional patterns | High |
| US-MT04 | As a user, I want to receive a gentle nudge if I haven't logged my mood by 8pm | Medium |
| US-MT05 | As a user, I want to see a streak counter so I feel motivated to log daily | High |

### AI Companion

| ID | Story | Priority |
|---|---|---|
| US-AI01 | As a user, I want to talk to an AI companion at 2am when I'm anxious so I don't feel alone | Critical |
| US-AI02 | As a user, I want the AI to remember our previous conversations so I don't repeat myself | High |
| US-AI03 | As a user experiencing a crisis, I want the AI to immediately provide emergency resources | Critical |
| US-AI04 | As a user, I want to give the AI a name/persona so it feels more personal | Medium |
| US-AI05 | As a user, I want the AI to suggest journaling prompts based on how I'm feeling | High |

### Booking

| ID | Story | Priority |
|---|---|---|
| US-BK01 | As a user, I want to search for coaches by specialty so I find the right match | Critical |
| US-BK02 | As a user, I want to book a session in < 3 minutes so the process doesn't feel overwhelming | Critical |
| US-BK03 | As a user, I want to reschedule a session up to 24 hours before without penalty | High |
| US-BK04 | As a coach, I want to set my availability so I only receive bookings when I'm free | Critical |
| US-BK05 | As a user, I want to rate and review my coach after each session | High |

---

## 13. Acceptance Criteria

### AC-MT01 — Daily Mood Check-In

**Given** a logged-in user opens the app  
**When** they navigate to the mood tracker  
**Then** they should see 5 mood options (emoji-based: Very Sad, Sad, Neutral, Happy, Very Happy)  
**And** optional text note field (max 280 characters)  
**And** optional trigger tag multi-select  
**And** the check-in should complete in 3 taps or fewer  
**And** the streak counter should increment  
**And** the entry should be persisted within 2 seconds  

### AC-AI01 — AI Companion Response

**Given** a user sends a message to the AI companion  
**When** the message is processed  
**Then** a response should appear within 3 seconds  
**And** the response should acknowledge the user's emotional state  
**And** if crisis keywords are detected, the response MUST include the crisis resource message  
**And** the response should NOT include any medical diagnoses  
**And** the conversation should be saved to history  

### AC-BK01 — Session Booking

**Given** a user selects a coach and time slot  
**When** they complete payment  
**Then** a booking confirmation should appear within 5 seconds  
**And** a confirmation email should be sent within 60 seconds  
**And** a calendar invite should be generated  
**And** the coach should receive a notification  
**And** the time slot should be removed from availability immediately  

---

## 14. Monetization Strategy

### 14.1 Revenue Streams

#### Stream 1 — Individual Subscriptions (B2C)

| Tier | Monthly Price (NGN) | Monthly Price (USD) | Annual Discount |
|---|---|---|---|
| **Free** | ₦0 | $0 | — |
| **Pro** | ₦5,000 | $5 | 20% (₦48,000/yr) |
| **Premium** | ₦15,000 | $15 | 25% (₦135,000/yr) |
| **Executive** | ₦50,000 | $50 | 15% (₦510,000/yr) |

**Free Tier Includes:**
- Daily mood check-in (limited history: 7 days)
- 3 journal entries per week
- Community read-only access
- Basic AI companion (5 messages/day)
- 1 free session credit at registration

**Pro Tier Adds:**
- Unlimited mood history and charts
- Unlimited journaling
- Full community access (posting + groups)
- AI companion: 50 messages/day
- 1 free session credit/month
- Wellness content library

**Premium Tier Adds:**
- Unlimited AI companion
- Group wellness sessions (2/month)
- Couples features
- Priority coach matching
- 2 free session credits/month
- Advanced analytics on mood trends

**Executive Tier Adds:**
- Dedicated wellness concierge
- Executive coach matching
- 4 free session credits/month
- Ironclad privacy mode
- API access (integrations)

---

#### Stream 2 — Session Revenue (Marketplace Commission)

| Session Type | Coach Earns | Itura Commission |
|---|---|---|
| Standard session (50 min) | 80% | 20% |
| Group session (per participant) | 75% | 25% |
| Premium/Executive session | 85% | 15% |

Coach pricing typically ₦5,000–₦50,000 per session depending on credentials and specialty.

---

#### Stream 3 — Corporate Wellness (B2B)

| Package | Price | Included |
|---|---|---|
| **Starter** | ₦500,000/month | Up to 100 employees, basic dashboard |
| **Growth** | ₦1,500,000/month | Up to 500 employees, analytics, 20 sessions |
| **Enterprise** | Custom | Unlimited employees, full EAP, custom integrations |

---

#### Stream 4 — AI Companion Add-On

Standalone subscription for users who only want the AI companion:  
- ₦3,000/month for unlimited AI conversations
- Targets users on free tier who want more AI without full upgrade

---

#### Stream 5 — Content Marketplace

- Premium wellness courses: ₦2,000–₦10,000 one-time purchase
- Coach-led workshops: ₦3,000–₦15,000 per ticket
- Corporate wellness webinars: ₦500,000+ per event

---

### 14.2 Unit Economics (Year 1 Projections)

| Metric | Value |
|---|---|
| Average Revenue Per User (ARPU) | $8/month |
| Customer Acquisition Cost (CAC) | $12 |
| LTV (12-month) | $96 |
| LTV:CAC Ratio | 8:1 |
| Gross Margin (subscriptions) | 82% |
| Gross Margin (sessions) | 20% (commission only) |
| Blended Gross Margin | ~65% |

---

## 15. KPIs

### Growth KPIs

| KPI | Definition | Target (Y1) |
|---|---|---|
| MAU | Monthly Active Users | 50,000 |
| DAU | Daily Active Users | 20,000 |
| DAU/MAU | Engagement ratio | 40% |
| New Registrations | Per month | 8,000 |
| Paid Conversion Rate | Free → Paid | 16% |
| Corporate Clients | Enterprise accounts | 10 |

### Retention KPIs

| KPI | Target (Y1) |
|---|---|
| Day-1 Retention | 70% |
| Day-7 Retention | 55% |
| Day-30 Retention | 40% |
| Day-90 Retention | 30% |
| Subscription Churn | < 5%/month |
| Coach Churn | < 3%/month |

### Engagement KPIs

| KPI | Target (Y1) |
|---|---|
| Avg Session Length | 8 minutes |
| Sessions per Week (active) | 4 |
| Mood Check-In Streak (avg) | 7 days |
| Journal Entries per User/Month | 8 |
| AI Companion Conversations/Day | 15,000 |
| Community Posts per Day | 500 |

### Business KPIs

| KPI | Target (Y1) |
|---|---|
| ARR | $2M |
| MRR Growth Rate | 15%/month |
| Gross Margin | 65% |
| NPS | ≥ 60 |
| App Store Rating | ≥ 4.5 |

---

## 16. Compliance Requirements

### 16.1 NDPR (Nigeria Data Protection Regulation)

| Requirement | Implementation |
|---|---|
| Lawful basis for processing | Explicit user consent at registration |
| Data subject rights | Access, correction, deletion, portability via Settings |
| Data minimization | Collect only what is necessary for service delivery |
| Data retention limits | User data deleted 90 days after account deletion |
| Breach notification | NITDA notified within 72 hours of confirmed breach |
| DPO appointment | Designated Data Protection Officer from Day 1 |

### 16.2 GDPR (EU Users)

| Requirement | Implementation |
|---|---|
| Consent management | Cookie banner, granular consent controls |
| Right to be forgotten | Automated deletion pipeline |
| Data portability | JSON export of all user data |
| Privacy by design | Architecture-level data protection |
| Cross-border transfer | Standard Contractual Clauses (SCCs) |

### 16.3 HIPAA-Aligned Health Data Controls

| Control | Implementation |
|---|---|
| PHI encryption | AES-256 at rest, TLS 1.3 in transit |
| Access controls | Role-based, least-privilege access |
| Audit trails | All PHI access logged with user ID, timestamp, action |
| Business Associate Agreements | Required for all health data sub-processors |
| Minimum necessary | Users, coaches only see data needed for care |

### 16.4 PCI DSS (Payment Data)

| Control | Implementation |
|---|---|
| No card storage | Tokenization via Paystack/Stripe |
| Secure payment flows | Redirect to payment processor; no card data on platform |
| HTTPS enforcement | All payment pages TLS 1.3 |
| Fraud detection | Paystack radar + custom rules |

---

## 17. Risk Assessment

### Risk Matrix

| Risk | Likelihood | Impact | Severity | Mitigation |
|---|---|---|---|---|
| Data breach / PII exposure | Medium | Critical | **HIGH** | Encryption, pen testing, access controls |
| AI companion gives harmful advice | Low | Critical | **HIGH** | Safety guardrails, crisis protocols, human escalation |
| Coach misconduct | Medium | High | **HIGH** | Background checks, user reporting, suspension workflow |
| Platform downtime during peak | Medium | High | **HIGH** | Multi-AZ deployment, circuit breakers, autoscaling |
| Coach supply shortage | High | High | **HIGH** | Early recruitment campaign, coach referral program |
| Regulatory non-compliance | Low | Critical | **HIGH** | Legal counsel, DPO, compliance audits |
| User payment fraud | Medium | Medium | **MEDIUM** | Paystack Radar, velocity checks, manual review |
| Low user retention | High | High | **HIGH** | Gamification, engagement loops, personalization |
| Corporate client churn | Medium | High | **HIGH** | Quarterly business reviews, ROI reporting |
| Competition from global players | Medium | High | **HIGH** | Local moat: culture, price, community |
| Poor AI quality | Medium | High | **HIGH** | Model selection, fine-tuning, human feedback loop |
| Negative press (mental health incident) | Low | Critical | **HIGH** | Crisis communication plan, clinical advisory board |

---

## 18. Future Expansion Plans

### Phase 1 — MVP (Month 1–6)
Core platform: Authentication, mood tracking, journaling, AI companion, basic coaching marketplace, subscription billing

### Phase 2 — Growth (Month 7–18)
Corporate wellness, group sessions, community features, gamification, advanced analytics, mobile app polish

### Phase 3 — Expansion (Month 19–36)
West Africa expansion (Ghana, Kenya, South Africa), additional local languages, white-label offering, couples/family features, content marketplace

### Phase 4 — Scale (Year 3+)
Pan-Africa rollout, API platform (developer ecosystem), clinical partnerships (insurance integrations), IPO preparation

### Geographic Expansion Roadmap

| Phase | Markets | Languages |
|---|---|---|
| Launch | Nigeria | English, Pidgin |
| Phase 2 | Ghana, Kenya | English |
| Phase 3 | South Africa, Rwanda | English, Swahili |
| Phase 4 | Francophone Africa | French |
| Phase 5 | Global emerging markets | Arabic, Portuguese |

### Product Expansion

| Feature | Timeline |
|---|---|
| Couples therapy module | Month 10 |
| Family wellness dashboard | Month 14 |
| Psychiatry referral network | Month 16 |
| Wellness content marketplace | Month 12 |
| White-label corporate platform | Month 18 |
| AI voice companion | Month 20 |
| Wearable integration (Fitbit, Apple Watch) | Month 24 |
| Clinical trial partnerships | Month 30 |

---

*End of Business Requirements Document*  
*Next: [PRD.md](./PRD.md)*
