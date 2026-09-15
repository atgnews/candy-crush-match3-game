# Candy Crush Match-3 Game - Architecture Documentation

## System Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    Unity Client (C#)                         │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  UI Layer (Scenes, Canvases, UI Elements)            │   │
│  │  - Splash Screen                                      │   │
│  │  - Login/Register                                     │   │
│  │  - Home Screen (Daily Rewards, Events)               │   │
│  │  - World Map (Episodes, Levels)                      │   │
│  │  - Level Select                                       │   │
│  │  - Gameplay Screen                                    │   │
│  │  - Post-Level Results                                │   │
│  │  - Leaderboards                                       │   │
│  │  - Shop/IAP                                           │   │
│  └──────────────────────────────────────────────────────┘   │
│  ┌───────────────────────────────��──────────────────────┐   │
│  │  Gameplay Layer                                       │   │
│  │  - Grid System (8x8)                                 │   │
│  │  - Candy Physics                                      │   │
│  │  - Match Detection Engine                            │   │
│  │  - Special Candy Logic                               │   │
│  │  - Gravity & Cascade System                          │   │
│  │  - Score Calculation                                 │   │
│  │  - Level State Manager                               │   │
│  └──────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Game Systems Layer                                  │   │
│  │  - Player Manager                                     │   │
│  │  - Level Manager                                      │   │
│  │  - Task Manager (Individual)                         │   │
│  │  - Team Manager                                       │   │
│  │  - Inventory Manager (Lives, Coins, Gems)           │   │
│  │  - Booster Manager                                    │   │
│  │  - Analytics Engine                                   │   │
│  │  - Ad Manager (AdMob)                                │   │
│  │  - IAP Manager                                        │   │
│  └──────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Network Layer                                        │   │
│  │  - Firebase Auth Service                             │   │
│  │  - Firestore Data Sync                               │   │
│  │  - Real-time Updates (Teams, Leaderboards)          │   │
│  │  - Cloud Functions RPC                               │   │
│  │  - FCM (Push Notifications)                          │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                           ↕
┌─────────────────────────────────────────────────────────────┐
│                    Backend Services                          │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Firebase (Google Cloud)                             │   │
│  │  ┌──────────────────────────────────────────────┐   │   │
│  │  │  Authentication (Email, Google, Facebook)    │   │   │
│  │  └──────────────────────────────────────────────┘   │   │
│  │  ┌──────────────────────────────────────────────┐   │   │
│  │  │  Firestore Database                          │   │   │
│  │  │  - Users Collection                          │   │   │
│  │  │  - Levels Collection                         │   │   │
│  │  │  - Teams Collection                          │   │   │
│  │  │  - Tasks Collection                          │   │   │
│  │  │  - Leaderboards Collection                   │   │   │
│  │  │  - Events Collection                         │   │   │
│  │  │  - Transactions Collection                   │   │   │
│  │  └──────────────────────────────────────────────┘   │   │
│  │  ┌──────────────────────────────────────────────┐   │   │
│  │  │  Cloud Functions (Node.js)                   │   │   │
│  │  │  - Score Validation                          │   │   │
│  │  │  - Rank Calculation                          │   │   │
│  │  │  - Task Progress Update                      │   │   │
│  │  │  - Team Milestone Checker                    │   │   │
│  │  │  - Leaderboard Aggregation                   │   │   │
│  │  │  - Event Management                          │   │   │
│  │  └──────────────────────────────────────────────┘   │   │
│  │  ┌──────────────────────────────────────────────┐   │   │
│  │  │  FCM (Firebase Cloud Messaging)              │   │   │
│  │  │  - Push Notifications                        │   │   │
│  │  │  - Event Alerts                              │   │   │
│  │  │  - Task Rewards                              │   │   │
│  │  └──────────────────────────────────────────────┘   │   │
│  │  ┌──────────────────────────────────────────────┐   │   │
│  │  │  Analytics (Firebase Analytics)              │   │   │
│  │  │  - Event Tracking                            │   │   │
│  │  │  - Funnel Analysis                           │   │   │
│  │  │  - Retention Metrics                         │   │   │
│  │  └──────────────────────────────────────────────┘   │   │
│  └──────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Third-Party Services                                │   │
│  │  - Google Play Billing (IAP)                        │   │
│  │  - Apple StoreKit 2 (IAP)                           │   │
│  │  - Google AdMob (Ads)                               │   │
│  │  - Meta Audience Network (Mediation)                │   │
│  │  - Unity Ads (Mediation)                            │   │
│  │  - AppLovin MAX (Mediation)                         │   │
│  │  - Adjust (Analytics)                               │   │
│  │  - Facebook SDK (Social)                            │   │
│  │  - Google Play Games (Achievements)                 │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## Data Flow Diagrams

