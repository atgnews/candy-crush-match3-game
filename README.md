# Candy Crush Match-3 Puzzle Game

A complete Candy Crush-style match-3 puzzle game for Android and iOS built with Unity 2022 LTS.

## 🎮 Game Features

### Core Mechanics
- **8x8 Grid System** with 6 candy types (Red, Orange, Yellow, Green, Blue, Purple)
- **Drag/Swipe Controls** for swapping adjacent candies
- **Match Detection** (3, 4, 5+ combos)
- **Special Candies**:
  - Striped Candy (4-match) - clears row/column
  - Wrapped Candy (L-shape) - creates explosion
  - Color Bomb (5-match) - clears all matching colors
- **Gravity System** with cascading combos
- **Combo Multiplier** with score scaling

### Level System
- **500+ Levels** organized in 25+ worlds
- **20 levels per world** with difficulty curve
- **Multiple Level Types**:
  - Jelly Levels (clear jellies)
  - Ingredient Levels (collect ingredients)
  - Timed Levels (race against clock)
  - Order Levels (collect specific items)
  - Monkling Levels (boss fights)
- **Level Goals**: Move limits, target scores, 1-3 star ratings
- **Blockers**: Chocolate, Licorice, Marmalade

### Player Systems
- **Individual Tasks**: Daily, Weekly, Achievement-based
- **Team System**: Create/join teams (max 30 players), team chat
- **Group Tasks**: Collaborative team challenges
- **Leaderboards**: Global, Friends, Level-specific, Weekly
- **Competitive Events**: Sugar Rush Tournament, Championship League, Live Duel Mode

### Monetization
- **In-App Purchases**: Coins, Lives, Booster Bundles, Premium Subscription
- **AdMob Integration**: Rewarded, Interstitial, Banner ads
- **Virtual Economy**: Coins (soft) & Gems (hard currency)
- **Season Pass**: 30-day premium track

### Engagement Features
- **Daily Login Streaks** with escalating rewards
- **Lives System**: 5 max, 1 refill every 30 minutes
- **Push Notifications** for re-engagement
- **Limited Time Events** (every 2 weeks, 3-5 days)
- **Social Features**: Send/receive lives, team chat, leaderboards

## 📁 Project Structure

```
candy-crush-match3-game/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/
│   │   ├── Gameplay/
│   │   ├── UI/
│   │   ├── Network/
│   │   ├── Analytics/
│   │   └── Ads/
│   ├── Scenes/
│   ├── Prefabs/
│   ├── Sprites/
│   ├── Audio/
│   └── Resources/
├── Packages/
├── ProjectSettings/
├── Firebase/
│   ├── Functions/
│   └── Config/
├── Backend/
│   ├── CloudFunctions/
│   └── Firestore/
└── Documentation/
```

## 🛠️ Tech Stack

- **Engine**: Unity 2022 LTS
- **Language**: C#
- **Backend**: Firebase (Auth, Firestore, Functions, FCM)
- **Payments**: Google Play Billing v5 / Apple StoreKit 2
- **Ads**: Google AdMob (with mediation)
- **Analytics**: Firebase Analytics + Adjust
- **Social**: Facebook SDK + Google Play Games
- **Anti-Cheat**: Server-side validation

## 🚀 Getting Started

1. Clone the repository
2. Open with Unity 2022 LTS
3. Import Firebase SDK
4. Configure AdMob IDs
5. Set up Google Play Services
6. Run on Android (API 26+) or iOS (14+)

## 📱 Screen Flow

```
Splash Screen
    ↓
Login (Guest/Google/Facebook)
    ↓
Home Screen
    ↓
World Map
    ↓
Level Select
    ↓
Pre-Level (Booster Selection)
    ↓
Gameplay
    ↓
Post-Level (Stars + Rewards)
    ↓
Task Complete Popup
    ↓
World Map (with Leaderboard)
```

## 📊 Difficulty Curve

- **Easy**: Levels 1-30
- **Medium**: Levels 31-100
- **Hard**: Levels 101-200
- **Expert**: Levels 201+

## 🏆 Competition System

1. **Sugar Rush Tournament** (48 hours) - All players on same level
2. **Championship League** - Weekly with promotion/demotion
3. **Live Duel Mode** - 1v1 real-time matches with wagering

## 💰 Revenue Model

- **IAP Revenue**: Coins, Lives, Boosters, Subscription
- **Ad Revenue**: Rewarded, Interstitial, Banner ads
- **ARPU Target**: High-quality monetization without pay-to-win feel

## 📈 Analytics & Metrics

- Level completion rates
- Daily/Monthly active users (DAU/MAU)
- Session length & frequency
- Purchase conversion rates
- Retention curves (Day 1, 7, 30)

## 🔐 Security

- Server-side move validation
- Hash checksums for score verification
- Firebase authentication
- XSS/CSRF protection in backend

## 📝 License

MIT License - See LICENSE file for details

## 👤 Author

Developed by @atgnews

---

**Status**: In Development
**Last Updated**: 2024
