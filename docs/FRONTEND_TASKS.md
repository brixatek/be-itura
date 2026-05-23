# ITURA — Frontend Engineering Task Breakdown

**Document Version:** 1.0  
**Owner:** Frontend Lead  
**Last Updated:** May 2026  
**Stack:** Next.js 14 · TypeScript · Tailwind CSS · TanStack Query · Zustand

---

## Project Structure

```
src/
├── app/                          # Next.js 14 App Router
│   ├── (auth)/                   # Auth route group (no layout)
│   │   ├── login/
│   │   ├── register/
│   │   ├── forgot-password/
│   │   └── verify-email/
│   ├── (dashboard)/              # Authenticated route group
│   │   ├── layout.tsx            # Sidebar + Header layout
│   │   ├── dashboard/
│   │   ├── mood/
│   │   ├── journal/
│   │   ├── coaches/
│   │   ├── sessions/
│   │   ├── community/
│   │   ├── ai-companion/
│   │   ├── subscription/
│   │   ├── profile/
│   │   └── settings/
│   ├── (admin)/                  # Admin route group
│   ├── (coach)/                  # Coach portal route group
│   └── (corporate)/              # Corporate HR route group
│
├── components/
│   ├── ui/                       # Base design system components
│   ├── auth/                     # Auth-specific components
│   ├── mood/
│   ├── journal/
│   ├── coaches/
│   ├── booking/
│   ├── ai-companion/
│   ├── community/
│   ├── notifications/
│   └── shared/                   # Cross-feature components
│
├── hooks/                        # Custom React hooks
├── lib/
│   ├── api/                      # API client functions
│   ├── auth/                     # Auth utilities
│   └── utils/                    # Helpers
├── store/                        # Zustand stores
├── types/                        # TypeScript type definitions
└── constants/                    # App constants
```

---

## Design System Foundation

### FE-DS-001: Design System Setup

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | L | 1 |

**Acceptance Criteria:**
- Tailwind config with Itura design tokens (colors, spacing, typography, shadows)
- Base component library: Button, Input, Select, Modal, Drawer, Toast, Badge, Avatar, Card, Spinner
- Dark mode support via `next-themes`
- Accessibility: all interactive components pass WCAG 2.1 AA keyboard navigation

**Component Structure:**
```tsx
// components/ui/Button.tsx
type ButtonVariant = 'primary' | 'secondary' | 'ghost' | 'destructive';
type ButtonSize = 'sm' | 'md' | 'lg';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  size?: ButtonSize;
  loading?: boolean;
  leftIcon?: React.ReactNode;
  rightIcon?: React.ReactNode;
}
```

**Design Tokens (Tailwind Config):**
```js
// tailwind.config.ts
colors: {
  brand: {
    50: '#f0f9f6',   // lightest
    100: '#d1f0e6',
    500: '#2d8a6d',  // primary
    600: '#1f7059',  // dark primary
    900: '#0d3d30',  // darkest
  },
  wellness: {
    calm: '#6b9fca',
    joy: '#f5a623',
    neutral: '#8a9bb5',
    growth: '#52a869',
    alert: '#e05252',
  }
}
```

**Subtasks:**
1. Set up Tailwind CSS with custom config
2. Create 12 base UI components with all variants and states
3. Create Storybook documentation for design system
4. Accessibility audit on all base components
5. Dark mode implementation and test

---

## Authentication Screens

### FE-AUTH-001: Registration Page

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 1 |

**Acceptance Criteria:**
- Registration form validates in real-time (no submit needed)
- Password strength indicator visible as user types
- Google OAuth button prominently displayed
- Shows error messages clearly without revealing security info
- Accessible: fully keyboard navigable, screen reader compatible
- Redirects to email verification page after success

**Component Structure:**
```
pages/register/
├── RegisterPage (server component — metadata)
└── RegisterForm (client component)
    ├── EmailInput
    ├── PasswordInput (with strength meter)
    ├── FirstNameInput
    └── GoogleOAuthButton
```

**API Dependencies:**
- `POST /api/v1/auth/register`
- Google OAuth redirect

**UX Considerations:**
- Clear distinction between email registration and Google OAuth
- Privacy notice link visible before submission
- Welcoming, non-clinical language ("Start your wellness journey")
- Mobile-first layout

