# ITURA — Project Management

**Document Version:** 1.0  
**Owner:** Product Manager / Engineering Lead  
**Last Updated:** May 2026

---

## 1. Team Structure

### Core Engineering Team

| Role | Count | Responsibilities |
|---|---|---|
| Product Manager | 1 | Roadmap, prioritization, stakeholder communication |
| Engineering Lead / Architect | 1 | Technical decisions, architecture, code review |
| Backend Engineers (.NET) | 3 | Microservices, APIs, business logic |
| Frontend Engineers (Next.js) | 2 | Web app, admin portal |
| Mobile Engineers (Flutter) | 2 | iOS & Android apps |
| AI/ML Engineer | 1 | AI companion, sentiment, moderation |
| DevOps Engineer | 1 | CI/CD, Kubernetes, monitoring |
| UI/UX Designer | 1 | Design system, wireframes, prototypes |
| QA Engineer | 1 | Test plans, automated testing |
| **Total** | **13** | |

### Extended Team

| Role | Type | When Engaged |
|---|---|---|
| Clinical Advisory Board (2 psychologists) | Part-time | AI safety reviews, content strategy |
| Legal Counsel (NDPR/GDPR) | Retainer | Data privacy, compliance reviews |
| Security Firm | Contract | Quarterly pen tests |
| Data Protection Officer (DPO) | Part-time | Compliance oversight |

---

## 2. Agile Process

### Sprint Structure
- **Sprint Length:** 2 weeks
- **Sprint Start:** Monday
- **Sprint Review & Retrospective:** Last Friday of sprint
- **Planning:** First Monday of sprint
- **Daily Standup:** 9:00 AM WAT, 15 minutes

### Ceremonies

| Ceremony | Frequency | Duration | Participants |
|---|---|---|---|
| Sprint Planning | Every 2 weeks | 2 hours | Full team |
| Daily Standup | Daily | 15 minutes | Engineering |
| Sprint Review | Every 2 weeks | 1 hour | Full team + stakeholders |
| Sprint Retrospective | Every 2 weeks | 1 hour | Full team |
| Product Backlog Grooming | Weekly | 1 hour | PM + Engineering Lead |
| Architecture Review | Bi-weekly | 1 hour | Engineering Lead + Architects |

### Definition of Done

A story is Done when:
- [ ] Code written and reviewed (min. 1 reviewer)
- [ ] Unit tests written (≥ 80% coverage for new code)
- [ ] Integration tests pass
- [ ] CI pipeline passes (build + tests + SAST)
- [ ] API documented in OpenAPI spec
- [ ] Deployed to staging and smoke-tested
- [ ] Acceptance criteria verified by PM or QA
- [ ] No critical/high security vulnerabilities introduced

---

## 3. Agile Roadmap

### Phase 1 — Foundation (Months 1–2 | Sprints 1–4)

**Goal:** Core platform infrastructure and authentication

| Sprint | Focus | Key Deliverables |
|---|---|---|
| Sprint 1 | Infrastructure + Auth | Docker setup, CI/CD, database schemas, registration, login, JWT |
| Sprint 2 | User Module | Onboarding wizard, profile CRUD, preferences, wellness assessment |
| Sprint 3 | AI Companion MVP | Sera chat interface, GPT-4o integration, crisis detection |
| Sprint 4 | Mood Tracker | Daily check-in, mood history, streak system |

---

### Phase 2 — Core Product (Months 3–4 | Sprints 5–8)

**Goal:** Coaching marketplace, payments, and journaling

| Sprint | Focus | Key Deliverables |
|---|---|---|
| Sprint 5 | Coach Module | Coach profiles, search, verification workflow |
| Sprint 6 | Booking System | Availability, booking flow, Paystack integration, video sessions (Agora) |
| Sprint 7 | Journal Module | Journal editor, templates, encryption, AI prompts |
| Sprint 8 | Subscriptions + Admin | Subscription billing, admin dashboard, coach payouts |

**MVP Launch → End of Sprint 8**

---

### Phase 3 — Growth Features (Months 5–8 | Sprints 9–16)

**Goal:** Engagement, retention, and community

| Sprint | Focus | Key Deliverables |
|---|---|---|
| Sprint 9 | Gamification | XP system, badges, wellness levels, leaderboard |
| Sprint 10 | Community MVP | Topic forums, anonymous posts, reactions, basic moderation |
| Sprint 11 | Community Advanced | Groups, challenges, AI moderation pipeline |
| Sprint 12 | Notifications | Push, email, SMS, in-app notification center |
| Sprint 13 | Corporate Wellness | Corporate accounts, team dashboard, bulk sessions |
| Sprint 14 | Mobile App Polish | Offline support, biometric auth, performance optimization |
| Sprint 15 | AI Improvements | Sentiment pipeline, mood insights, coach recommendations |
| Sprint 16 | Async Messaging | Coach-client async messaging between sessions |

---

### Phase 4 — Expansion (Months 9–12 | Sprints 17–24)

**Goal:** Advanced features and market expansion

| Sprint | Focus |
|---|---|
| Sprint 17–18 | Couples wellness module |
| Sprint 19–20 | Content library (articles, guided meditations, podcasts) |
| Sprint 21–22 | Advanced analytics + AI insights |
| Sprint 23 | Ghana/Kenya market launch prep (local currencies, local coaches) |
| Sprint 24 | White-label corporate offering |

---

## 4. Milestones

