# ITURA — Scalability & Growth Strategy

**Document Version:** 1.0  
**Owner:** Engineering Lead / CTO  
**Last Updated:** May 2026

---

## 1. Scale Strategy for Millions of Users

### 1.1 Scaling Phases

| Phase | MAU | Infrastructure | Architecture |
|---|---|---|---|
| **Seed (Now–Month 6)** | 0–5K | Docker Compose (dev), AKS 3-node (prod) | MVP microservices |
| **Early (Month 6–12)** | 5K–50K | AKS 10-node, PostgreSQL + 2 read replicas | Full microservices, Redis cluster |
| **Growth (Year 2)** | 50K–500K | AKS 30-node, PostgreSQL sharded, CDN | Event sourcing, CQRS mature |
| **Scale (Year 3)** | 500K–5M | AKS 100+ node, global multi-region | Global distribution, Kafka pipeline |
| **Hyper-scale (Year 4+)** | 5M–50M | Custom optimizations, dedicated cloud | Edge AI, regional clusters |

### 1.2 Database Scaling Roadmap

**Year 1 (0–50K users):**
- PostgreSQL Flexible Server (General Purpose, 4 vCPU / 16GB)
- 1 primary + 1 synchronous read replica
- Connection pooling via PgBouncer (transaction mode)

**Year 2 (50K–500K users):**
- Upgrade to 16 vCPU / 64GB
- 1 primary + 2 read replicas (geographic spread)
- Mood data migrated to TimescaleDB dedicated instance
- Analytics queries → Redshift or Azure Synapse

**Year 3 (500K–5M users):**
- Horizontal sharding by `user_id` hash (4 shards initially)
- Citus extension for PostgreSQL distributed tables
- OLAP queries completely separated from OLTP

### 1.3 Caching Strategy at Scale

| Scale | Redis Setup | TTL Strategy |
|---|---|---|
| 0–50K | Single Redis instance (Premium P1, 6GB) | As documented |
| 50K–500K | Redis Cluster (3 nodes, 13GB each) | Aggressive caching: profile 30min |
| 500K+ | Redis Cluster + regional replicas | Edge caching via Azure CDN for static data |

### 1.4 AI Service Scaling

The AI service is the most compute-intensive. Scaling approach:

- **Azure OpenAI reserved capacity:** Scale from 10K → 100K → 500K TPM
- **Response caching:** Identical prompts (common questions) cached in Redis (1hr TTL)
- **Async processing:** Non-urgent AI tasks (weekly insights, recommendations) run via background jobs, not request path
- **Model tiering:** 
  - Free tier users → GPT-4o-mini (cheaper, faster)
  - Pro+ users → GPT-4o (full quality)
  - Executive → GPT-4o with extended context

---

## 2. Cost Optimization

### 2.1 Cloud Cost Structure (Year 1 Projections)

| Service | Monthly Cost (USD) | % of Total |
|---|---|---|
| Azure Kubernetes Service | $800 | 16% |
| Azure PostgreSQL | $600 | 12% |
| Azure Cache for Redis | $400 | 8% |
| Azure OpenAI (GPT-4o) | $1,500 | 30% |
| Azure Blob Storage | $100 | 2% |
| Azure Front Door | $200 | 4% |
| Azure Monitor + App Insights | $200 | 4% |
| Azure Communication Services | $150 | 3% |
| Agora RTC | $500 | 10% |
| SendGrid | $100 | 2% |
| Paystack (transaction fees) | ~variable | — |
| **Total (excluding variable)** | **~$4,550/month** | |

### 2.2 Cost Optimization Levers

| Optimization | Saving | Implementation |
|---|---|---|
| GPT-4o-mini for Free tier | -40% AI cost | Route free users to mini model |
| Azure Reserved Instances | -30% compute | 1-year commit on AKS nodes |
| Spot instances for batch jobs | -70% batch compute | Background jobs on spot VMs |
| Aggressive caching | -20% DB cost | Cache common queries 30 min |
| TimescaleDB compression | -60% mood data storage | Auto-compression after 7 days |
| CDN for static assets | -30% bandwidth | Azure Front Door CDN |
| Conversation summarization | -50% AI context tokens | Compress conversation history |

---

## 3. International Expansion

### 3.1 Expansion Roadmap

| Wave | Markets | Timeline | Key Requirements |
|---|---|---|---|
| **Wave 1 (Launch)** | Nigeria | Month 0 | NGN, English, Paystack |
| **Wave 2** | Ghana, Kenya | Month 14 | GHS/KES, M-Pesa, local coaches |
| **Wave 3** | South Africa, Rwanda | Month 20 | ZAR, English, Swahili |
| **Wave 4** | Francophone Africa (CI, SN, CM) | Month 28 | XOF, French language support |
| **Wave 5** | UK/Europe (diaspora) | Month 36 | GBP/EUR, GDPR, NHS coaching network |
| **Wave 6** | North America (diaspora) | Month 42 | USD, HIPAA, insurance integration |

### 3.2 Market Entry Requirements

For each new market:

**Regulatory:**
- Data protection law compliance (Kenya: DPA 2019; South Africa: POPIA; EU: GDPR)
- Healthcare regulations for therapy delivery
- Payment processor licensing