---

### FE-AUTH-002: Login Page

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | S | 1 |

**Acceptance Criteria:**
- Email + password form
- Google OAuth button
- Forgot password link
- Remember me (7-day session) option
- Redirect to `callbackUrl` after login (preserve deep links)
- MFA step shown conditionally if user has MFA enabled

**Subtasks:**
1. Form with React Hook Form + Zod schema validation
2. `useLoginMutation` hook (TanStack Query mutation)
3. Token storage: `authStore` in Zustand (access token in memory only)
4. Refresh token managed via HttpOnly cookie (web)
5. Redirect logic: check for `callbackUrl` in query params

---

### FE-AUTH-003: Onboarding Flow

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | L | 2 |

**Acceptance Criteria:**
- Multi-step wizard (5 steps)
- Progress indicator visible (step X of 5)
- Back navigation without losing data
- Animated transitions between steps
- Skippable steps (except first)
- Personalized dashboard generated based on responses

**Component Structure:**
```
OnboardingWizard
├── StepIndicator (progress bar)
├── Step1_WelcomeIntro (brand intro + value prop)
├── Step2_WellnessGoals (multi-select chips)
├── Step3_PrimaryConcerns (multi-select chips)
├── Step4_WellnessAssessment (6-question adapted PHQ/GAD)
└── Step5_MeetSera (AI companion intro + first message)
```

**State Management:**
```ts
// store/onboardingStore.ts
interface OnboardingState {
  step: number;
  goals: string[];
  concerns: string[];
  assessmentResponses: Record<string, number>;
  setStep: (step: number) => void;
  setGoals: (goals: string[]) => void;
  submitOnboarding: () => Promise<void>;
}
```

---

## Dashboard

### FE-DASH-001: Main Dashboard

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | XL | 2–3 |

**Acceptance Criteria:**
- Personalized greeting with user's first name and time of day
- Daily mood check-in card prominent at top (if not done today)
- Streak display: mood streak + journal streak
- Quick actions: Open Sera, Log Mood, Write Journal, Book Session
- Today's session reminder (if any session today)
- Recent mood chart (7-day sparkline)
- Community prompt of the day
- Coach recommendation (if no session in past 2 weeks)
- Wellness level and XP progress bar

**Component Structure:**
```
DashboardPage
├── DashboardHeader (greeting, date, streak badges)
├── MoodCheckInCard (prominent, dismissed after completion)
├── QuickActionsBar (4 primary actions)
├── TodaySessionCard (conditional)
├── MoodSparklineCard
├── WellnessLevelCard (XP progress)
├── CoachRecommendationCard (conditional)
└── CommunityPromptCard
```

**API Dependencies:**
- `GET /api/v1/users/me` — user profile
- `GET /api/v1/mood/today` — today's mood status
- `GET /api/v1/mood/history?days=7` — sparkline data
- `GET /api/v1/bookings/me?upcoming=true` — today's sessions
- `GET /api/v1/community/prompt` — daily prompt

**UX Considerations:**
- Dashboard should load in < 1 second (use SSR + TanStack Query hydration)
- Skeleton loaders for all card components
- Empty states that encourage action (not blank spaces)

---

## Mood Tracking

### FE-MOOD-001: Mood Check-In Component

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 2 |

**Acceptance Criteria:**
- 5 emoji options with animated selection feedback
- Optional note (280 chars, live counter)
- Optional trigger tags (chip multi-select, scrollable)
- Submit in ≤ 3 taps from dashboard
- Optimistic update (streak increments immediately)
- Animated confirmation screen with streak update

**Component Structure:**
```
MoodCheckIn
├── MoodEmojiSelector (5 options with hover/active animation)
├── MoodNoteInput (optional, expandable)
├── TriggerTagSelector (chip multi-select)
├── SubmitButton
└── ConfirmationScreen (animated, shows streak)
```

**Custom Hook:**
```ts
// hooks/useMoodCheckIn.ts
export function useMoodCheckIn() {
  const mutation = useMutation({
    mutationFn: (data: MoodLogRequest) => moodApi.logMood(data),
    onSuccess: () => {
      queryClient.invalidateQueries(['mood', 'today']);
      queryClient.invalidateQueries(['mood', 'history']);
      queryClient.invalidateQueries(['user', 'streaks']);
    }
  });
  return mutation;
}
```