### 1. Level Completion Flow

```
Player completes level
        ↓
Local score calculation (Match detection + cascades)
        ↓
Server-side validation (Cloud Function)
        ↓
✓ Valid: Update Firestore
        ↓
Update player stats:
- Total score
- Coins earned
- Stars achieved
- Level progress
        ↓
Check task progress:
- Daily tasks
- Weekly challenges
- Achievements
        ↓
Update leaderboards (async)
        ↓
Show post-level rewards screen
```

### 2. Match Detection Algorithm

```
Swap candies at (x1, y1) and (x2, y2)
        ↓
Validate swap is adjacent
        ↓
Perform swap locally
        ↓
Scan grid for matches (3+ in row/column)
        ↓
Found matches?
   Yes ↓ No
       → Send to server for validation
       → Return to initial state
        ↓
Clear matched candies, apply special effects
        ↓
Apply gravity (candies fall)
        ↓
Check for cascading matches (new combinations)
        ↓
Continue cascading until no more matches
        ↓
Calculate total score with combo multiplier
        ↓
Update score display
```

### 3. Task Progress System

```
Player action (match striped, use color bomb, etc.)
        ↓
Increment local task counter
        ↓
Sync to Firestore (batch every 5 seconds)
        ↓
Cloud Function checks completion
        ↓
Task complete?
   Yes ↓ No
       → Mark as complete
       → Award rewards
       → Trigger notification
       → Update local inventory
        ↓
Display task completion popup
        ↓
Add reward (coins/XP/booster) to player
```

### 4. Leaderboard Ranking Flow

```
Player score updated in Firestore
        ↓
Trigger Cloud Function (onWrite)
        ↓
Recalculate global rank:
- Count users with higher score
- Store rank in user document
        ↓
Update weekly leaderboard:
- Reset every Sunday 00:00 UTC
- Copy current score to weekly score
- Recalculate weekly ranks
        ↓
Update friends leaderboard (subset query)
        ↓
Cache leaderboard snapshot (performance)
        ↓
Push update to client (if viewing)
```

### 5. Team Task Milestone System

```
Team member completes individual contribution
        ↓
Sync contribution to team's shared task document
        ↓
Cloud Function aggregates team progress
        ↓
Calculate: total_progress / goal_target = % complete
        ↓
Check milestones:
- 25% reached? → Award small reward to all
- 50% reached? → Award medium reward to all
- 100% reached? → Award jackpot + team badge
        ↓
Notify all team members of milestone reached
        ↓
Update team stats (XP, level, reputation)
```

## Database Schema (Firestore)

### Collections Structure

