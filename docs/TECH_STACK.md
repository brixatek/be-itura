# ITURA — Technology Stack Recommendation

**Document Version:** 1.0  
**Owner:** Engineering Lead  
**Last Updated:** May 2026

---

## Philosophy

Every technology choice in the Itura stack is made against four criteria:
1. **Production-proven** — Used at scale by companies with similar loads
2. **Team-productive** — Good developer experience, strong tooling, large community
3. **Operationally sound** — Observable, operable, debuggable in production
4. **Cost-efficient** — Appropriate cost for the expected load

---

## 1. Backend — .NET 8 / ASP.NET Core

### Why .NET 8

| Reason | Detail |
|---|---|
| **Performance** | .NET 8 is consistently in the top 3 in TechEmpower web framework benchmarks, handling millions of requests/sec on modest hardware |
| **Ecosystem maturity** | ASP.NET Core, Entity Framework Core, SignalR, gRPC, MassTransit — all first-class, well-maintained libraries |
| **Type safety** | C# strong typing catches entire classes of runtime bugs at compile time |
| **AOT compilation** | Native AOT in .NET 8 enables faster startup and lower memory for containerized microservices |
| **Cloud-native** | Deep Azure integration; Microsoft hosts its own global platforms on this stack |
| **Team alignment** | Senior .NET engineers available in Nigeria and Africa; large developer community |

### Core Backend Libraries

| Library | Version | Purpose |
|---|---|---|
| ASP.NET Core | 8.x | Web API framework |
| Entity Framework Core | 8.x | ORM for PostgreSQL |
| MediatR | 12.x | CQRS command/query handling |
| FluentValidation | 11.x | Request validation rules |
| AutoMapper | 13.x | Object-to-object mapping |
| Serilog | 3.x | Structured logging |
| OpenTelemetry | 1.x | Distributed tracing |
| Polly | 8.x | Resilience: retries, circuit breakers |
| Hangfire | 1.8.x | Background job scheduling |
| Carter | 8.x | Minimal API routing (optional for lightweight endpoints) |
| BCrypt.Net | 4.x | Password hashing |
| Npgsql | 8.x | PostgreSQL driver |
| StackExchange.Redis | 2.x | Redis client |
| MongoDB.Driver | 2.x | MongoDB client |

---

## 2. Real-Time — SignalR

### Why SignalR

| Reason | Detail |
|---|---|
| **First-class .NET** | Built into ASP.NET Core; zero friction integration |
| **Transport fallback** | Automatically negotiates WebSocket → SSE → Long Poll based on client capability |
| **Horizontal scale** | Redis backplane enables consistent messaging across multiple pods |
| **Hub model** | Clean abstraction for typed real-time communication |
| **Client support** | Official SDKs for JavaScript, Flutter, iOS, Android |

### SignalR Scale Configuration

```csharp
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConnectionString, options =>
    {
        options.Configuration.ChannelPrefix = "itura-signalr";
    });
```

---

## 3. Inter-Service Communication — gRPC

### Why gRPC (for synchronous service-to-service calls)

| Reason | Detail |
|---|---|
| **Performance** | 7–10x faster than REST/JSON for service-to-service calls due to binary serialization (Protobuf) |
| **Contract-first** | `.proto` files define API contracts; client/server code generated automatically |
| **Streaming** | Native bidirectional streaming for real-time data flows |
| **Type safety** | Protobuf schema enforced at compile time |
| **Load balancing** | Native integration with Kubernetes service discovery |

### gRPC Service Example

```protobuf
// availability.proto
syntax = "proto3";
service CoachAvailabilityService {
  rpc CheckSlotAvailability (AvailabilityRequest) returns (AvailabilityResponse);
  rpc BlockSlot (BlockSlotRequest) returns (BlockSlotResponse);
  rpc StreamAvailabilityUpdates (AvailabilityStreamRequest) returns (stream SlotUpdate);
}

message AvailabilityRequest {
  string coach_id = 1;
  string date = 2;
}
```

---

## 4. Async Messaging — MassTransit + RabbitMQ