---

### FE-MOOD-002: Mood History Dashboard

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | L | 3 |

**Acceptance Criteria:**
- Toggle between: 7-day, 30-day, 90-day views
- Line chart showing mood score over time
- Color-coded mood levels (red → green gradient)
- Tap on data point → show note and tags for that entry
- Insights panel (if 7+ days data): pattern observations
- Export data option (PDF/CSV for Pro+)

**Components:**
```
MoodHistoryPage
├── DateRangePicker (7d/30d/90d tabs)
├── MoodLineChart (Recharts, animated)
├── MoodCalendarHeatmap (alternative view)
├── TriggerAnalysis (most common triggers)
├── InsightsPanel (AI-generated, if Pro+)
└── ExportButton (Pro+)
```

---

## Journaling

### FE-JRN-001: Journal Editor

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | L | 3 |

**Acceptance Criteria:**
- Rich text editor (bold, italic, bullet lists, headings)
- AI prompt suggestions visible in sidebar/above editor
- Emotion tag selection after writing
- Auto-save every 30 seconds
- Word count display
- Privacy indicator: "Your journal is encrypted and private"
- Share with coach toggle (explicit, prominent)
- Free tier: shows prompt to upgrade after 3rd entry in week

**Component Structure:**
```
JournalEditorPage
├── JournalToolbar (formatting options)
├── RichTextEditor (TipTap or Lexical)
├── AIPromptsPanel (collapsible sidebar)
├── EmotionTagger
├── MoodScoreSlider (optional, at time of writing)
├── CoachShareToggle
└── JournalMetaBar (word count, save status, privacy badge)
```

**Hooks:**
```ts
// hooks/useJournalAutosave.ts
// Debounces saves by 30 seconds on content change
// Shows "Saving..." / "Saved" indicator
```

---

### FE-JRN-002: Journal Feed

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 3 |

**Acceptance Criteria:**
- Paginated list of journal entries (newest first)
- Search bar with instant filtering
- Filter by emotion tag, date range, template
- Entry preview (first 100 chars, blurred after for privacy)
- Click to open full entry in editor
- Streak counter displayed prominently
- Empty state with encouraging message and "Write your first entry" CTA

---

## Coach Discovery & Booking

### FE-COACH-001: Coach Discovery Page

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | XL | 4 |

**Acceptance Criteria:**
- Search bar with instant results
- Filter panel: specialty, language, session type, price range, availability, gender, rating
- Coach cards with: photo, name, title, specialty badges, rating, price, "Available today" indicator
- Pagination / infinite scroll
- Mobile: filter panel as bottom sheet
- URL-based filter state (shareable, bookmarkable)
- Loading skeleton cards during data fetch

**Component Structure:**
```
CoachDiscoveryPage
├── CoachSearchBar
├── CoachFilterPanel (sidebar desktop, bottom sheet mobile)
│   ├── SpecialtyFilter (multi-select chips)
│   ├── LanguageFilter
│   ├── PriceRangeSlider
│   ├── SessionTypeFilter
│   ├── GenderFilter
│   └── RatingFilter
├── CoachGrid
│   └── CoachCard (repeated)
└── CoachPagination
```

**Custom Hook:**
```ts
// hooks/useCoachSearch.ts
export function useCoachSearch(filters: CoachFilters) {
  return useQuery({
    queryKey: ['coaches', filters],
    queryFn: () => coachApi.search(filters),
    staleTime: 2 * 60 * 1000, // 2 minutes
    keepPreviousData: true,   // smooth pagination
  });
}
```

---

### FE-COACH-002: Coach Profile Page

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 4 |

**Acceptance Criteria:**
- Cover photo / avatar, name, credentials, bio
- Specialty tags
- Rating breakdown (stars distribution)
- Review excerpts (with anonymous option)
- Session pricing (per session type and duration)
- Language badges
- Availability preview (next 3 available slots)
- "Book Session" primary CTA
- "Send Message" secondary CTA (for async)

---

### FE-BOOK-001: Booking Flow

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | XL | 5 |

