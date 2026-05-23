# ITURA — Flutter Mobile App Engineering Tasks

**Document Version:** 1.0  
**Owner:** Mobile Engineering Lead  
**Last Updated:** May 2026  
**Stack:** Flutter 3.x · Dart · BLoC · go_router · Dio · Hive

---

## App Architecture

### Architecture Pattern: Clean Architecture + BLoC

```
lib/
├── core/
│   ├── di/                     # get_it dependency injection
│   │   └── injection.dart
│   ├── network/
│   │   ├── api_client.dart     # Dio client with interceptors
│   │   ├── auth_interceptor.dart
│   │   └── error_interceptor.dart
│   ├── storage/
│   │   ├── secure_storage.dart # flutter_secure_storage
│   │   └── local_storage.dart  # Hive boxes
│   ├── router/
│   │   └── app_router.dart     # go_router config
│   ├── theme/
│   │   ├── app_theme.dart
│   │   ├── colors.dart
│   │   └── typography.dart
│   ├── error/
│   │   ├── failures.dart
│   │   └── exceptions.dart
│   └── utils/
│       ├── validators.dart
│       └── extensions.dart
│
├── features/
│   ├── auth/
│   │   ├── data/
│   │   │   ├── datasources/    # remote & local data sources
│   │   │   ├── models/         # JSON-serializable models
│   │   │   └── repositories/   # repository implementations
│   │   ├── domain/
│   │   │   ├── entities/       # pure Dart entities
│   │   │   ├── repositories/   # abstract interfaces
│   │   │   └── usecases/       # business logic
│   │   └── presentation/
│   │       ├── bloc/           # BLoC event/state/bloc
│   │       ├── pages/
│   │       └── widgets/
│   │
│   ├── mood/
│   ├── journal/
│   ├── ai_companion/
│   ├── booking/
│   ├── sessions/
│   ├── community/
│   ├── notifications/
│   ├── subscription/
│   └── profile/
│
└── shared/
    ├── widgets/                # Reusable UI components
    ├── bloc/                   # Global BLoC (auth state, theme)
    └── constants/
```

---

## EPIC 1 — App Architecture & Foundation

### MOB-ARCH-001: Project Setup & Architecture Foundation

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | L | 1 |

**Subtasks:**
1. Create Flutter project with proper package naming (`com.itura.app`)
2. Set up `get_it` dependency injection container
3. Configure `Dio` HTTP client with:
   - Base URL configuration per environment (dev/staging/prod)
   - `AuthInterceptor`: auto-attach JWT to all requests
   - `RefreshTokenInterceptor`: silent token refresh on 401
   - `ErrorInterceptor`: standardize error objects
   - Logging interceptor (debug builds only)
4. Set up `go_router` with:
   - Public routes (auth, landing)
   - Protected routes (redirect to login if unauthenticated)
   - Deep link handling
   - Shell routes (persistent bottom navigation)
5. Set up Hive for local database (offline support)
6. Set up `flutter_secure_storage` for sensitive data
7. Configure `flutter_native_splash` (branded splash)
8. Configure app icons (iOS + Android via `flutter_launcher_icons`)
9. Set up `flutter_flavors` or `dart-define` for env config
10. Configure linter rules (`analysis_options.yaml` with strict mode)

---

### MOB-ARCH-002: Network Layer

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 1 |

**API Client Setup:**
```dart
// core/network/api_client.dart
class ApiClient {
  final Dio _dio;

  ApiClient(this._dio) {
    _dio.options = BaseOptions(
      baseUrl: AppConfig.apiBaseUrl,
      connectTimeout: const Duration(seconds: 30),
      receiveTimeout: const Duration(seconds: 60),
      headers: {'Accept': 'application/json'},
    );

    _dio.interceptors.addAll([
      AuthInterceptor(_secureStorage),
      ErrorInterceptor(),
      if (kDebugMode) LogInterceptor(responseBody: true),
    ]);
  }
}
```

**Auth Interceptor:**
```dart
// Attaches Bearer token; on 401, attempts silent refresh
class AuthInterceptor extends Interceptor {
  @override
  void onRequest(options, handler) async {
    final token = await _secureStorage.getAccessToken();
    if (token != null) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    handler.next(options);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    if (err.response?.statusCode == 401) {
      final refreshed = await _refreshToken();
      if (refreshed) {
        return handler.resolve(await _retry(err.requestOptions));
      }
    }
    handler.next(err);
  }
}
```

---

### MOB-ARCH-003: State Management Setup (BLoC)

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 1 |