### Why MassTransit

| Reason | Detail |
|---|---|
| **Abstraction layer** | Broker-agnostic: swap RabbitMQ for Azure Service Bus without changing consumer code |
| **Saga support** | Long-running workflows (booking → payment → confirmation → notification) |
| **Outbox pattern** | Built-in transactional outbox prevents message loss |
| **Retry/redelivery** | Configurable policies with exponential backoff |
| **Dead-letter handling** | Failed messages automatically moved to error queues |

### Why RabbitMQ (vs Kafka)

| Factor | RabbitMQ | Kafka |
|---|---|---|
| **Complexity** | Low | High (ZooKeeper/KRaft, partitioning) |
| **Use case** | Task queues, routing, pub/sub | Event streaming, high-throughput log |
| **Message routing** | Topic exchange, flexible routing keys | Partition-based |
| **Message ordering** | Per-queue | Per-partition |
| **Persistence** | Queue-level | Durable log |
| **Itura fit** | **Excellent** — event routing between services | Better for analytics event streaming at scale |

Decision: **RabbitMQ** for MVP and Year 1; evaluate Kafka migration for analytics event pipeline at Year 2 scale.

---

## 5. Primary Database — PostgreSQL 16

### Why PostgreSQL

| Reason | Detail |
|---|---|
| **ACID compliance** | Critical for financial data (payments, wallet, payouts) |
| **Rich type system** | JSONB for flexible schema, arrays, full-text search, enums |
| **Row Level Security** | Multi-tenant data isolation without application-layer complexity |
| **Extensions** | TimescaleDB for time-series mood data; pgcrypto for PII encryption |
| **Maturity** | 35+ years; proven at Uber, Instagram, Shopify scale |
| **Azure PostgreSQL** | Managed service, automatic backups, read replicas, HA |
| **EF Core support** | Excellent Npgsql EF Core provider |

### PostgreSQL Configuration Strategy

```
Per service: dedicated database (not shared schema)
  ├── itura_auth        → auth-service
  ├── itura_users       → user-service
  ├── itura_coaches     → coach-service
  ├── itura_bookings    → booking-service
  ├── itura_payments    → payment-service
  ├── itura_journal     → journal-service
  ├── itura_mood        → mood-service (TimescaleDB extension)
  ├── itura_community   → community-service
  ├── itura_notifications → notification-service
  ├── itura_subscriptions → subscription-service
  └── itura_analytics   → analytics-service
```

---

## 6. Cache Layer — Redis 7

### Why Redis

| Reason | Detail |
|---|---|
| **Speed** | Sub-millisecond read/write; orders of magnitude faster than DB for hot data |
| **Data structures** | Strings, hashes, sorted sets, lists — perfect for rate limiting, leaderboards, sessions |
| **Pub/Sub** | SignalR backplane; real-time event broadcasting |
| **TTL** | Native expiration for cache entries |
| **Cluster mode** | Horizontal scale for high availability |
| **Azure managed** | Azure Cache for Redis Premium with geo-replication |

### Redis Use Cases in Itura

| Use Case | Data Structure | TTL |
|---|---|---|
| JWT blacklist | Set | 15 min (access token lifetime) |
| Rate limiting | Sorted set (sliding window) | 1 min |
| User session cache | Hash | 30 min |
| Feature flags | String | 5 min |
| Coach availability cache | Hash | 2 min |
| Leaderboard (XP) | Sorted set | 1 hour |
| OTP codes | String | 10 min |
| SignalR backplane | Channel | Auto-managed |

---

## 7. Document Store — MongoDB 7

### Why MongoDB (for AI/unstructured data)

| Reason | Detail |
|---|---|
| **Flexible schema** | Conversation history varies in structure; no rigid schema needed |
| **Document model** | A conversation thread = 1 document; natural representation |
| **Aggregation pipeline** | Powerful analytics queries on conversation data |
| **Horizontal sharding** | Scale conversation data independently from relational data |
| **Atlas AI** | Vector search for semantic similarity (future: find similar conversation patterns) |

### MongoDB Collections