**Acceptance Criteria:**
- Step 1: Select date (calendar view, available days highlighted)
- Step 2: Select time slot (list of available slots for selected date)
- Step 3: Select session type (video, voice, async)
- Step 4: Review + payment
- Step 5: Confirmation screen with calendar add option
- Back button on every step without losing state
- Payment: embedded Paystack/Stripe form or redirect
- Session credit deduction shown if user has credits
- Coupon code input in payment step

**Component Structure:**
```
BookingFlow
├── BookingProgress (step indicator)
├── Step1_DatePicker (calendar with availability overlay)
├── Step2_TimeSlotPicker
├── Step3_SessionTypePicker
├── Step4_PaymentStep
│   ├── BookingSummary
│   ├── CouponCodeInput
│   ├── SessionCreditOption (if available)
│   └── PaymentWidget (Paystack/Stripe)
└── Step5_Confirmation
    ├── SuccessAnimation
    ├── BookingDetails
    └── AddToCalendarButton
```

**State:**
```ts
// store/bookingStore.ts
interface BookingState {
  coachId: string | null;
  selectedDate: Date | null;
  selectedSlot: TimeSlot | null;
  sessionType: SessionType | null;
  couponCode: string;
  useSessionCredit: boolean;
}
```

---

## AI Companion (Sera)

### FE-AI-001: AI Chat Interface

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | XL | 3–4 |

**Acceptance Criteria:**
- WhatsApp-style chat bubbles (user right, Sera left)
- Sera avatar (animated, subtle breathing effect)
- Message timestamps
- Typing indicator (3 animated dots)
- Streaming responses (text appears word by word via SSE)
- Error state with retry button
- Crisis message renders differently (highlighted, with resources)
- Conversation history loaded on open (last 20 messages)
- "Sera remembers our last conversation" indicator on first open
- Rate limit indicator: "X messages remaining today" (Free/Pro)
- Suggest actions: quick reply chips (e.g., "I want to journal", "Book a coach")
- Input: text area, send button, character count

**Component Structure:**
```
AICompanionPage
├── ConversationHeader (Sera avatar, name, clear button)
├── MessageList
│   ├── UserMessage
│   ├── AIMessage (with streaming support)
│   ├── CrisisMessage (special styling)
│   └── SystemMessage (typing indicator)
├── MessageInput
│   ├── TextArea (multiline, auto-resize)
│   ├── SendButton
│   └── QuickReplyChips
└── RateLimitBanner (Free/Pro tier)
```

**Streaming Implementation:**
```ts
// hooks/useAIStream.ts
export function useAIStream() {
  const [streamContent, setStreamContent] = useState('');
  const [isStreaming, setIsStreaming] = useState(false);

  const sendMessage = async (message: string) => {
    setIsStreaming(true);
    setStreamContent('');

    const response = await fetch('/api/v1/ai/conversations', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ message }),
    });

    const reader = response.body!.getReader();
    const decoder = new TextDecoder();

    while (true) {
      const { done, value } = await reader.read();
      if (done) break;
      setStreamContent(prev => prev + decoder.decode(value));
    }
    setIsStreaming(false);
  };

  return { streamContent, isStreaming, sendMessage };
}
```

---

## Session Management

### FE-SESS-001: Video Session Page

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | XL | 5–6 |

**Acceptance Criteria:**
- Full-screen video call interface
- Local video preview (bottom corner)
- Remote video (full screen)
- Controls: mute, camera off, end call, screen share
- "Poor connection" warning indicator
- Session timer (counts up from 00:00)
- Session notes for user (private, saved post-session)
- Waiting room if joining early (shows countdown)
- Post-session: immediate rating prompt

**Component Structure:**
```
VideoSessionPage
├── WaitingRoom (if early)
├── VideoGrid
│   ├── RemoteVideoTile (coach)
│   └── LocalVideoTile (user, corner)
├── SessionControls
│   ├── MuteButton (toggle)
│   ├── VideoButton (toggle)
│   ├── ScreenShareButton
│   └── EndCallButton
├── SessionTimer
├── ConnectionQualityIndicator
└── PostSessionModal (rating + notes)
```

---

## Community

### FE-COM-001: Community Feed

| Priority | Complexity | Sprint |
|---|---|---|
| P1 | L | 6–7 |