```
firestore/
├── users/
│   └── {userId}/
│       ├── profile/
│       │   ├── username: string
│       │   ├── level: number
│       │   ├── totalScore: number
│       │   ├── createdAt: timestamp
│       │   └── avatar: string
│       ├── progression/
│       │   ├── currentLevel: number
│       │   ├── completedLevels: array
│       │   ├── starsEarned: map {levelId -> stars}
│       │   └── worlds: map {worldId -> completion%}
│       ├── inventory/
│       │   ├── coins: number
│       │   ├── gems: number
│       │   ├── lives: number
│       │   ├── lifeExpireTime: timestamp
│       │   ├── boosters: map {boosterType -> count}
│       │   └── lastLifeRefill: timestamp
│       ├── stats/
│       │   ├── dailyTasksCompleted: number
│       │   ├── weeklyTasksCompleted: number
│       │   ├── loginStreak: number
│       │   ├── totalMatches: number
│       │   ├── totalPlayTime: number
│       │   └── lastPlayTime: timestamp
│       ├── tasks/
│       │   ├── daily/
│       │   │   └── {taskId}: {progress, completed, reward}
│       │   ├── weekly/
│       │   │   └── {taskId}: {progress, completed, reward}
│       │   └── achievements/
│       │       └── {achievementId}: {completed, unlockedAt}
│       ├── leaderboard/
│       │   ├── globalRank: number
│       │   ├── weeklyRank: number
│       │   ├── trophies: number
│       │   └── lastRankUpdate: timestamp
│       ├── team/
│       │   ├── teamId: string
│       │   ├── role: enum [MEMBER, LEADER]
│       │   ├── joinedAt: timestamp
│       │   └── contribution: number
│       └── settings/
│           ├── pushNotificationsEnabled: boolean
│           ├── soundEnabled: boolean
│           ├── musicEnabled: boolean
│           └── language: string
│
├── levels/
│   └── {levelId}/
│       ├── basic/
│       │   ├── world: number
│       │   ├── episode: number
│       │   ├── type: enum [jelly, ingredient, timed, order, monkling]
│       │   ├── difficulty: enum [easy, medium, hard, expert]
│       │   └── description: string
│       ├── goals/
│       │   ├── moveLimit: number (or null)
│       │   ├── timeLimit: number (or null)
│       │   ├── targetScore: array [1-star, 3-star, 5-star]
│       │   └── specialGoals: array
│       ├── board/
│       │   ├── grid: array[8][8] (candy types)
│       │   ├── blockers: array {type, position}
│       │   ├── jelly: array {positions} (if jelly level)
│       │   └── ingredients: array {type, target}
│       ├── rules/
│       │   ├── allowedBoosters: array
│       │   ├── startingBoosters: array {boosterType -> count}
│       │   └── specialRules: array
│       ├── leaderboard/
│       │   └── topScores: array {userId, score, stars, timestamp}
│       └── meta/
│           ├── createdAt: timestamp
│           ├── lastModified: timestamp
│           ├── version: number
│           └── published: boolean
│
├── teams/
│   └── {teamId}/
│       ├── profile/
│       │   ├── name: string
│       │   ├── level: number (1-50)
│       │   ├── xp: number
│       │   ├── createdAt: timestamp
│       │   ├── leader: userId
│       │   ├── members: array {userId, joinedAt, role, contribution}
│       │   ├── badge: string
│       │   └── bio: string
│       ├── stats/
│       │   ├── totalScore: number
│       │   ├── weeklyScore: number
│       │   ├── activeLevels: number
│       │   └── memberCount: number
│       ├── tasks/
│       │   ├── current: array {taskId, goal, progress, milestones}
│       │   └── history: array {taskId, completedAt, reward}
│       ├── chat/
│       │   └── messages: array {userId, message, timestamp}
│       └── leaderboard/
│           └── weeklyRank: number
│
├── tasks/
│   ├── daily/
│   │   └── {taskId}/
│   │       ├── name: string
│   │       ├── description: string
│   │       ├── goal: number
│   │       ├── reward: map {type -> amount}
│   │       ├── difficulty: string
│   │       └── createdAt: timestamp
│   ├── weekly/
│   │   └── {taskId}/
│   │       ├── name: string
│   │       ├── goal: number
│   │       ├── reward: map
│   │       └── resetTime: timestamp
│   └── achievements/
│       └── {achievementId}/
│           ├── name: string
│           ├── requirement: string
│           ├── reward: map
│           └── hidden: boolean
│
├── events/
│   └── {eventId}/
│       ├── name: string
│       ├── type: enum [tournament, league, daily_duel]
│       ├── startTime: timestamp
│       ├── endTime: timestamp
│       ├── level: number (special level for tournament)
│       ├── leaderboard: array {rank, userId, score}
│       ├── rewards: map {rank_range -> reward}
│       └── status: enum [upcoming, active, ended]
│
├── leaderboards/
│   ├── global/
│   │   └── snapshot/ (top 100 cached)
│   │       ├── rank: number
│   │       ├── userId: string
│   │       ├── score: number
│   │       ├── level: number
│   │       └── lastUpdated: timestamp
│   ├── weekly/
│   │   └── snapshot/ (resets each week)
│   ├── friends/
│   │   └── {userId}/
│   │       ├── friendId: string
│   │       ├── friendScore: number
│   │       └── friendRank: number
│   └── byLevel/
│       └── {levelId}/
│           └── topScores: array {userId, score, stars}
│
└── transactions/
    └── {transactionId}/
        ├── userId: string
        ├── type: enum [iap, ad_reward, task_reward, daily_login]
        ├── amount: map {currency -> amount}
        ├── productId: string (if IAP)
        ├── status: enum [pending, completed, failed]
        ├── timestamp: timestamp
        └── receipt: object (if IAP)
```

