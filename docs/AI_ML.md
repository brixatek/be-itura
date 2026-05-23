# ITURA — AI & Machine Learning Architecture

**Document Version:** 1.0  
**Owner:** AI Engineering  
**Last Updated:** May 2026

---

## Table of Contents

1. [AI Strategy Overview](#1-ai-strategy-overview)
2. [AI Emotional Companion (Sera)](#2-ai-emotional-companion-sera)
3. [Sentiment Analysis Pipeline](#3-sentiment-analysis-pipeline)
4. [Recommendation Engine](#4-recommendation-engine)
5. [AI Content Moderation](#5-ai-content-moderation)
6. [Emotion Detection Pipeline](#6-emotion-detection-pipeline)
7. [Personalized Wellness Recommendations](#7-personalized-wellness-recommendations)
8. [AI Safety Framework](#8-ai-safety-framework)
9. [Model Management & MLOps](#9-model-management--mlops)
10. [AI Evaluation Framework](#10-ai-evaluation-framework)

---

## 1. AI Strategy Overview

### AI Philosophy

Itura's AI strategy is built on three pillars:

1. **Augment, don't replace:** AI extends human capacity (24/7 availability, personalization at scale) but never replaces licensed human professionals for clinical care
2. **Safety first:** Every AI output is filtered through safety layers before reaching users; crisis detection is non-negotiable
3. **Contextually intelligent:** AI should know enough about the user to be genuinely helpful, not just generically responsive

### AI Capability Map

| Capability | Technology | Maturity |
|---|---|---|
| Conversational companion (Sera) | GPT-4o (Azure OpenAI) | Production MVP |
| Sentiment analysis | Azure AI Language | Production MVP |
| Crisis detection | Custom classifier + regex | Production MVP |
| Content moderation | Azure AI Content Safety + custom | Production MVP |
| Journaling prompts | GPT-4o | Production MVP |
| Mood pattern insights | Statistical + GPT-4o narrative | Post-MVP |
| Coach recommendations | Collaborative filtering | Post-MVP |
| Burnout risk scoring | Custom model | Post-MVP |
| Emotion detection (text) | Fine-tuned classifier | Post-MVP |
| Wellness content personalization | Hybrid recommendation | Post-MVP |
| Voice companion | Azure AI Speech + GPT-4o | Year 2 |
| Predictive mood alerts | LSTM/Prophet | Year 2 |

---

## 2. AI Emotional Companion (Sera)

### 2.1 System Design

```
User Message
    │
    ▼
┌──────────────────────────────────────────────────────┐
│                  AI SERVICE                          │
│                                                      │
│  1. Pre-Processing Layer                             │
│     ├── Rate limit check (Redis)                     │
│     ├── Input safety filter (Azure Content Safety)   │
│     └── Message validation (length, format)          │
│                                                      │
│  2. Context Assembly Layer                           │
│     ├── Retrieve conversation history (MongoDB)      │
│     ├── Fetch user profile summary (gRPC)            │
│     ├── Fetch today's mood (gRPC)                    │
│     ├── Fetch wellness goals (cache)                 │
│     └── Assemble context-rich system prompt          │
│                                                      │
│  3. Azure OpenAI GPT-4o                              │
│     ├── Streaming response                           │
│     └── Token usage tracked                         │
│                                                      │
│  4. Post-Processing Layer                            │
│     ├── Crisis keyword detector                      │
│     ├── Medical advice detector                      │
│     ├── PII leakage detector                         │
│     └── Response quality check                       │
│                                                      │
│  5. Output Layer                                     │
│     ├── Stream to client (SSE)                       │
│     ├── Save message to MongoDB                      │
│     └── Publish SentimentAnalyzedEvent              │
└──────────────────────────────────────────────────────┘
```

### 2.2 System Prompt Design

```
SYSTEM PROMPT STRUCTURE:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

[PERSONA — Fixed, ~200 tokens]
You are Sera, a compassionate emotional wellness companion on the Itura platform.
You are warm, non-judgmental, curious, and gently encouraging.
You are NOT a therapist, psychiatrist, or medical professional.
You do NOT diagnose conditions or prescribe treatments.
You are a supportive companion who listens, reflects, and gently guides.
Your approach draws from principles of Motivational Interviewing, 
Cognitive Behavioral Therapy (CBT), and Positive Psychology.
You are culturally aware and sensitive to African lived experiences.

[BEHAVIORAL RULES — Fixed, ~300 tokens]
1. Always acknowledge feelings before offering perspectives or suggestions
2. Ask one thoughtful follow-up question per response (not multiple)
3. When suggesting coping strategies, offer specific, actionable techniques
4. If the user mentions seeing a therapist or coach, reinforce and encourage
5. Never minimize or dismiss any emotional experience
6. Use the user's preferred name if known
7. Never claim to be human if directly asked
8. In a crisis: ALWAYS activate the safety response (see crisis protocol)

[USER CONTEXT — Dynamic, ~300 tokens]
User name: {fullName}
Current wellness level: {level} ({levelName})
Primary wellness goals: {goals}
Current mood (today): {moodScore}/5 ({moodLabel}) — logged {timeAgo}
Current streak: {streakDays} days
Recent concerns mentioned: {recentThemes}
Subscription tier: {tier}

[CONVERSATION CONTEXT — Dynamic, ~500 tokens]
{conversationSummary — last 10 turns or summary of older conversation}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Total system prompt: ~1,300 tokens
User message: up to 500 tokens  
Total input: ~1,800 tokens
Response budget: up to 600 tokens
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

### 2.3 Conversation Context Management

**Challenge:** GPT-4o context window is 128K tokens but storing full history is expensive.

**Solution: Progressive summarization**

```
Conversation with 50 messages:
  ├── Last 10 messages: included verbatim (most relevant)
  ├── Messages 11–30: summarized into 3 paragraphs by GPT-4o
  └── Messages 31–50: summarized into 1 paragraph (key themes only)

Summary generation job (background):
  - Triggered when conversation reaches 15 messages
  - GPT-4o-mini used for summarization (cheaper)
  - Summaries stored in MongoDB alongside raw messages
  - Raw messages archived (still accessible) but not included in active context
```

**MongoDB Document Structure:**
```json
{
  "_id": "conv_01H9AB...",
  "userId": "usr_01H7Y3...",
  "createdAt": "2026-05-01T08:00:00Z",
  "lastMessageAt": "2026-05-22T09:15:00Z",
  "messageCount": 47,
  "summary": "User has been dealing with work anxiety, particularly related to a new manager who is overly critical. They've tried journaling (found it helpful) and breathing exercises (partially helpful). They have a presentation coming up on May 25th that they're very anxious about. Their mood has been 2-3/5 this week. They respond well to validation and concrete CBT techniques.",
  "messages": [
    {
      "id": "msg_01H...",
      "role": "user",
      "content": "I feel so anxious about tomorrow's presentation",
      "timestamp": "2026-05-22T09:10:00Z",
      "tokenCount": 12,
      "sentiment": { "score": -0.72, "label": "negative" }
    },
    {
      "id": "msg_02H...",
      "role": "assistant",
      "content": "That sounds really stressful, Amara. Presentations can feel overwhelming, especially when a lot is riding on them. What part of tomorrow feels most daunting right now?",
      "timestamp": "2026-05-22T09:10:03Z",
      "tokenCount": 38
    }
  ],
  "recentWindow": 10,
  "archivedMessages": [...]
}
```

### 2.4 Persona Customization

Users can customize Sera's persona within safe bounds:

| Customizable | Options |
|---|---|
| Name | User-chosen (up to 20 chars; profanity filtered) |
| Communication tone | Friendly (default) · Professional · Spiritual · Direct |
| Response style | Conversational · Structured (with numbered steps) |

Non-customizable (safety-critical):
- Crisis detection behavior
- Clinical boundary (no diagnosis)
- Mandatory safety disclosures

---

## 3. Sentiment Analysis Pipeline

### 3.1 Architecture

```
Input: User message (AI chat) or journal entry (with permission)
    │
    ▼
Azure AI Language — Sentiment Analysis
    ├── Document sentiment: Positive / Negative / Neutral (0.0–1.0)
    ├── Sentence-level sentiment
    └── Opinion mining (subject + aspect + sentiment)
    │
    ▼
Emotion Classifier (custom fine-tuned model)
    ├── Labels: anxious | sad | angry | overwhelmed | content | hopeful | grateful | neutral
    ├── Confidence score per label
    └── Multi-label output (multiple emotions can be present)
    │
    ▼
Storage: mood_insights table (PostgreSQL)
    {
      user_id, source_type (chat|journal), source_id,
      sentiment_score, sentiment_label,
      primary_emotion, secondary_emotion, confidence,
      analyzed_at
    }
    │
    ▼
Used by:
  ├── AI context assembly (recent sentiment feeds into system prompt)
  ├── Weekly mood insights generation
  ├── Coach dashboard (with user permission)
  ├── Corporate burnout risk scoring (aggregate, anonymized)
  └── Recommendation engine
```

### 3.2 Emotion Classifier

**Base Model:** `microsoft/deberta-v3-base` fine-tuned on:
- GoEmotions dataset (27 emotion labels, reduced to 8 relevant categories)
- Custom mental wellness conversation dataset
- Reviewed and relabeled by clinical psychologist consultants

**Training Infrastructure:**
- Azure ML compute cluster (GPU)
- Training time: ~4 hours on 4x A100 GPUs
- Evaluation metric: Macro F1 (target: > 0.75)
- Re-training schedule: quarterly with new labeled data

---

## 4. Recommendation Engine

### 4.1 Coach Recommendation

**Algorithm: Hybrid Collaborative + Content-Based Filtering**

```
Input signals:
  ├── User wellness goals (from onboarding)
  ├── User assessment results (anxiety/depression/burnout risk level)
  ├── User demographic (age, location, language preference)
  ├── Past session ratings (explicit feedback)
  ├── Time-on-platform with certain coaches (implicit feedback)
  └── Community posts topics (implicit interest signals)

Content-based features:
  ├── Coach specialties
  ├── Coach language
  ├── Coach approach/style
  ├── Session price (within user's willingness range)
  └── Coach gender (if preference specified)

Collaborative filtering:
  ├── Users similar to this user → which coaches did they book?
  ├── Coaches rated 4+ by users with similar goals → recommend
  └── Avoid coaches booked but not re-booked (negative signal)

Output: Ranked list of 5 coach recommendations
  ├── Diversity injection: ensure variety in specialty and price
  └── New coach boost: surface new coaches with < 10 reviews
```

### 4.2 Content Recommendation

**Algorithm: Content-based with collaborative signals**

```
User reads article on "Managing workplace anxiety"
    │
    ▼
Embed article (text-embedding-3-small via Azure OpenAI)
    │
    ▼
Find semantically similar articles (cosine similarity > 0.75)
    │
    ▼
Filter by:
  ├── User's stated goals match article topic
  ├── Not already read (interaction history)
  └── Subscription tier (premium content gated)
    │
    ▼
Rank by: (similarity * 0.6) + (rating * 0.3) + (freshness * 0.1)
    │
    ▼
Return 5 recommended articles
```

---

## 5. AI Content Moderation

### 5.1 Multi-Layer Moderation Pipeline

```
User submits community post
    │
    ▼
Layer 1: Azure AI Content Safety
    ├── Hate/Discrimination detection
    ├── Violence/Graphic content detection
    ├── Self-harm/Suicide content detection
    ├── Sexual content detection
    └── Severity scores (0–6) per category
    │
    ├── BLOCK if any category severity ≥ 4
    │
    ▼
Layer 2: Custom Wellness-Specific Classifier
    ├── Medical misinformation detection
    ├── Spam/Promotional content detection
    ├── Personal contact information detection
    └── Crisis/Urgent distress signals
    │
    ├── AUTO-REJECT if: self-harm explicit, spam, contact info
    ├── HOLD for human review if: medical claims, crisis signal
    │
    ▼
Layer 3: Confidence Threshold Routing
    ├── Confidence ≥ 0.95 (safe): Publish immediately
    ├── Confidence 0.80–0.95: Publish + flag for soft review
    ├── Confidence 0.60–0.80: Hold for human review
    └── Confidence < 0.60: Auto-reject + appeals path
    │
    ▼
Human Moderator Queue (if needed)
    ├── Review within 4 hours (standard)
    ├── Review within 30 minutes (crisis signals)
    └── Action: Approve | Edit | Remove | Warn | Ban
```

### 5.2 Crisis Content Special Handling

```
Triggered by: Self-harm content in community post OR AI conversation
    │
    ▼
1. HOLD content (not published, not rejected)
2. Show author a support message:
   "We noticed you might be going through something difficult.
    Your post is being reviewed. In the meantime, if you need
    immediate support: [Crisis Line] or chat with Sera."
3. Notify human moderator (P1 alert, reviewed within 30 min)
4. Publish modified version OR reach out to user directly
```

---

## 6. Emotion Detection Pipeline

### 6.1 Real-Time Emotion Detection in Conversations

```dart
// Each AI conversation message analyzed in background
POST /ai/analyze-sentiment (internal)
{
  "text": "I've been feeling really hopeless lately, nothing seems to matter",
  "context": "ai_conversation",
  "userId": "usr_01H7Y3..."
}

Response:
{
  "sentimentScore": -0.85,
  "sentimentLabel": "very_negative",
  "primaryEmotion": "hopeless",
  "secondaryEmotions": ["sad", "overwhelmed"],
  "crisisRisk": 0.72,     // 0-1, >0.5 triggers crisis protocol check
  "confidence": 0.89
}
```

### 6.2 Crisis Detection Multi-Signal System

**Not just keyword matching — multi-signal approach:**

```
Signal 1: Regex pattern matching (fast, catches obvious cases)
  Patterns: suicide, self-harm, kill myself, end it all, no point living,
            want to die, hurt myself, can't go on, goodbye forever, etc.
  Action: Immediate crisis response if match

Signal 2: ML classifier (catches subtle signals)
  Input: Full message + conversation context
  Threshold: crisisRisk > 0.65 → trigger review
  Action: Override AI response with safety message

Signal 3: Escalating distress pattern (longitudinal)
  Track: Sentiment moving from neutral → negative → very negative over
         3+ consecutive messages
  Action: AI proactively checks in; suggest crisis line

Signal 4: PHQ-9 proxy signals (over time)
  Monitor: Hopelessness + anhedonia + worthlessness + sleep issues
  Action: Suggest professional help; offer session credits

CRISIS PROTOCOL TRIGGER → guaranteed response:
  "I hear you, and what you're sharing matters deeply.
   It sounds like you're going through something very painful right now.
   
   I'm not able to provide the level of support you need in this moment,
   but a trained counselor can. Please reach out:
   
   Nigeria Crisis Line: 0800-WELLNESS (0800-9355-6377)
   Lagos Counseling Hotline: +234 1 555 0001
   
   Would you like me to help you find a therapist on Itura right now?"
  
  [Find a Therapist]  [More Resources]  [Continue Chatting]

Backend: CrisisDetectedEvent published → admin notified → clinical review
```

---

## 7. Personalized Wellness Recommendations

### 7.1 Daily Recommendation Card

Every morning, each user receives a personalized wellness recommendation:

```
Recommendation engine runs at 6 AM per user timezone:

Inputs:
  ├── Today's day of week (Monday = work-stress peak)
  ├── Yesterday's mood score
  ├── 7-day mood trend (improving/declining/stable)
  ├── Last completed wellness activity
  ├── Active wellness goals
  ├── Weather/season (future: location-aware)
  └── Subscription tier (gates premium content)

Output types:
  ├── Breathing exercise (if mood ≤ 2 or anxiety trigger yesterday)
  ├── Journaling prompt (if no journal in 3 days)
  ├── Gratitude practice (if mood declining for 3 days)
  ├── Coach session suggestion (if no session in 14 days)
  ├── Community challenge (if engagement decreasing)
  ├── Wellness article (match to current goals)
  └── Affirmation (always as baseline)

Delivery:
  ├── Home dashboard card
  ├── Push notification (if user has push enabled)
  └── AI companion opening message
```

### 7.2 Burnout Risk Scoring (Corporate)

**Algorithm: Composite risk model**

```
Input signals (per employee, anonymized):
  ├── Mood score trend (30-day declining trend → risk)
  ├── Session frequency (decrease → risk)
  ├── Journal negative sentiment trend
  ├── AI conversation crisis score average
  └── Self-reported wellness survey

Risk score: 0–100
  0–30: Low risk (green)
  31–60: Moderate risk (amber)  
  61–80: High risk (orange)
  81–100: Critical risk (red)

Corporate dashboard shows:
  ├── Team aggregate risk distribution (pie chart)
  ├── Trend over time (line chart)
  ├── % of team in each risk tier
  └── NOT individual employee data (anonymized)

Trigger at ≥ 30% of team in high/critical:
  → HR admin alert: "Your team's burnout risk has increased significantly"
  → Suggested actions: increase session credits, team wellness session
```

---

## 8. AI Safety Framework

### 8.1 Guardrails Architecture

```
┌────────────────────────────────────────────────────────────┐
│                    SAFETY GUARDRAILS                       │
│                                                            │
│  INPUT GUARDRAILS                    OUTPUT GUARDRAILS     │
│  ─────────────────                   ───────────────────   │
│  ✓ Content safety filter             ✓ Crisis detection    │
│  ✓ Prompt injection detection        ✓ Medical advice      │
│    (e.g., "ignore previous          ✓ PII leakage          │
│     instructions")                  ✓ Factual claims       │
│  ✓ Jailbreak detection              ✓ Harmful advice       │
│    (role-play as doctor, etc.)      ✓ Suicide method       │
│  ✓ Length + format validation          details (never)     │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

### 8.2 Prohibited Outputs (Hard Rules)

The following outputs are categorically blocked regardless of user request:

| Prohibited | Example | Override Possible |
|---|---|---|
| Specific methods of self-harm | "How can I hurt myself with..." | Never |
| Medical diagnosis | "You have clinical depression" | Never |
| Prescription advice | "Take 20mg of..." | Never |
| Specific suicide methods | Any detailed method | Never |
| Harmful relationship advice | Advice encouraging abuse | Never |
| Impersonating a human | "Yes, I'm a real therapist" | Never |

### 8.3 Clinical Boundary Enforcement

```
User: "Do I have anxiety disorder?"

Without guardrail: "Based on what you've described, it sounds like
you may have Generalized Anxiety Disorder (GAD)..."  ← BLOCKED

With guardrail: "What you're describing — those persistent worries
that feel hard to control — sounds really challenging. I'm not able
to diagnose conditions; that requires assessment by a trained professional.
But I can tell you that many people experience similar feelings, and there
are effective ways to manage them. Would you like to explore that together,
or would it help to connect with one of our licensed therapists who can
give you a proper assessment?"  ← APPROVED
```

### 8.4 Human Oversight Mechanism

AI is never fully autonomous for high-stakes decisions:

| AI Decision | Human Override |
|---|---|
| Crisis response triggered | Clinical team notified, can follow up |
| Content removed by AI | Human moderator can reinstate via appeals |
| Coach match recommendation | User retains full choice |
| Burnout risk flag (corporate) | HR takes action, not AI |
| Wellness assessment scoring | Used for personalization only; not diagnosis |

### 8.5 AI Ethics Principles

1. **Transparency:** Users always know they're talking to an AI, not a human
2. **No manipulation:** AI does not use persuasive dark patterns to extend conversation or increase platform dependency
3. **Promote human connection:** AI actively encourages users to connect with human coaches when appropriate
4. **Data frugality:** AI uses minimum data necessary for context; no hoarding of personal data for model training without explicit consent
5. **Bias monitoring:** AI outputs reviewed regularly for cultural, gender, and socioeconomic bias
6. **No gatekeeping:** AI never withholds crisis resources or claims they're unavailable

---

## 9. Model Management & MLOps

### 9.1 Model Registry

| Model | Version | Purpose | Hosting | Refresh |
|---|---|---|---|---|
| GPT-4o | Azure-managed | Companion, prompts | Azure OpenAI | Continuous (Azure) |
| Emotion classifier | v1.2 | Emotion detection | Azure ML endpoint | Quarterly |
| Burnout risk model | v1.0 | Corporate risk | Azure ML endpoint | 6 months |
| Coach recommender | v1.1 | Matching | Azure ML endpoint | Monthly |
| Content embeddings | text-embedding-3-small | Semantic search | Azure OpenAI | Continuous |

### 9.2 Model Deployment Pipeline

```
Data Scientists tag new model version in MLflow
    │
    ▼
Automated evaluation suite runs:
  ├── Accuracy / F1 regression tests
  ├── Safety evaluation (200 adversarial prompts)
  ├── Bias evaluation (demographic parity checks)
  └── Latency benchmark (P95 < 200ms)
    │
    ▼ (if all pass)
Canary deployment (5% traffic)
    │
    ├── Monitor for 24 hours (error rate, user satisfaction signals)
    │
    ▼ (if metrics green)
Full rollout
    │
    ├── Old version retained in registry for 30 days (easy rollback)
    └── Old version endpoint decommissioned after 30 days
```

### 9.3 Model Monitoring

| Metric | Alert Threshold | Frequency |
|---|---|---|
| Emotion classifier accuracy | < 0.72 F1 (data drift) | Weekly evaluation |
| Crisis detection recall | < 0.99 recall | Daily evaluation |
| AI response latency (P95) | > 5 seconds | Real-time |
| Azure OpenAI token usage | > 90% of rate limit | Real-time |
| Negative user feedback rate on AI | > 15% | Daily |
| Hallucination rate (sampled) | > 2% | Weekly |

---

## 10. AI Evaluation Framework

### 10.1 Companion Quality Evaluation

**Automated evaluation (weekly):**
- Sample 100 conversations randomly
- GPT-4 evaluator scores on:
  - Empathy (1–5): Does the response acknowledge and validate emotions?
  - Helpfulness (1–5): Does the response provide value or direction?
  - Safety (1–5): Is the response free of harmful content?
  - Coherence (1–5): Does the response make sense in context?
- Target: Average ≥ 4.0 on all dimensions

**Human evaluation (monthly):**
- Clinical psychologist reviews 50 conversations
- Checks: appropriate boundaries, therapeutic alignment, crisis detection accuracy
- Feedback incorporated into system prompt refinement

### 10.2 A/B Testing Framework

New AI prompt variants tested before full rollout:

```
Variant A (control): Current system prompt
Variant B (new): Revised prompt with new persona elements

Metrics tracked:
  ├── Conversation length (proxy for engagement)
  ├── Return rate (user opens AI chat again next day)
  ├── User satisfaction rating (5-star post-conversation survey, 10% sample)
  ├── Session booking rate post-conversation (downstream value)
  └── Crisis escalation accuracy (recall metric)

Minimum sample: 500 users per variant
Test duration: 2 weeks
Significance threshold: p < 0.05
```

### 10.3 Red Teaming

Quarterly red team exercises:
- Attempt to extract harmful advice (self-harm methods, drug interactions)
- Attempt to make AI impersonate a human doctor
- Attempt prompt injection to bypass safety rules
- Attempt to extract other users' data through conversation
- Attempt to use AI for harassment of other users

All findings documented; critical failures block release.

---

*End of AI & Machine Learning Document*  
*Next: [PROJECT_MANAGEMENT.md](./PROJECT_MANAGEMENT.md)*