**Global BLoCs registered at app root:**
```dart
// AuthBloc: global auth state, token management
// ThemeBloc: light/dark mode
// NotificationBloc: real-time notification state
// ConnectivityBloc: online/offline state
```

**BLoC Pattern Example:**
```dart
// features/mood/presentation/bloc/mood_bloc.dart
class MoodBloc extends Bloc<MoodEvent, MoodState> {
  final LogMoodUseCase _logMood;
  final GetMoodHistoryUseCase _getMoodHistory;

  MoodBloc({required LogMoodUseCase logMood, required GetMoodHistoryUseCase getMoodHistory})
      : _logMood = logMood, _getMoodHistory = getMoodHistory, super(MoodInitial()) {
    on<LogMoodEvent>(_onLogMood);
    on<GetMoodHistoryEvent>(_onGetMoodHistory);
  }

  Future<void> _onLogMood(LogMoodEvent event, Emitter<MoodState> emit) async {
    emit(MoodLogging());
    final result = await _logMood(event.params);
    result.fold(
      (failure) => emit(MoodError(failure.message)),
      (entry) => emit(MoodLogged(entry)),
    );
  }
}
```

---

### MOB-ARCH-004: Router Setup

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 1 |

**Route Configuration:**
```dart
// core/router/app_router.dart
final router = GoRouter(
  initialLocation: '/splash',
  refreshListenable: authBloc.stream,
  redirect: (context, state) {
    final isAuthenticated = authBloc.state is Authenticated;
    final isAuthRoute = state.matchedLocation.startsWith('/auth');
    final isOnboardingDone = authBloc.state.user?.onboardingCompleted ?? false;

    if (!isAuthenticated && !isAuthRoute) return '/auth/login';
    if (isAuthenticated && isAuthRoute) return '/dashboard';
    if (isAuthenticated && !isOnboardingDone) return '/onboarding';
    return null;
  },
  routes: [
    GoRoute(path: '/splash', builder: (_, __) => const SplashPage()),
    GoRoute(path: '/auth/login', builder: (_, __) => const LoginPage()),
    GoRoute(path: '/auth/register', builder: (_, __) => const RegisterPage()),
    GoRoute(path: '/onboarding', builder: (_, __) => const OnboardingPage()),
    ShellRoute(
      builder: (_, __, child) => MainLayout(child: child),
      routes: [
        GoRoute(path: '/dashboard', builder: (_, __) => const DashboardPage()),
        GoRoute(path: '/mood', builder: (_, __) => const MoodPage()),
        GoRoute(path: '/journal', builder: (_, __) => const JournalPage()),
        GoRoute(path: '/ai-companion', builder: (_, __) => const AICompanionPage()),
        GoRoute(path: '/coaches', builder: (_, __) => const CoachDiscoveryPage()),
        GoRoute(path: '/community', builder: (_, __) => const CommunityPage()),
      ],
    ),
  ],
);
```

---

## EPIC 2 — Authentication

### MOB-AUTH-001: Login & Registration Screens

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 1–2 |

**Subtasks:**
1. `SplashPage`: check stored token → navigate appropriately
2. `LoginPage`: email + password form with validation
3. `RegisterPage`: multi-field registration form
4. `VerifyEmailPage`: OTP input with auto-submit on 6 digits
5. Google Sign-In via `google_sign_in` package
6. Apple Sign-In via `sign_in_with_apple` (iOS only)
7. Form validation with `flutter_form_builder` + `form_builder_validators`
8. Show/hide password toggle
9. Remember me checkbox (extends session to 7 days)

---

### MOB-AUTH-002: Secure Token Storage

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | S | 1 |

**Subtasks:**
1. `SecureStorageService` wrapping `flutter_secure_storage`
2. Store access token + refresh token in encrypted storage
3. Store user profile snapshot in Hive for offline display
4. Token refresh on app foreground: check expiry, refresh silently
5. Clear all secure data on logout

```dart
class SecureStorageService {
  static const _accessTokenKey = 'access_token';
  static const _refreshTokenKey = 'refresh_token';

  Future<void> saveTokens({required String accessToken, required String refreshToken}) async {
    await _storage.write(key: _accessTokenKey, value: accessToken);
    await _storage.write(key: _refreshTokenKey, value: refreshToken);
  }

  Future<void> clearAll() async => await _storage.deleteAll();
}
```

---

### MOB-AUTH-003: Biometric Authentication

| Priority | Complexity | Sprint |
|---|---|---|
| P1 | M | 3 |