## Cloud Functions

### Key Functions

1. **validateScore** - Verify move is legal, recalculate score
2. **updateLeaderboard** - Aggregate and rank scores
3. **checkTaskProgress** - Monitor and award task completions
4. **processMilestone** - Check team task milestones
5. **awardDailyReward** - Handle daily login rewards
6. **processPurchase** - Verify IAP and grant items
7. **manageEvents** - Create/end competitive events
8. **sendNotifications** - Trigger push notifications
9. **generateDailyTasks** - Create new daily tasks at reset
10. **antiCheatValidation** - Detect suspicious patterns

## Security Rules (Firestore)

```javascript
// Users can only read/write their own data
match /users/{userId} {
  allow read, write: if request.auth.uid == userId;
}

// Levels are public read, admin write
match /levels/{levelId} {
  allow read: if true;
  allow write: if request.auth.token.admin == true;
}

// Teams: members can read/write
match /teams/{teamId} {
  allow read, write: if request.auth.uid in resource.data.members[].userId;
}

// Leaderboards are public read
match /leaderboards/{document=**} {
  allow read: if true;
  allow write: if false;
}

// Transactions require auth
match /transactions/{transactionId} {
  allow read: if request.auth.uid == resource.data.userId;
  allow create: if request.auth.uid != null;
}
```

## Performance Optimizations

1. **Client-side Caching**: Levels cached after first download
2. **Batch Operations**: Task progress synced every 5 seconds
3. **Pagination**: Leaderboards load top 100 + user's rank
4. **Real-time Updates**: Only sync active gameplay data
5. **Offline Support**: Store level state locally, sync on reconnect
6. **CDN**: Level assets served via Firebase Storage
7. **Indexing**: Composite indexes on leaderboard queries

## Anti-Cheat Measures

1. **Server-side Score Validation**
   - Recalculate score from move list
   - Validate move legality
   - Check cascade combinations

2. **Anomaly Detection**
   - Flag impossible speed (10 moves in 1 second)
   - Detect pattern manipulation
   - Monitor win rate vs. level difficulty

3. **Hash Verification**
   - Client sends move hash
   - Server recalculates and compares
   - Flag mismatches

4. **Rate Limiting**
   - Max 100 moves per level
   - Max 10 levels per minute
   - Cool-down on task rewards