```
itura_ai_db
  ├── conversations         # Full conversation history per user
  ├── ai_insights           # Generated insights and recommendations
  ├── sentiment_analysis    # Cached sentiment results
  └── content_embeddings    # Vector embeddings for semantic search
```

---

## 8. Frontend — Next.js 14 + TypeScript + Tailwind CSS

### Why Next.js 14

| Reason | Detail |
|---|---|
| **App Router** | Server Components for fast initial load; Client Components for interactivity |
| **SSR/SSG** | Coach discovery pages pre-rendered for SEO; user dashboard SSR for freshness |
| **Performance** | Automatic code splitting, image optimization, font optimization |
| **API Routes** | BFF (Backend for Frontend) pattern for web-specific aggregation |
| **TypeScript first** | Full type safety across frontend codebase |
| **React ecosystem** | Access to entire React library ecosystem |
| **Vercel / Azure Static** | Easy deployment to Azure Static Web Apps or Vercel |

### Why TypeScript

| Reason | Detail |
|---|---|
| **Type safety** | Eliminates entire classes of runtime errors; critical for financial and health UI |
| **IDE support** | Superior autocomplete, refactoring, and error detection |
| **API contract** | Share types between frontend and backend (shared type packages) |
| **Team scalability** | Type-annotated code is self-documenting for growing teams |

### Why Tailwind CSS

| Reason | Detail |
|---|---|
| **Design system** | Utility-first approach enables consistent design without custom CSS sprawl |
| **Performance** | PurgeCSS removes unused styles; minimal CSS bundle |
| **Dark mode** | First-class dark mode support (important for wellness: night-time use) |
| **Responsive** | Mobile-first responsive utilities built-in |
| **Customizable** | Design tokens (colors, spacing, typography) configured once, used everywhere |

### Frontend Supporting Libraries

| Library | Purpose |
|---|---|
| TanStack Query (React Query) | Server state management, caching, background refetching |
| Zustand | Client state management (auth, UI state) |
| React Hook Form + Zod | Form handling with schema validation |
| Radix UI | Accessible headless UI components (dialog, select, etc.) |
| Recharts / Tremor | Mood charts, analytics visualizations |
| @microsoft/signalr | SignalR client for real-time features |
| Framer Motion | Animation for emotional engagement (breathing circles, transitions) |
| next-i18next | Internationalization for multi-language support |
| dayjs | Date/time manipulation |
| Lucide React | Icon library |

---

## 9. Mobile — Flutter 3.x

### Why Flutter

| Reason | Detail |
|---|---|
| **Single codebase** | One codebase for iOS and Android; halves mobile development cost |
| **Performance** | Compiled to native ARM code; 60/120fps animations |
| **UI fidelity** | Custom rendering engine (Skia/Impeller) — pixel-perfect UI control |
| **Dart** | Type-safe, easy to learn for JavaScript/Kotlin developers |
| **Package ecosystem** | pub.dev has mature packages for video, push notifications, secure storage, biometrics |
| **Platform channels** | Native code invocation when needed (biometrics, background services) |
| **Google + Fuchsia** | Long-term investment and maintenance guaranteed |

### Flutter Architecture (Clean Architecture + BLoC)

```
lib/
├── core/
│   ├── di/               # Dependency injection (get_it)
│   ├── network/          # Dio HTTP client + interceptors
│   ├── storage/          # Secure storage (flutter_secure_storage)
│   ├── router/           # go_router navigation
│   └── theme/            # Design system tokens
│
├── features/
│   ├── auth/
│   │   ├── data/         # Repositories + data sources
│   │   ├── domain/       # Entities + use cases
│   │   └── presentation/ # BLoC + Pages + Widgets
│   ├── mood/
│   ├── journal/
│   ├── ai_companion/
│   ├── booking/
│   ├── community/
│   └── profile/
│
└── shared/
    ├── widgets/          # Shared UI components
    ├── utils/            # Extensions, helpers
    └── constants/
```

### Flutter Key Packages