**Subtasks:**
1. Check biometric availability on device (`local_auth` package)
2. Settings option: "Use Face ID / Fingerprint to login"
3. On app launch (if biometric enabled): prompt biometric → load stored token
4. Biometric bypass: show PIN option if biometric fails
5. Disable biometric option in settings
6. Re-authenticate with biometric for sensitive actions (payments, account deletion)

---

## EPIC 3 — Onboarding

### MOB-ONB-001: Onboarding Wizard

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | L | 2 |

**Subtasks:**
1. `PageView` with custom swipe-controlled transitions
2. Progress indicator (animated dots)
3. Lottie animation on welcome screen
4. Wellness goals: `Wrap` widget with selectable chips
5. Assessment: animated question-by-question display
6. Meet Sera: animated Sera avatar + typewriter first message
7. Persist progress: if app closed mid-onboarding, resume at last step
8. Submit onboarding data to API on completion

---

## EPIC 4 — Daily Wellness (Mood & Journal)

### MOB-MOOD-001: Mood Check-In

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 2 |

**Subtasks:**
1. Full-screen mood selection UI with haptic feedback on selection
2. Emoji row with scale-up animation on selection
3. Optional note: expandable text field
4. Trigger tags: horizontal scrollable chips
5. Submit with success confetti animation (`confetti` package)
6. Offline support: save to Hive if no connection, sync when online
7. Widget home screen (future): Flutter `home_widget` package

---

### MOB-MOOD-002: Mood History Charts

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 3 |

**Subtasks:**
1. `fl_chart` line chart with 7/30/90 day toggle
2. Custom tooltip on data point tap (shows note + tags)
3. Mood calendar heatmap (custom widget: `TableCalendar` with color coding)
4. Animated chart rendering on data load
5. Insights card below chart (if AI insights available)

---

### MOB-JRN-001: Journal Editor

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | L | 3 |

**Subtasks:**
1. `flutter_quill` rich text editor integration
2. AI prompt chips above editor (tap to insert)
3. Emotion tag bottom sheet
4. Auto-save: debounce 30 seconds, save to Hive + sync to API
5. Offline drafts: stored in Hive, uploaded when online
6. Character/word count display
7. Privacy lock icon (tap → info modal explaining encryption)
8. Coach share toggle with confirmation dialog

---

## EPIC 5 — AI Companion

### MOB-AI-001: Sera Chat Interface

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | XL | 3–4 |

**Subtasks:**
1. Chat bubble layout (`ListView.builder` reverse: true for bottom-up)
2. User bubble (right-aligned, brand color)
3. Sera bubble (left-aligned, white/card, Sera avatar beside)
4. Streaming text: character-by-character animation using `AnimatedTextKit`
5. Typing indicator: 3-dot pulse animation
6. Crisis message: distinct card with red border + emergency resources
7. Quick reply chip row (horizontally scrollable)
8. Text input: multi-line `TextField`, auto-expands, "Send" button
9. Haptic feedback on message send and receive
10. Conversation history: load last 20 messages on open
11. Rate limit display: `RateLimitBanner` widget
12. SSE streaming via `Dio` stream + `EventSource` alternative via `eventsource` package

---

## EPIC 6 — Coach & Booking

### MOB-COACH-001: Coach Discovery

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | L | 4 |

**Subtasks:**
1. Coach list: `ListView.builder` with `CoachCard` widget
2. Search bar with debounced API call (300ms)
3. Filter bottom sheet: draggable `DraggableScrollableSheet`
4. Specialty filter: `FilterChip` multi-select
5. Price range slider: `RangeSlider`
6. Infinite scroll: detect scroll near end → load next page
7. Skeleton loading (shimmer effect via `shimmer` package)
8. Empty state illustration

---

### MOB-BOOK-001: Booking Flow

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | XL | 5 |

**Subtasks:**
1. Multi-step bottom sheet or separate pages with `go_router`
2. Date picker: `TableCalendar` with available days highlighted
3. Time slot grid: wrap of selectable time chips
4. Session type: icon + label radio selection
5. Payment: in-app Paystack SDK or WebView for Paystack hosted page
6. Confirmation: animated success screen with booking details
7. Add to Calendar: `add_2_calendar` package for native calendar integration
8. Handle payment failure gracefully with retry option

---

## EPIC 7 — Video Sessions

### MOB-SESS-001: Video Session Interface

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | XL | 5–6 |

**Subtasks:**
1. Agora RTC Engine integration (`agora_rtc_engine`)
2. Token fetching from `session-service` before joining
3. Local video preview (small, corner)
4. Remote video (full screen)
5. Controls overlay (tap screen to show/hide):
   - Mute audio
   - Toggle camera
   - Flip camera (front/back)
   - End call
