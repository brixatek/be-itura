# ITURA — Mental Wellness & Emotional Wellbeing Platform

> *"Itura" — Yoruba for comfort, rest, and peace of mind.*

**Version:** 1.0.0  
**Status:** Pre-Production / Architecture Phase  
**Last Updated:** May 2026  
**Classification:** Confidential — Internal Engineering Use

---

## What Is Itura?

Itura is a next-generation, AI-powered Mental Wellness and Emotional Wellbeing Platform built as a daily emotional operating system for individuals, couples, families, students, professionals, and corporate teams across Africa and globally.

It is **not** a simple therapy booking app.

It is a full emotional wellness ecosystem that combines:

| Pillar | Features |
|---|---|
| Professional Care | Therapy booking, licensed coaching, psychiatry referrals |
| AI Companionship | 24/7 AI emotional companion, sentiment-aware conversations |
| Daily Wellness | Mood tracking, journaling, breathing exercises, meditations |
| Community | Peer support forums, group sessions, anonymous sharing |
| Relationship Wellness | Couples therapy, family coaching, relationship check-ins |
| Corporate Wellness | Team dashboards, EAP programs, burnout analytics |
| Gamification | Streaks, badges, wellness challenges, leaderboards |
| Content | Curated wellness articles, guided sessions, podcasts |
| Subscriptions | Freemium, Pro, Premium, Corporate tiers |

---

## Mission

> To empower every person to build emotional resilience, find professional support when needed, and belong to a community that understands their mental health journey — through technology that feels human, intelligent, and deeply caring.

## Vision

> To become the most trusted daily emotional wellness companion for 100 million people across Africa and emerging markets by 2030.

---

## Documentation Index

| Document | Description | Audience |
|---|---|---|
| [BRD.md](./BRD.md) | Business Requirements Document | Executives, Product, Business |
| [PRD.md](./PRD.md) | Product Requirements Document | Product, Design, Engineering |
| [ARCHITECTURE.md](./ARCHITECTURE.md) | System Architecture | Engineering Leads, DevOps |
| [TECH_STACK.md](./TECH_STACK.md) | Tech Stack Recommendations | Engineering |
| [DATABASE.md](./DATABASE.md) | Database Design & Schema | Backend Engineers, DBAs |
| [BACKEND_TASKS.md](./BACKEND_TASKS.md) | Backend Engineering Task Breakdown | Backend Engineers |
| [FRONTEND_TASKS.md](./FRONTEND_TASKS.md) | Frontend Engineering Task Breakdown | Frontend Engineers |
| [MOBILE_TASKS.md](./MOBILE_TASKS.md) | Flutter Mobile App Tasks | Mobile Engineers |
| [API_DESIGN.md](./API_DESIGN.md) | REST API Design & Standards | Full-Stack Engineers |
| [DEVOPS.md](./DEVOPS.md) | DevOps, CI/CD & Infrastructure | DevOps / Platform Engineers |
| [SECURITY.md](./SECURITY.md) | Security Architecture & Compliance | Security, Engineering |
| [AI_ML.md](./AI_ML.md) | AI & Machine Learning Architecture | AI/ML Engineers |
| [PROJECT_MANAGEMENT.md](./PROJECT_MANAGEMENT.md) | Agile Roadmap & Team Structure | PMs, Engineering Leads |
| [SCALABILITY.md](./SCALABILITY.md) | Scalability & Growth Strategy | Engineering, Leadership |
| [UI_UX.md](./UI_UX.md) | UI/UX Design Strategy | Designers, Product |

---

## Target Audience

| Segment | Primary Needs |
|---|---|
| Individuals with anxiety | Daily coping tools, AI companion, therapy access |
| Couples | Relationship wellness, couples coaching |
| Students | Affordable access, peer community, academic stress tools |
| Professionals | Burnout prevention, executive coaching, quick tools |
| Families | Family sessions, parenting support |
| Youths (13–24) | Safe space, peer support, gamified wellness |
| Corporate employees | EAP, team wellbeing, burnout monitoring |
| Wellness seekers | Content, journaling, meditation, growth tools |

---

## High-Level Architecture Overview