| Package | Purpose |
|---|---|
| flutter_bloc | State management (BLoC pattern) |
| get_it | Dependency injection |
| go_router | Declarative navigation + deep linking |
| dio | HTTP client with interceptors |
| flutter_secure_storage | Encrypted local storage (tokens, user data) |
| local_auth | Biometric authentication (Face ID, fingerprint) |
| firebase_messaging | Push notifications |
| agora_rtc_engine | Video/voice sessions |
| signalr_netcore | SignalR real-time connection |
| hive_flutter | Offline data persistence |
| flutter_local_notifications | Local scheduled notifications |
| image_picker | Profile photo, media upload |
| in_app_purchase | Future: in-app purchases |
| flutter_native_splash | Branded splash screen |
| lottie | Animated illustrations (breathing exercises) |
| fl_chart | Mood history charts |

---

## 10. Infrastructure — Docker + Kubernetes + Azure

### Why Docker

| Reason | Detail |
|---|---|
| **Reproducibility** | Same container runs identically on dev machine and production |
| **Isolation** | Each service has its own dependencies, no conflicts |
| **Fast CI/CD** | Build once, deploy anywhere |
| **Layer caching** | Docker layer caching speeds up CI builds significantly |

### Why Kubernetes (AKS)

| Reason | Detail |
|---|---|
| **Auto-scaling** | HPA scales pods based on CPU/memory/custom metrics |
| **Self-healing** | Failed pods automatically restarted; unhealthy nodes replaced |
| **Zero-downtime deploys** | Rolling updates, blue-green, canary deployments |
| **Resource efficiency** | Bin-packing: fits more workloads on fewer VMs |
| **Ecosystem** | Helm, Istio, cert-manager, external-secrets — rich operator ecosystem |
| **Azure-native** | AKS deeply integrated with Azure AD, Azure Monitor, ACR |

### Why Azure (vs AWS)

| Factor | Azure | AWS | Decision |
|---|---|---|---|
| OpenAI integration | Azure OpenAI Service (same underlying model, enterprise agreement) | AWS Bedrock (wrapped models) | **Azure wins** |
| .NET optimization | First-class .NET support, MSFT product | Good .NET support | **Azure wins** |
| Africa region | West Africa (Nigeria) region GA | No West Africa region | **Azure wins** |
| Enterprise sales | Existing enterprise relationships in Africa | Fewer enterprise touchpoints | **Azure wins** |
| Cost | Comparable | Comparable | Tie |

---

## 11. Video Conferencing — Agora / Daily.co

### Why Not Build Custom WebRTC

Custom WebRTC is a significant engineering effort requiring:
- STUN/TURN server infrastructure
- SFU (Selective Forwarding Unit) for multi-party calls
- Network traversal, codec handling, recording infrastructure
- Ongoing maintenance and scaling

### Why Agora

| Reason | Detail |
|---|---|
| **Africa coverage** | Edge nodes in Nigeria, Ghana, South Africa for low-latency |
| **SDK quality** | Flutter SDK well-maintained, active community |
| **HIPAA-eligible** | BAA available for health data compliance |
| **Recording** | Cloud recording to Azure Blob built-in |
| **Reliability** | 99.99% uptime SLA |
| **Pricing** | Pay-per-minute, cost-effective at scale |

Alternative: **Daily.co** (simpler API, WebRTC-based, good developer experience — evaluate for MVP)

---

## 12. Payments — Paystack + Stripe

### Why Paystack (Africa)

| Reason | Detail |
|---|---|
| **Africa-native** | Supports NGN, GHS, KES, ZAR natively |
| **Payment methods** | Cards, bank transfer, USSD, mobile money |
| **Compliance** | CBN licensed, PCI DSS compliant |
| **API quality** | Excellent REST API, webhooks, dashboard |
| **Radar fraud** | Built-in fraud detection |
| **Payout support** | Transfer to Nigerian banks, mobile money |

### Why Stripe (Global)

| Reason | Detail |
|---|---|
| **Global coverage** | 135+ currencies, 40+ countries |
| **Subscription engine** | Best-in-class recurring billing, proration, trials |
| **Developer experience** | Industry-leading API documentation and SDKs |
| **Financial infrastructure** | Stripe Connect for coach payouts globally |