6. Session timer display
7. Connection quality indicator (network bars)
8. Waiting room page (if joining early): countdown timer
9. Background audio continuation (voice-only mode)
10. Post-session rating bottom sheet (slides up after call ends)
11. Request permission handling: camera + microphone
12. Handle call state in background (ongoing notification)

---

## EPIC 8 — Community

### MOB-COM-001: Community Feed

| Priority | Complexity | Sprint |
|---|---|---|
| P1 | L | 6–7 |

**Subtasks:**
1. Topic tab bar (horizontal scroll, `TabBar` + `TabBarView`)
2. Post card: author avatar, anonymous name, content excerpt, reactions
3. Pull-to-refresh (`RefreshIndicator`)
4. Infinite scroll
5. FAB: "Create Post" (only visible when scrolled up)
6. Reaction long-press menu (4 reaction types)
7. Report post: `showModalBottomSheet` with reason selection

---

## EPIC 9 — Notifications

### MOB-NOT-001: Push Notifications (Firebase Cloud Messaging)

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 6 |

**Subtasks:**
1. `firebase_messaging` integration
2. Request permission on first relevant action (not on cold open)
3. Handle foreground notifications: show custom in-app banner
4. Handle background/terminated notifications: deep link on tap
5. Handle notification tap: `go_router` deep link to relevant screen
6. Save device token to backend on login
7. Refresh token on change and update backend
8. `flutter_local_notifications` for local scheduled notifications (mood nudge at set time)

---

### MOB-NOT-002: Scheduled Local Notifications

| Priority | Complexity | Sprint |
|---|---|---|
| P1 | M | 6 |

**Subtasks:**
1. Daily mood reminder: scheduled using `flutter_local_notifications` with timezone support
2. Streak at-risk alert: 7pm local time if no activity
3. Session reminder: 1hr before booked session
4. User-configurable times (from Settings → Preferences)
5. Cancel/reschedule notifications when preferences change
6. Handle exact alarm permissions (Android 12+)

---

## EPIC 10 — Offline Support

### MOB-OFF-001: Offline-First Architecture

| Priority | Complexity | Sprint |
|---|---|---|
| P1 | XL | 7–8 |

**Strategy:**
- **Online-first** with offline fallback (not offline-first)
- Cache read for: user profile, mood history, journal entries, coach list
- Queue writes for: mood logs, journal drafts
- Show stale data with "last updated" timestamp when offline

**Subtasks:**
1. `ConnectivityBloc`: monitors network status (connectivity_plus package)
2. Show persistent banner when offline: "You're offline — some features unavailable"
3. Hive boxes:
   - `moodBox`: last 30 mood entries
   - `journalBox`: all journal drafts + last 20 entries
   - `userBox`: user profile snapshot
   - `queueBox`: pending write operations
4. `SyncService`: on reconnect, flush `queueBox` to API in order
5. Conflict resolution: last-write-wins for mood + journal
6. Mood log offline: save to `moodBox` + `queueBox` → sync on reconnect
7. Journal draft offline: save to Hive every 30s → sync on reconnect
8. Optimistic UI: show success immediately, revert if sync fails

---

## EPIC 11 — Performance Optimization

### MOB-PERF-001: Performance Baseline

| Priority | Complexity | Sprint |
|---|---|---|
| P1 | L | 8 |

**Subtasks:**
1. Profile app with Flutter DevTools (frame rate, memory, CPU)
2. Target: 60fps on mid-range Android (Tecno Spark equivalent)
3. `ListView.builder` everywhere (never `ListView` with children for long lists)
4. `const` constructors on all stateless widgets
5. Image caching: `cached_network_image` for all remote images
6. Lottie animations: preload and cache
7. Minimize rebuilds: `BlocSelector` instead of `BlocBuilder` where possible
8. App size optimization:
   - `--split-debug-info` for smaller APK
   - `--obfuscate` for release builds
   - Trim unused assets and packages
9. ProGuard rules for Android release
10. Cold start time target: < 3 seconds on mid-range Android

---

## EPIC 12 — Deep Linking

### MOB-DEEP-001: Deep Link Configuration

| Priority | Complexity | Sprint |
|---|---|---|
| P1 | M | 7 |

**Subtasks:**
1. Android: configure `intent-filter` in `AndroidManifest.xml`
2. iOS: configure `Associated Domains` + `Info.plist` for Universal Links
3. `go_router` deep link support: all routes accessible via URL
4. Test links:
   - `itura://ai-companion` → Open Sera
   - `itura://coaches/{id}` → Open coach profile
   - `itura://sessions/{id}` → Join session
   - `itura://community/posts/{id}` → Open post