```
┌──────────────────────────────────────────────────────────────┐
│                        CLIENTS                               │
│   Web (Next.js)  │  Mobile (Flutter)  │  API Consumers       │
└──────────────────┬───────────────────┬──────────────────────┘
                   │                   │
         ┌─────────▼───────────────────▼─────────┐
         │              API GATEWAY               │
         │   (Rate Limiting · Auth · Routing)     │
         └─────────────────┬─────────────────────┘
                           │
         ┌─────────────────▼─────────────────────┐
         │          MICROSERVICES LAYER           │
         │  Auth · Users · Coaches · Booking      │
         │  Payments · AI · Journal · Community   │
         │  Notifications · Analytics · Admin     │
         └─────────────────┬─────────────────────┘
                           │
         ┌─────────────────▼─────────────────────┐
         │          DATA & MESSAGING LAYER        │
         │  PostgreSQL · Redis · MongoDB          │
         │  RabbitMQ · SignalR · Blob Storage     │
         └────────────────────────────────────────┘
```

---

## MVP Scope (Sprint 1–8)

- [x] User registration and authentication
- [x] Coach/Therapist profiles and discovery
- [x] Session booking (video + async messaging)
- [x] AI emotional companion (text-based)
- [x] Mood tracker (daily check-in)
- [x] Personal journal
- [x] Basic community feed
- [x] Subscription billing (Paystack + Stripe)
- [x] Push notifications
- [x] Admin dashboard

---

## Team Structure

| Role | Count |
|---|---|
| Product Manager | 1 |
| Engineering Lead / Architect | 1 |
| Backend Engineers (.NET) | 3 |
| Frontend Engineers (Next.js) | 2 |
| Mobile Engineers (Flutter) | 2 |
| AI/ML Engineer | 1 |
| DevOps Engineer | 1 |
| UI/UX Designer | 1 |
| QA Engineer | 1 |

---

## Repository Structure

```
itura/
├── src/
│   ├── Itura.API/                  # ASP.NET Core API Gateway
│   ├── Itura.Services/
│   │   ├── Auth/
│   │   ├── Users/
│   │   ├── Coaches/
│   │   ├── Booking/
│   │   ├── Payments/
│   │   ├── AI/
│   │   ├── Journal/
│   │   ├── Community/
│   │   ├── Notifications/
│   │   └── Analytics/
│   ├── Itura.Web/                  # Next.js frontend
│   └── Itura.Mobile/               # Flutter app
├── infrastructure/
│   ├── terraform/
│   ├── kubernetes/
│   └── docker/
├── docs/                           # This documentation
├── scripts/
└── tests/
```

---

## Monetization Summary

| Tier | Price | Features |
|---|---|---|
| Free | ₦0 / $0 | Mood tracking, 3 journal entries/week, community read-only |
| Pro | ₦5,000 / $5/month | Full journal, AI companion, community posting, 1 free session credit |
| Premium | ₦15,000 / $15/month | Unlimited AI, group sessions, premium content, 2 free session credits |
| Corporate | Custom | Team dashboards, EAP, bulk session credits, analytics |

---

## Key Performance Indicators

| KPI | Year 1 Target |
|---|---|
| Monthly Active Users | 50,000 |
| DAU/MAU Ratio | ≥ 40% |
| Day-30 Retention | ≥ 40% |
| Average Session Length | ≥ 8 minutes |
| NPS Score | ≥ 60 |
| ARR | $2M |
| Verified Coaches | 500 |
| Platform Uptime | 99.9% |

---

## Tech Stack Summary

| Layer | Technology |
|---|---|
| Backend | .NET 8 / ASP.NET Core / C# |
| Real-Time | SignalR |
| Inter-Service | gRPC + MassTransit + RabbitMQ |
| Database | PostgreSQL (primary), MongoDB (unstructured), Redis (cache) |
| Frontend | Next.js 14 + TypeScript + Tailwind CSS |
| Mobile | Flutter 3.x |
| AI | OpenAI GPT-4o + Azure AI Services |
| Infrastructure | Docker + Kubernetes + Azure |
| Payments | Paystack (Africa) + Stripe (Global) |
| Video | Daily.co / Agora |

---

## Compliance

- NDPR (Nigeria Data Protection Regulation)
- GDPR (General Data Protection Regulation)
- HIPAA-aligned data handling for health information
- ISO 27001-aligned security controls
- PCIDSS for payment processing

---

*Built with love for Africa's mental health future.*  
*© 2026 Itura Technologies Ltd. All rights reserved.*