| Milestone | Target Date | Success Criteria |
|---|---|---|
| M1: Infrastructure Ready | Week 2 | All services running locally; CI/CD pipeline active |
| M2: Auth Complete | Week 4 | Users can register, log in, verify email, OAuth |
| M3: AI Companion Live | Week 6 | Sera conversations with crisis detection working |
| M4: Coaching Marketplace | Week 10 | Coach search, booking, video sessions, Paystack |
| M5: MVP Launch | Month 4 | Full MVP live in production, first 100 users |
| M6: 1,000 Users | Month 5 | 1,000 registered users, 100 paying |
| M7: 500 Coaches | Month 8 | 500 verified coaches on platform |
| M8: Community Live | Month 6 | Community module with moderation live |
| M9: Corporate Launch | Month 7 | First 3 corporate clients signed and onboarded |
| M10: 10,000 MAU | Month 8 | 10,000 monthly active users |
| M11: Mobile App Store Launch | Month 5 | iOS App Store + Google Play Store |
| M12: 50,000 MAU / $2M ARR | Month 12 | Year 1 revenue and user targets |

---

## 5. Dependency Management

### Critical Path Dependencies

```
Database design ──────────────► Backend services ──► Frontend integration
     │
     ▼
Auth service ────────────────────────────────────────► All services
     │
     ▼
User service ──────► AI service context ──────────────► AI companion
     │
     ▼
Coach service ──────► Availability API ──► Booking service ──► Video sessions
     │
     ▼
Payment service ──────────────────────────────────────► Subscriptions, Payouts
```

### External Dependencies

| Dependency | Risk | Mitigation |
|---|---|---|
| Azure OpenAI capacity | Medium | Reserved throughput; fallback to GPT-4o-mini |
| Paystack API availability | Low | Retry logic; Stripe as fallback for NGN |
| Agora RTC | Low | Daily.co as backup; tested failover |
| Firebase FCM | Low | APNs direct as iOS fallback |
| Coach recruitment (500 coaches) | High | Partnerships with NIMH, NMA; dedicated recruiter |
| NITDA regulatory approval | Medium | Legal counsel engaged; conservative compliance-first approach |

---

## 6. Risk Register

| Risk | Probability | Impact | Severity | Owner | Mitigation |
|---|---|---|---|---|---|
| Key engineer resignation | Medium | High | HIGH | Engineering Lead | Documentation, knowledge sharing, competitive salaries |
| Delayed coach recruitment | High | High | HIGH | Partnerships Lead | Start recruitment Month 1; referral bonuses |
| Payment processor issues | Low | Critical | HIGH | DevOps | Multi-processor; automatic failover |
| AI safety incident | Low | Critical | HIGH | AI Engineer | Continuous safety testing; clinical advisory board |
| Low user retention | High | High | HIGH | PM | Gamification; weekly review loops; user research |
| Infrastructure cost overrun | Medium | Medium | MEDIUM | DevOps | Cost monitoring alerts; autoscaling |
| Competitor enters market | Medium | Medium | MEDIUM | Leadership | Accelerate roadmap; deepen local moat |
| NDPR compliance gap | Low | High | HIGH | DPO | Legal review before launch; DPIA |
| App Store rejection | Low | High | MEDIUM | Mobile Lead | Review Apple guidelines; pre-submission review |
| Data breach | Low | Critical | HIGH | Security | Pen testing; SIEM; incident response plan |

---

## 7. Sprint Planning Template

### Sprint Goal Format
```
"By the end of this sprint, users will be able to [key action] 
so that [business value]."

Example Sprint 6:
"By the end of this sprint, users will be able to book a video 
session with a verified coach and pay via Paystack so that 
the platform can generate its first coaching revenue."
```

### Story Point Scale (Fibonacci)

| Points | Complexity | Time estimate (rough) |
|---|---|---|
| 1 | Trivial | < 2 hours |
| 2 | Very simple | Half day |
| 3 | Simple | 1 day |
| 5 | Moderate | 2–3 days |
| 8 | Complex | 4–5 days |
| 13 | Very complex | 1 week |
| 21 | Epic-level (split it) | > 1 week |

### Team Velocity (estimated)

| Team | Sprint Capacity | Notes |
|---|---|---|
| Backend (3 engineers) | 45 points | Includes design, code review, testing |
| Frontend (2 engineers) | 30 points | Includes UI design time |
| Mobile (2 engineers) | 28 points | |
| AI Engineer | 15 points | Research-heavy work |
| DevOps | 20 points | Infrastructure + automation |
| **Total platform** | **~138 points/sprint** | |

---

## 8. Delivery Timeline Summary

```
Month 1  │ Infrastructure, Auth, DB design
Month 2  │ User module, AI companion, Mood tracker
Month 3  │ Coach module, Booking flow
Month 4  │ Payments, Video sessions, Journal, Admin → MVP LAUNCH
Month 5  │ Mobile app store launch, Gamification
Month 6  │ Community module, Notifications
Month 7  │ Corporate wellness, Advanced AI
Month 8  │ Offline support, Async messaging, 10K MAU target
Month 9  │ Couples module, Content library
Month 10 │ Ghana/Kenya prep, White-label
Month 11 │ Performance optimization, Scale testing
Month 12 │ Year 1 review, 50K MAU / $2M ARR target
```

---

*End of Project Management Document*  
*Next: [SCALABILITY.md](./SCALABILITY.md)*
