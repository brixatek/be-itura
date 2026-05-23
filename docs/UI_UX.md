# ITURA — UI/UX Design Strategy

**Document Version:** 1.0  
**Owner:** UI/UX Lead / Product Design  
**Last Updated:** May 2026

---

## Table of Contents

1. [Design Philosophy](#1-design-philosophy)
2. [Design System Strategy](#2-design-system-strategy)
3. [Color Psychology](#3-color-psychology)
4. [Typography System](#4-typography-system)
5. [Accessibility Guidelines](#5-accessibility-guidelines)
6. [Emotional UX Principles](#6-emotional-ux-principles)
7. [Wellness-Centered UI Concepts](#7-wellness-centered-ui-concepts)
8. [Retention-Focused UX](#8-retention-focused-ux)
9. [Gamification UX](#9-gamification-ux)
10. [Screen Design Specifications](#10-screen-design-specifications)

---

## 1. Design Philosophy

### Core Design Beliefs

**Itura's UI should feel like a warm hug, not a clinical form.**

Every design decision is measured against three questions:
1. Does this reduce emotional friction or increase it?
2. Does this feel safe and non-judgmental?
3. Does this encourage a positive daily habit?

### Design Principles

| Principle | What It Means | Example |
|---|---|---|
| **Calm over clever** | Prioritize clarity and peace over impressive complexity | Simple 5-emoji mood picker vs. complex slider |
| **Invisible effort** | The most valuable actions should require the least effort | Mood check-in: 3 taps maximum |
| **Progress, not perfection** | Celebrate showing up, not achievement | "7-day streak!" not "Only 7 days" |
| **Earned depth** | Surface simplicity; depth available when wanted | Basic mood log always; charts unlocked with data |
| **Culturally rooted** | Visuals and language reflect African wellness | African illustrations, local idioms, Afrocentric color warmth |
| **Forgiving by design** | Mistakes feel fixable; nothing feels permanent | Easy edit/delete; "Are you sure?" for destructive actions only |

---

## 2. Design System Strategy

### 2.1 Design Tokens

Design tokens are the single source of truth for all visual decisions:

```
SPACING
  --space-1:  4px
  --space-2:  8px
  --space-3:  12px
  --space-4:  16px
  --space-5:  20px
  --space-6:  24px
  --space-8:  32px
  --space-10: 40px
  --space-12: 48px
  --space-16: 64px

BORDER RADIUS
  --radius-sm:  4px    (inputs, tags)
  --radius-md:  8px    (cards, buttons)
  --radius-lg:  16px   (modals, panels)
  --radius-xl:  24px   (bottom sheets)
  --radius-full: 9999px (pills, avatars)

SHADOWS
  --shadow-sm:  0 1px 3px rgba(0,0,0,0.08)
  --shadow-md:  0 4px 12px rgba(0,0,0,0.10)
  --shadow-lg:  0 8px 24px rgba(0,0,0,0.12)
  --shadow-focus: 0 0 0 3px rgba(45,138,109,0.25)  (brand focus ring)

TRANSITIONS
  --transition-fast:   150ms ease
  --transition-base:   250ms ease
  --transition-slow:   400ms ease
  --transition-spring: 400ms cubic-bezier(0.34, 1.56, 0.64, 1)
```

### 2.2 Component Library

All components built as: accessible → interactive → themed

| Category | Components |
|---|---|
| **Foundation** | Button, IconButton, Link, Divider |
| **Forms** | Input, Textarea, Select, Checkbox, Radio, Switch, Slider, DatePicker |
| **Feedback** | Toast, Alert, Badge, Spinner, Skeleton, ProgressBar |
| **Navigation** | Navbar, Sidebar, BottomNav, Tabs, Breadcrumb, Pagination |
| **Overlay** | Modal, Drawer, BottomSheet, Tooltip, Popover |
| **Data Display** | Card, Avatar, Tag, Chip, Table, List |
| **Charts** | LineChart, BarChart, Heatmap, Sparkline |
| **Media** | ImageUpload, VideoPlayer, AudioPlayer |
| **Wellness-Specific** | MoodEmojiPicker, StreakBadge, XPProgressBar, WellnessCard, BreathingCircle |

### 2.3 Component States

Every interactive component must have defined states:
- **Default** — rest state
- **Hover** — desktop pointer hover
- **Focus** — keyboard/accessibility focus (visible focus ring)
- **Active** — pressed/click state
- **Loading** — async operation in progress
- **Disabled** — not interactive
- **Error** — validation failure
- **Success** — positive completion

---

## 3. Color Psychology

### 3.1 Primary Palette — Healing Green

Green is the primary brand color. In color psychology:
- Green = growth, healing, balance, renewal, nature
- In African contexts: green is associated with prosperity, life, and vitality

```
GREEN SCALE (Primary)
  green-50:  #f0faf5   ← background tints
  green-100: #d1f5e5
  green-200: #a7ebb1
  green-300: #6dd98f
  green-400: #47c274
  green-500: #2d8a6d   ← PRIMARY (main CTA, active states)
  green-600: #1f7059
  green-700: #165944
  green-800: #0f4233
  green-900: #0a2c22   ← dark text on light backgrounds
```

### 3.2 Emotional Color Palette

Each emotional state has an associated color used consistently across the platform:

| Emotion / State | Color | Hex | Usage |
|---|---|---|---|
| Joy / Happy | Amber | `#F5A623` | Mood score 5, achievements, celebration |
| Calm / Good | Sage Green | `#52A869` | Mood score 4, success states |
| Neutral | Slate Blue | `#8A9BB5` | Mood score 3, informational |
| Low / Sad | Dusty Blue | `#6B9FCA` | Mood score 2, low engagement |
| Distress | Warm Rose | `#E05252` | Mood score 1, crisis states, errors |
| Growth | Brand Green | `#2D8A6D` | Progress, streaks, achievements |
| Rest | Lavender | `#B09FD4` | Night mode, meditation, breathing |
| Energy | Warm Orange | `#F47B20` | Challenges, gamification |

### 3.3 Semantic Colors

```
SUCCESS:  #2D8A6D  (brand green — positive action, completion)
WARNING:  #F5A623  (amber — caution, attention needed)
ERROR:    #E05252  (warm rose — destructive, errors)
INFO:     #3B82F6  (blue — neutral information)
NEUTRAL:  #8A9BB5  (slate — secondary information)
```

### 3.4 Dark Mode Palette

Wellness apps have high night-time usage (users often process emotions at night). Dark mode is essential.

```
DARK MODE BACKGROUNDS
  background-primary:   #0F1117   (deep charcoal, easier on eyes than true black)
  background-secondary: #1A1D27   (cards, panels)
  background-tertiary:  #242836   (inputs, hover states)
  background-elevated:  #2D3041   (modals, drawers)

DARK MODE TEXT
  text-primary:   #F0F4F8   (high contrast)
  text-secondary: #A0A9BA   (secondary content)
  text-tertiary:  #636D7E   (placeholder, disabled)

DARK MODE BRAND
  primary-dark-mode: #4BAD8A  (slightly lighter green for dark backgrounds)
```

---

## 4. Typography System

### 4.1 Font Stack

| Usage | Font | Fallback |
|---|---|---|
| Primary (body, UI) | Inter | -apple-system, Roboto, sans-serif |
| Display (headings, emotional moments) | Playfair Display | Georgia, serif |
| Monospace (code, data) | JetBrains Mono | Consolas, monospace |

**Rationale:**
- **Inter:** Exceptional readability at small sizes; optimized for screens; supports Latin + extended characters for future localization
- **Playfair Display:** Serif warmth for emotional moments (welcome screens, affirmations, milestone celebrations) creates contrast and gravitas

### 4.2 Type Scale

```
DISPLAY (Playfair Display, emotional moments)
  display-xl:   48px / 1.1 lh / -0.5px ls
  display-lg:   36px / 1.2 lh / -0.3px ls
  display-md:   28px / 1.3 lh / -0.2px ls

HEADINGS (Inter, page/section titles)
  h1: 24px / 1.3 lh / -0.2px ls / Semibold (600)
  h2: 20px / 1.4 lh / -0.1px ls / Semibold (600)
  h3: 18px / 1.4 lh / 0px ls    / Medium (500)
  h4: 16px / 1.5 lh / 0px ls    / Medium (500)

BODY (Inter, content)
  body-lg: 16px / 1.6 lh / 0px ls   / Regular (400)
  body-md: 14px / 1.6 lh / 0px ls   / Regular (400)
  body-sm: 12px / 1.5 lh / 0.1px ls / Regular (400)

LABELS (Inter, UI elements)
  label-lg: 14px / 1 lh / 0.3px ls  / Medium (500)
  label-sm: 12px / 1 lh / 0.4px ls  / Medium (500)

CAPTIONS
  caption:  11px / 1.4 lh / 0.2px ls / Regular (400)
```

---

## 5. Accessibility Guidelines

### 5.1 WCAG 2.1 AA Compliance Targets

| Criterion | Target | Test Method |
|---|---|---|
| Color contrast (normal text) | ≥ 4.5:1 | axe-core, Colour Contrast Analyser |
| Color contrast (large text) | ≥ 3:1 | axe-core |
| Color contrast (UI components) | ≥ 3:1 | Manual inspection |
| Keyboard navigation | 100% operable | Manual keyboard test |
| Focus visible | Always visible (3px ring) | Manual inspection |
| Touch target size | ≥ 44×44px | Design check |
| Text resize to 200% | No content loss | Browser zoom test |
| Screen reader | Fully navigable | VoiceOver + TalkBack |
| Animation | Respect `prefers-reduced-motion` | CSS media query |
| Form errors | Programmatically associated | axe-core |

### 5.2 Accessibility by Component

**Buttons:**
```html
<!-- Always accessible button -->
<button
  type="button"
  aria-label="Log your mood"  ← descriptive label if no visible text
  aria-pressed="false"         ← for toggle buttons
  aria-disabled="false"
>
  Log Mood
</button>
```

**Form Fields:**
```html
<label for="mood-note">
  How are you feeling? (optional)
</label>
<textarea
  id="mood-note"
  aria-describedby="mood-note-hint mood-note-error"
  aria-invalid="false"
  maxlength="280"
/>
<span id="mood-note-hint">Your note is private and encrypted.</span>
<span id="mood-note-error" role="alert" aria-live="polite"></span>
```

**Mood Emoji Picker:**
```html
<fieldset>
  <legend>How are you feeling today?</legend>
  <div role="radiogroup" aria-required="true">
    <label>
      <input type="radio" name="mood" value="1" aria-label="Very Sad" />
      😢
    </label>
    <!-- ... 4 more options -->
  </div>
</fieldset>
```

### 5.3 Inclusive Design

- **Low literacy:** Icons always accompanied by labels; no icon-only buttons for primary actions
- **Low bandwidth:** Skeleton screens instead of spinning loaders; progressive image loading
- **Older devices:** Smooth 60fps targeted even on low-end Android (Tecno Spark class)
- **Visual impairment:** High contrast mode toggle in settings; respect OS high-contrast setting
- **Cognitive accessibility:** Simple language (Grade 8 reading level target); one primary action per screen

---

## 6. Emotional UX Principles

### 6.1 Emotional Design Framework

**Three levels of emotional design (Donald Norman):**

| Level | Itura Application |
|---|---|
| **Visceral** (appearance) | Soft, warm colors; rounded corners; nature-inspired illustrations; Lottie animations |
| **Behavioral** (usability) | Instant mood logging; predictable interactions; always recoverable actions |
| **Reflective** (meaning) | Streaks that feel earned; data that tells your story; journeys that build identity |

### 6.2 Micro-Interactions for Emotional Impact

| Interaction | Animation | Purpose |
|---|---|---|
| Mood selection | Emoji bounces + scales up | Physical confirmation of choice |
| Streak increment | Flame "grows" + orange particles | Dopamine reward moment |
| Journal save | Text "settles" into page with gentle fade | Completion feeling |
| Level up | Full-screen celebration burst | Major achievement recognition |
| Badge earned | Badge "stamps" onto screen | Collection pride |
| Session booking confirmed | Green checkmark draws in | Trust and relief |
| AI message received | Typing dots → message "slides in" | Natural conversation feel |
| Breathing exercise | Smooth expanding/contracting circle | Physiological calm cue |

### 6.3 Safe Space Language Guidelines

**Tone of Voice:**
- Warm, not clinical
- Encouraging, not prescriptive
- Curious, not interrogating
- Grounded, not spiritual bypass ("Everything happens for a reason")
- Acknowledging, not dismissive

**Word choices:**

| Avoid | Use Instead |
|---|---|
| "Mental illness" | "Mental health" or "emotional wellbeing" |
| "Disorder" | "Challenge" or "difficulty" |
| "Symptoms" | "Experiences" or "signs" |
| "You should..." | "Some people find it helpful to..." |
| "Normal" | "Common" |
| "Just" | (remove entirely — minimizes experience) |
| "I understand" (AI) | "That sounds..." / "It makes sense that..." |
| "Don't worry" | "I hear you" |

### 6.4 Failure State Design

Errors must never make users feel judged:

```
Payment failed:
  ❌ DON'T: "Your payment was declined. Please check your card details."
  ✅ DO:    "It looks like there was a hiccup with your payment.
             No charge was made. Let's try again? 
             [Try Again]  [Use a Different Card]"

Mood streak broken:
  ❌ DON'T: "Your streak has ended. Start again from Day 1."
  ✅ DO:    "Your streak took a pause — that's okay. Showing up imperfectly
             is still showing up. Let's start fresh today. 🌱
             [Log Today's Mood]"

Session canceled by coach:
  ❌ DON'T: "Your session has been canceled."
  ✅ DO:    "Dr. Obi had to reschedule your session. We're sorry for the
             inconvenience! Here are her next available times, or we can
             match you with another coach right away."
```

---

## 7. Wellness-Centered UI Concepts

### 7.1 Home Dashboard Design

The home dashboard is the user's "wellness mirror" — a daily snapshot that feels:
- **Welcoming** (not overwhelming)
- **Personalized** (speaks to you specifically)
- **Action-oriented** (clear next step)

```
Layout: Vertical scroll, card-based

Top Section:
  ┌────────────────────────────────────────┐
  │  Good morning, Amara ☀️               │
  │  Thursday, May 22                     │
  │                                       │
  │  🔥 7-day streak  ·  Level 3: Root    │
  └────────────────────────────────────────┘

Priority Card (if mood not logged):
  ┌────────────────────────────────────────┐
  │  How are you feeling today?            │
  │  😢  😔  😐  😊  😄                  │
  │         [Add a note]                  │
  └────────────────────────────────────────┘

Quick Actions (horizontal scroll):
  [🤖 Chat with Sera]  [📖 Journal]  [🗓 Book Session]

Upcoming Session (if any):
  ┌────────────────────────────────────────┐
  │  📹 Session in 2 hours                │
  │  Dr. Chinelo Obi · 2:00 PM today      │
  │  [Join]  [Reschedule]                 │
  └────────────────────────────────────────┘

7-Day Mood Sparkline:
  ┌────────────────────────────────────────┐
  │  Your week: mostly calm 😊            │
  │  ___/‾‾\__/‾‾‾‾\___                  │
  │  Mon Tue Wed Thu Fri Sat Sun          │
  └────────────────────────────────────────┘

Today's Insight (AI):
  "You tend to feel better on days you journal. 
   Want to write something today?"  [Journal Now]
```

### 7.2 Breathing Exercise UI

The breathing exercise is one of the most emotionally impactful features. Design requirements:

```
Full-screen, immersive experience:
  Background: Gradient transitions (deep teal → soft lavender → deep teal)
  
  Center: Animated circle
    ├── Inhale: circle expands over 4 seconds (scale 0.4 → 1.0)
    ├── Hold: circle pulses gently (scale 1.0, subtle opacity pulse)
    ├── Exhale: circle contracts over 6 seconds (scale 1.0 → 0.4)
    └── Smooth: all transitions use ease-in-out curves
  
  Text instruction: "Breathe in..." / "Hold..." / "Breathe out..."
    ├── Playfair Display, large, centered
    └── Fades in/out with circle animation
  
  Timer: subtle countdown (not prominent — keep focus on breathing)
  
  Sound (optional): ambient rain/ocean audio
  
  Exit: gentle "X" in corner; no confirmation needed
  
  Completion: gentle "You did it" message + XP notification
```

### 7.3 Journal Editor Design

```
Clean, distraction-free writing environment:

  Minimal toolbar (only shows when text selected):
    Bold  Italic  • List  H1  H2  "Quote"
  
  AI prompt chip (top):
    💡 "What small thing brought you comfort today?"
    [Tap to use prompt]  [Show me another]
  
  Editor area:
    ├── Generous padding (24px)
    ├── Wide line spacing (1.8)
    ├── Soft parchment background (not stark white) → #FAFAF7
    └── Autofocus on open
  
  Bottom bar:
    Word count: 247 words
    [🏷 Tag emotions]  [🔒 Private]  [Save Draft]
  
  Auto-save indicator: "Saved just now ✓"
```

---

## 8. Retention-Focused UX

### 8.1 Onboarding-to-Habit Design

The first 7 days determine long-term retention. Design for habit formation:

**Day 1:** Focus on ONE action — mood check-in. Don't overwhelm with features.
```
Post-registration flow:
  Welcome → Meet Sera (1 conversation) → Log first mood → Dashboard
  
  NOT: feature tour of 10 things
```

**Day 2–3:** Introduce journaling after mood is established.
**Day 4–5:** Recommend first coaching session (now they're invested).
**Day 7:** Celebrate 7-day streak with meaningful reward (custom AI companion name unlock).

### 8.2 Re-engagement Moment Design

When users return after absence:

```
2-day absence:
  "We missed you, Amara. How have you been? 
   Your last mood was Neutral 😐 on Monday."
  [Log Today's Mood]

7-day absence:
  New landing experience (not normal home dashboard):
  "It's been a week — how are things? Let's catch up."
  Simplified view: just mood picker + Sera chat CTA
  → Reduce friction to re-enter

30-day absence:
  "Your wellbeing journey is still here, whenever you're ready.
   No pressure. We saved everything."
  → Warm, zero-pressure re-engagement
```

### 8.3 Progress Visibility

Users stay when they see progress. Design surfaces that show it:

| Surface | Where | Motivation Type |
|---|---|---|
| Streak flame on home | Dashboard, always visible | Loss aversion (don't break it) |
| XP progress bar | Profile tab | Progress to next level |
| Mood chart (7 days) | Dashboard sparkline | Trend visibility |
| Milestone celebrations | Full-screen, on achievement | Celebration moment |
| Weekly summary | Sunday email + push | Reflection + pride |
| Badge collection | Profile page | Collection psychology |
| Level name display | Header/profile | Identity ("I'm a Bloom") |

---

## 9. Gamification UX

### 9.1 Gamification Design Principles

**Wellness gamification ≠ game gamification.**

Rules:
- Rewards celebrate consistency, not performance (logging any mood is rewarded equally)
- No competitive pressure between users (no public leaderboard showing "you're ranked 847th")
- Opt-in social features (leaderboards only visible to users who opt in)
- Progression feels earned, not forced
- "Catching up" after absence should feel possible, not punitive

### 9.2 Streak UX Design

```
Streak = flame icon + number

Visual states:
  0 days:   Gray flame (dormant)
  1–3 days: Small orange flame
  4–7 days: Medium golden flame
  8–14 days: Large bright flame
  15–30 days: Flame with aura
  31–99 days: Pulsing flame
  100+ days: Legendary flame (rainbow animated)

Streak at risk (not logged by 7pm):
  Push notification: "🔥 Your 12-day streak is at risk! Log your mood before midnight."
  In-app banner: gentle amber banner on home screen

Streak broken:
  → Animate flame going out (gentle, not dramatic)
  → Show message: "Your streak paused at 12 days. That's okay. 🌱"
  → Show "Start Fresh" CTA
  → Show longest streak as consolation: "Your personal best: 12 days"
```

### 9.3 Badge Presentation

```
Badge earn moment:
  1. Full-screen overlay (semi-transparent)
  2. Badge "stamps" in from above with physics bounce
  3. Particle burst effect (color matches badge)
  4. Sound cue (satisfying chime — optional, respects DND)
  5. Badge name + description displayed
  6. [Add to Profile]  [Share]  [Continue]

Badge collection page:
  Grid of all possible badges
  Earned: Full color, with earn date
  Unearned: Grayed out with "?" overlay + hint of requirement
  (creates completion psychology — users want to fill the grid)
```

### 9.4 Level-Up UX

```
Level up trigger:
  XP crosses threshold mid-session
  
  Animation sequence:
  1. XP bar fills and overflows (gold particle trail)
  2. Screen flashes white briefly
  3. Level counter increments with bounce
  4. New level name fades in (large, Playfair Display)
  5. New perks revealed:
     "Level 5: Bloom 🌸
      You've unlocked:
      ✓ 10% discount on your next session
      ✓ Access to the premium content library
      ✓ Custom journal cover designs"
  6. [Claim Rewards]  [Continue]
```

---

## 10. Screen Design Specifications

### 10.1 Spacing System Application

```
Page layout:
  ├── Page horizontal padding:  24px (mobile) / 32px (tablet) / 48px (desktop)
  ├── Section spacing:          32px vertical between major sections
  ├── Card internal padding:    20px
  └── Form field spacing:       16px between form elements

Navigation:
  ├── Bottom navigation height: 64px (mobile)
  ├── Sidebar width:            240px (desktop)
  └── Top header height:        60px
```

### 10.2 Icon System

**Icon Library:** Lucide Icons (consistent stroke-based style)  
**Custom icons:** Wellness-specific icons (mood emojis, breathing circle, wellness level badges) designed in brand style

Icon sizes:
- Navigation: 24px
- Inline (with text): 16px  
- Feature icons (cards): 32px
- Illustrative: 48–96px (Lottie for animated)

### 10.3 Illustration Style

Illustrations used throughout: onboarding, empty states, achievements, breathing exercises.

**Style Guide:**
- Afrocentric characters (diverse skin tones, hair styles, features)
- Soft, rounded shapes (not sharp/geometric)
- Warm color palette (aligned to brand)
- No clinical imagery (no hospitals, syringes, medical equipment)
- Scenes from everyday African life (markets, homes, nature, community)
- Lottie format for animated illustrations (breathing, celebrations, welcome)

---

## Summary: UX Quality Checklist

Before any screen goes to production:

**Emotional Quality:**
- [ ] Does this feel calm and welcoming?
- [ ] Is the language non-judgmental and empowering?
- [ ] Are error states kind and actionable?

**Accessibility:**
- [ ] Color contrast ≥ 4.5:1 for all text
- [ ] All interactive elements keyboard-accessible
- [ ] Screen reader tested (VoiceOver on iOS, TalkBack on Android)
- [ ] Touch targets ≥ 44px

**Performance:**
- [ ] LCP < 2.5s (web)
- [ ] No layout shift (CLS < 0.1)
- [ ] Animations respect `prefers-reduced-motion`

**Retention:**
- [ ] Primary action obvious and accessible in ≤ 2 taps/clicks
- [ ] Progress visible where relevant
- [ ] Empty states have a clear, encouraging next action

---

*End of UI/UX Strategy Document*

---

*This completes the full Itura Software Delivery Package.*  
*Return to [README.md](./README.md) for the full documentation index.*