5. Email verification deep link (web → mobile handoff)
6. Marketing deep links with UTM parameter tracking

---

## EPIC 13 — Security

### MOB-SEC-001: Mobile Security Hardening

| Priority | Complexity | Sprint |
|---|---|---|
| P0 | M | 8 |

**Subtasks:**
1. Certificate pinning: pin Itura API certificate (`dio` with pinned certificates)
2. Screenshot prevention on sensitive screens (journal, payment, session)
3. Jailbreak/root detection (`flutter_jailbreak_detection`)
4. Clear sensitive data from memory after use
5. Obfuscation in release builds
6. No sensitive data in logs (strip in release)
7. Biometric authentication for re-authentication on sensitive actions
8. Auto-lock: lock app after 5 minutes background (show biometric/PIN prompt on resume)

---

## Screen Inventory

| Screen | Route | Priority |
|---|---|---|
| Splash Screen | `/splash` | P0 |
| Login | `/auth/login` | P0 |
| Register | `/auth/register` | P0 |
| Email Verification | `/auth/verify-email` | P0 |
| Forgot Password | `/auth/forgot-password` | P0 |
| Onboarding Wizard | `/onboarding` | P0 |
| Dashboard | `/dashboard` | P0 |
| Mood Check-In | `/mood/check-in` | P0 |
| Mood History | `/mood/history` | P0 |
| Journal List | `/journal` | P0 |
| Journal Editor | `/journal/editor` | P0 |
| AI Companion (Sera) | `/ai-companion` | P0 |
| Coach Discovery | `/coaches` | P0 |
| Coach Profile | `/coaches/:id` | P0 |
| Booking Flow | `/coaches/:id/book` | P0 |
| My Bookings | `/bookings` | P0 |
| Video Session | `/sessions/:id/video` | P0 |
| Community Feed | `/community` | P1 |
| Community Post Detail | `/community/posts/:id` | P1 |
| Create Post | `/community/create` | P1 |
| Subscription Plans | `/subscription` | P0 |
| Notification Center | `/notifications` | P1 |
| Profile | `/profile` | P0 |
| Settings | `/settings` | P0 |
| Settings > Preferences | `/settings/preferences` | P0 |
| Settings > Privacy | `/settings/privacy` | P0 |
| Settings > Notifications | `/settings/notifications` | P0 |
| Settings > Billing | `/settings/billing` | P0 |

---

## Flutter Package Dependencies

```yaml
# pubspec.yaml (key dependencies)
dependencies:
  flutter_bloc: ^8.1.3
  get_it: ^7.6.4
  go_router: ^13.2.0
  dio: ^5.4.0
  flutter_secure_storage: ^9.0.0
  hive_flutter: ^1.1.0
  local_auth: ^2.1.7
  firebase_messaging: ^14.7.10
  flutter_local_notifications: ^16.3.0
  agora_rtc_engine: ^6.3.1
  signalr_netcore: ^1.3.5
  google_sign_in: ^6.1.6
  sign_in_with_apple: ^5.0.0
  image_picker: ^1.0.4
  cached_network_image: ^3.3.0
  fl_chart: ^0.67.0
  lottie: ^2.7.0
  shimmer: ^3.0.0
  table_calendar: ^3.0.9
  flutter_quill: ^9.3.4
  connectivity_plus: ^5.0.2
  add_2_calendar: ^2.2.3
  intl: ^0.18.1
  package_info_plus: ^5.0.1
  device_info_plus: ^9.1.1
  flutter_native_splash: ^2.3.6
  confetti: ^0.7.0
  share_plus: ^7.2.1

dev_dependencies:
  build_runner: ^2.4.7
  hive_generator: ^2.0.1
  json_serializable: ^6.7.1
  bloc_test: ^9.1.5
  mocktail: ^1.0.1
  flutter_launcher_icons: ^0.13.1
```

---

## Testing Strategy

### Unit Tests
- All Use Cases: cover happy path + all failure modes
- All BLoC: use `bloc_test` package, cover all events and state transitions
- Validators: test all validation rules
- Utility functions

### Widget Tests
- All form screens: test validation, submission, error display
- All list widgets: test empty state, loading state, populated state
- Navigation: test routing logic

### Integration Tests
- Full login flow
- Mood check-in flow
- Booking flow (mock payment)
- Offline → online sync flow

### Coverage Target
- Unit + Widget: ≥ 80% coverage
- Integration: critical user paths

---

*End of Mobile App Task Breakdown*  
*Next: [API_DESIGN.md](./API_DESIGN.md)*