**Acceptance Criteria:**
- Topic tabs horizontally scrollable
- Feed sorted by: New, Trending, Top
- Post cards: author (or Anonymous), topic badge, excerpt, reactions, reply count
- Infinite scroll (cursor pagination)
- Create post FAB (floating action button, mobile)
- Filter: show only from followed topics
- Post type badges (Story, Question, Resource, Milestone)
- Reported/removed posts show placeholder (no jarring removals)

---

### FE-COM-002: Create Post Flow

| Priority | Complexity | Sprint |
|---|---|---|
| P1 | M | 7 |

**Acceptance Criteria:**
- Topic selector (required)
- Post type selector (story, question, resource, milestone)
- Rich text area (basic formatting)
- Anonymous toggle (with explanation of what "anonymous" means)
- Trigger warning option
- Character limit indicator (1000 chars)
- Image attachment (future state placeholder)
- Preview before publishing
- Loading state during submission

---

## Subscription & Payments

### FE-SUB-001: Subscription Plans Page

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 5–6 |

**Acceptance Criteria:**
- Monthly / Annual toggle (shows savings)
- Plan comparison table with feature checkmarks
- Current plan highlighted with "Current Plan" badge
- Upgrade CTA prominent; downgrade option in settings
- Pay with card (Paystack embed) or Stripe
- Countdown for annual plan savings
- Free trial messaging if applicable

**Component Structure:**
```
SubscriptionPage
├── BillingCycleToggle (monthly/annual)
├── PlanGrid
│   ├── PlanCard (Free)
│   ├── PlanCard (Pro) ← highlighted for most users
│   ├── PlanCard (Premium)
│   └── PlanCard (Executive)
├── FeatureComparisonTable
└── PaymentModal
    ├── OrderSummary
    └── PaystackButton / StripeButton
```

---

## Notifications Center

### FE-NOT-001: Notification Center

| Priority | Complexity | Sprint |
|---|---|---|
| P1 | M | 6 |

**Acceptance Criteria:**
- Bell icon in header with unread count badge
- Notification drawer (slides in from right on desktop, bottom sheet on mobile)
- Grouped by: Today, Yesterday, This Week, Older
- Each notification: icon (by type), title, body, timestamp, read/unread state
- Mark all as read button
- Click navigates to relevant context (e.g., community reply → opens post)
- Empty state: "You're all caught up!"

**Real-time Updates (SignalR):**
```ts
// hooks/useNotifications.ts
export function useNotifications() {
  const connection = useSignalRConnection('/hubs/notifications');
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);

  useEffect(() => {
    connection.on('NotificationReceived', (notification) => {
      setNotifications(prev => [notification, ...prev]);
      setUnreadCount(prev => prev + 1);
    });
  }, [connection]);

  return { notifications, unreadCount };
}
```

---

## Settings

### FE-SET-001: Settings Pages

| Priority | Complexity | Sprint |
|---|---|---|
| P1 | M | 7 |

**Sections:**
- Profile settings (photo, name, bio)
- Account settings (email, password, phone)
- Notification preferences (per type, per channel)
- Privacy settings (data sharing, community visibility)
- Subscription & billing (current plan, invoices, cancel)
- Connected apps (Google Calendar, etc.)
- Data export (GDPR)
- Danger zone (Delete account)

---

## Admin Dashboard

### FE-ADMIN-001: Admin Overview Dashboard

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | XL | 7–8 |

**Acceptance Criteria:**
- Key metrics tiles: MAU, DAU, Revenue (MTD), New Registrations, Active Sessions, Verification Queue Count
- Line charts: user growth (30d), revenue (30d)
- Quick actions: View Verification Queue, View Moderation Queue, Trigger Payout
- Recent activity log

**Sections:**
```
AdminDashboard
├── MetricsOverview (6 KPI tiles)
├── UserGrowthChart
├── RevenueChart
├── CoachVerificationQueue (action required)
├── ContentModerationQueue (action required)
├── SystemHealthStatus
└── RecentAuditLog
```

---

## Shared Components & Hooks

### Shared Components