**Product:**
- Local currency support in payment-service
- Local payment methods (M-Pesa, MTN MoMo, Flutterwave)
- Local language content (at minimum: English + primary local language)
- Local coach recruitment (minimum 50 verified coaches per market)
- Pricing adaptation (purchasing power parity)

**Infrastructure:**
- Azure region proximity (West Africa → West Europe → East Africa)
- CDN edge nodes in target region
- Latency test: P95 API response < 300ms from target country

---

## 4. Localization Strategy

### 4.1 Internationalization (i18n) Architecture

**Backend:**
- All user-facing strings stored in translation files (`/resources/i18n/`)
- Language negotiation via `Accept-Language` header
- Database: store content in multiple languages where needed (coaches can have multi-language profiles)

**Frontend (Next.js):**
```
next-i18next configuration:
  defaultLocale: 'en'
  locales: ['en', 'pcm', 'yo', 'ig', 'ha', 'fr', 'sw']
  
Translation files:
  /public/locales/en/common.json
  /public/locales/pcm/common.json  (Nigerian Pidgin)
  /public/locales/yo/common.json   (Yoruba)
  etc.
```

**Mobile (Flutter):**
```dart
// flutter_localizations + intl
supportedLocales: [
  Locale('en'),
  Locale('pcm'),  // Pidgin
  Locale('yo'),
  Locale('ig'),
  Locale('ha'),
  Locale('fr'),
  Locale('sw'),
],
```

### 4.2 Content Localization

| Content Type | Localization Approach |
|---|---|
| UI strings | Professional translation + community review |
| Journal prompts | Culturally adapted per region (not direct translation) |
| Wellness content | Region-specific content created by local coaches |
| AI companion (Sera) | Responds in user's preferred language; GPT-4o multilingual capability |
| Push notifications | Translated templates per locale |
| Coach profiles | Coaches self-declare languages; clients filter by language |

### 4.3 Cultural Adaptation (Not Just Translation)

- **Spiritual dimension:** Many African users integrate faith with wellness. Platform supports "Spiritual" AI tone and allows coaches to bring faith-informed approaches
- **Family context:** Wellness often involves family dynamics. Content reflects communal African family structures
- **Economic sensitivity:** Pricing, session length options, and free-tier generosity calibrated per market purchasing power
- **Stigma navigation:** Onboarding uses non-clinical language ("emotional wellness" not "mental illness") tuned per market

---

## 5. White-Label Architecture

### 5.1 White-Label Offering (Year 2)

Corporate clients and healthcare organizations can license Itura as their own branded wellness platform:

**What's White-Labeled:**
- Mobile app (custom branding, app icon, splash screen)
- Web app (custom domain, logo, color scheme)
- AI companion (custom persona name and personality)
- Welcome email templates

**What's Shared (infrastructure):**
- Core platform APIs (shared backend, tenant-isolated)
- AI models (shared Azure OpenAI deployment)
- Coach network (white-label clients access the same verified coaches)

**What's Separate (per tenant):**
- Database rows (tenant_id isolation)
- Coach earnings and payment flows
- Analytics and reporting
- Branding assets

### 5.2 Technical Implementation

```
White-Label Configuration (per tenant, stored in DB):
{
  "tenantId": "tenant_abc_corp",
  "brandName": "ABC WellnessHub",
  "primaryColor": "#005A9C",
  "logoUrl": "https://cdn.itura.app/tenants/abc_corp/logo.png",
  "aiCompanionName": "Alex",
  "aiPersonality": "professional",
  "customDomain": "wellness.abccorp.com",
  "mobileAppConfig": {
    "appId": "com.abccorp.wellness",
    "appName": "ABC WellnessHub",
    "splashImage": "..."
  },
  "featureFlags": {
    "community": false,      // corporate clients may disable public community
    "couplesWellness": false,
    "contentLibrary": true
  }
}
```

---

## 6. Enterprise SaaS Expansion

### 6.1 Enterprise SaaS Tiers

| Tier | Target | Price | SLA | Features |
|---|---|---|---|---|
| **Corporate Starter** | 50–200 employees | ₦500K/month | 99.9% | Basic dashboard, 100 session credits |
| **Corporate Growth** | 200–1000 employees | ₦2M/month | 99.9% | Full EAP, analytics, custom wellness programs |
| **Enterprise** | 1000+ employees | Custom | 99.95% | White-label option, dedicated success manager, SLA with penalties |
| **Healthcare/Insurer** | Health plans | Custom | 99.99% | Clinical integrations, HIPAA BAA, claims support |

### 6.2 B2B Sales Motion

```
Inbound:
  Content marketing (burnout statistics, ROI of wellness)
    → Demo request → Demo → Proposal → Contract (30–90 day cycle)

Outbound:
  HR conferences, LinkedIn campaigns → SDR outreach → Demo
  
Partnership:
  HR software integrations (BambooHR, SAP SuccessFactors, Workday)
    → Marketplace listing → Inbound enterprise leads
```

---

*End of Scalability & Growth Document*  
*Next: [UI_UX.md](./UI_UX.md)*