### Implementation

```
Nigerian users → Paystack
International users → Stripe
Subscription management → Both (via platform's own subscription service)
Coach payouts → Paystack Transfer API (Nigeria) | Stripe Connect (global)
```

---

## 13. AI — Azure OpenAI + Azure AI Services

### Why Azure OpenAI (vs direct OpenAI API)

| Reason | Detail |
|---|---|
| **Data residency** | Data processed in Azure region (GDPR/NDPR compliance) |
| **Enterprise SLA** | 99.9% uptime; Microsoft contractual guarantees |
| **HIPAA eligible** | BAA available |
| **Private networking** | Access via Azure Private Endpoint (no public internet) |
| **Same models** | GPT-4o, GPT-4 Turbo — same models as OpenAI |
| **Cost** | Reserved capacity pricing for predictable costs at scale |

### Azure AI Services Used

| Service | Use Case |
|---|---|
| Azure OpenAI GPT-4o | AI companion conversations |
| Azure AI Language | Sentiment analysis, entity extraction |
| Azure AI Content Safety | Content moderation, harmful content detection |
| Azure AI Speech | Voice-to-text (future: voice companion) |
| Azure Cognitive Search | Enhanced coach search with semantic ranking |

---

## 14. Full Stack Summary Table

| Layer | Technology | Version | Justification |
|---|---|---|---|
| Backend API | ASP.NET Core | 8.x | Performance, ecosystem, .NET team |
| Language (BE) | C# | 12 | Type-safe, modern, AOT-ready |
| CQRS | MediatR | 12.x | Clean CQRS implementation |
| ORM | Entity Framework Core | 8.x | Code-first migrations, LINQ |
| Real-time | SignalR | 8.x (ASP.NET Core) | Chat, notifications, presence |
| gRPC | Grpc.AspNetCore | 2.x | Service-to-service performance |
| Messaging | MassTransit + RabbitMQ | 8.x | Async events, sagas |
| Primary DB | PostgreSQL | 16 | ACID, rich types, proven scale |
| Cache | Redis | 7.x | Speed, data structures, SignalR |
| Document DB | MongoDB | 7.x | AI conversations, unstructured data |
| Search | Elasticsearch | 8.x | Coach/content full-text search |
| Background jobs | Hangfire | 1.8.x | Scheduled tasks, retries |
| Frontend | Next.js 14 | 14.x | SSR, performance, React ecosystem |
| Frontend language | TypeScript | 5.x | Type safety, team scale |
| CSS | Tailwind CSS | 3.x | Utility-first, design system |
| State (server) | TanStack Query | 5.x | Caching, sync, background refetch |
| State (client) | Zustand | 4.x | Simple, performant |
| Mobile | Flutter | 3.x | Cross-platform, performance |
| Mobile state | flutter_bloc | 8.x | Predictable state, testable |
| Video | Agora RTC | Latest | Africa coverage, HIPAA |
| AI model | GPT-4o (Azure) | Latest | Intelligence, safety, compliance |
| AI safety | Azure AI Content Safety | Latest | Pre/post content filtering |
| Payment (Africa) | Paystack | Latest | Africa-native, full stack |
| Payment (Global) | Stripe | Latest | Global coverage, subscriptions |
| Container | Docker | 25.x | Reproducibility, isolation |
| Orchestration | Kubernetes (AKS) | 1.29+ | Scale, self-healing, ecosystem |
| Cloud | Azure | — | OpenAI, West Africa region, .NET |
| IaC | Terraform | 1.7+ | Declarative, versioned infra |
| CI/CD | Azure DevOps | — | Integrated with Azure ecosystem |
| Monitoring | Grafana + Prometheus | Latest | Dashboards + alerting |
| Tracing | OpenTelemetry + App Insights | Latest | Distributed traces |
| Logging | Serilog → Log Analytics | 3.x | Structured logs, searchable |

---

*End of Tech Stack Document*  
*Next: [DATABASE.md](./DATABASE.md)*