| Component | Description |
|---|---|
| `<PageHeader>` | Page title, breadcrumb, action buttons |
| `<EmptyState>` | Illustration + message + optional CTA |
| `<SkeletonCard>` | Loading placeholder (matches card dimensions) |
| `<ConfirmationModal>` | Destructive action confirmation dialog |
| `<UpgradePrompt>` | Feature gate modal for free tier users |
| `<Toast>` | Success / error / info toasts (Sonner) |
| `<Pagination>` | Cursor-based and offset pagination controls |
| `<SearchInput>` | Debounced search with clear button |
| `<DateRangePicker>` | Calendar-based date range selection |
| `<FileUpload>` | Drag-and-drop + click file upload |
| `<LoadingOverlay>` | Full-page loading state |
| `<ConnectionStatus>` | Offline/poor connection indicator |

### Custom Hooks

| Hook | Description |
|---|---|
| `useAuth()` | Auth state, login, logout, token refresh |
| `useCurrentUser()` | Cached current user profile |
| `useSignalR(hubUrl)` | SignalR connection management |
| `useInfiniteScroll(query)` | Infinite scroll with IntersectionObserver |
| `useDebounce(value, delay)` | Debounce hook for search inputs |
| `useMoodData(range)` | Mood history data + derived insights |
| `useNotifications()` | Real-time notification state |
| `useSubscription()` | Current subscription state + feature gating |
| `useFeatureGate(feature)` | Check if feature available for user tier |
| `useMediaQuery(query)` | Responsive breakpoint hook |
| `useOffline()` | Network status detection |
| `useAnalytics()` | PostHog event tracking wrapper |

---

## State Management Strategy

### Zustand Stores

```ts
// store/authStore.ts
interface AuthStore {
  user: UserProfile | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  setTokens: (accessToken: string) => void;
  clearAuth: () => void;
}

// store/uiStore.ts
interface UIStore {
  sidebarOpen: boolean;
  theme: 'light' | 'dark' | 'auto';
  notificationDrawerOpen: boolean;
  toggleSidebar: () => void;
  setTheme: (theme: string) => void;
}

// store/bookingStore.ts — transient booking flow state
// store/onboardingStore.ts — transient onboarding state
```

### TanStack Query Config

```ts
// lib/queryClient.ts
export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000,        // 5 minutes
      gcTime: 10 * 60 * 1000,          // 10 minutes
      retry: 2,
      refetchOnWindowFocus: false,
    },
    mutations: {
      onError: (error) => toast.error(getErrorMessage(error)),
    },
  },
});
```

---

## Routing & Navigation

### Route Structure

```ts
// Navigation groups
const publicRoutes = ['/', '/login', '/register', '/coaches/{id}'];
const authRoutes = ['/dashboard', '/mood', '/journal', '/ai-companion', ...];
const coachRoutes = ['/coach/dashboard', '/coach/sessions', ...];
const adminRoutes = ['/admin/**'];

// Middleware (Next.js middleware.ts)
// Check authentication on protected routes
// Redirect to onboarding if not completed
// Redirect to appropriate dashboard based on role
```

---

## Accessibility Requirements

| Requirement | Implementation |
|---|---|
| WCAG 2.1 AA | All pages audited with axe-core |
| Keyboard navigation | All interactive elements focusable and operable via keyboard |
| Screen reader | ARIA labels on all non-text elements |
| Color contrast | 4.5:1 minimum ratio for normal text |
| Focus management | Modal focus trap; focus returns after modal close |
| Motion sensitivity | Respect `prefers-reduced-motion` for animations |
| Form errors | Error messages associated with fields via `aria-describedby` |
| Skip links | "Skip to main content" link at top of each page |

---

## Responsive Design

| Breakpoint | Tailwind Prefix | Layout |
|---|---|---|
| Mobile | default (< 640px) | Single column, bottom navigation |
| Tablet | `sm:` (640px+) | Two column where applicable |
| Laptop | `lg:` (1024px+) | Sidebar layout revealed |
| Desktop | `xl:` (1280px+) | Full sidebar + content area |

---

## Performance Requirements

| Metric | Target |
|---|---|
| Largest Contentful Paint (LCP) | < 2.5s |
| First Input Delay (FID) | < 100ms |
| Cumulative Layout Shift (CLS) | < 0.1 |
| Time to Interactive (TTI) | < 3.5s |
| JavaScript bundle size | < 200KB initial |
| Image optimization | Next.js `<Image>` component everywhere |

---

*End of Frontend Task Breakdown*  
*Next: [MOBILE_TASKS.md](./MOBILE_TASKS.md)*
