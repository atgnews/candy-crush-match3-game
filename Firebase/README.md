# Firebase Configuration

## Setup Instructions

### 1. Install Firebase CLI

```bash
npm install -g firebase-tools
firebase login
```

### 2. Initialize Firebase Project

```bash
cd Firebase/
firebase init
```

### 3. Deploy Firestore Rules

```bash
firebase deploy --only firestore:rules
```

### 4. Deploy Cloud Functions

```bash
cd Functions/
npm install
firebase deploy --only functions
```

### 5. Setup Firestore Collections

Create the following collections in Firestore Console:

- `users` - User profiles and progress
- `levels` - Level data
- `teams` - Team data
- `tasks` - Task definitions
- `leaderboards` - Leaderboard entries
- `transactions` - Purchase and reward transactions
- `events` - Game events and tournaments

### 6. Environment Variables

Create `.env` file:

```env
FIREBASE_PROJECT_ID=candycrush2024
FIREBASE_API_KEY=your_api_key
FIREBASE_AUTH_DOMAIN=candycrush2024.firebaseapp.com
FIREBASE_STORAGE_BUCKET=candycrush2024.appspot.com
FIREBASE_MESSAGING_SENDER_ID=your_sender_id
FIREBASE_APP_ID=your_app_id
```

## Cloud Functions Deployment

### Deploy All Functions

```bash
firebase deploy --only functions
```

### Deploy Specific Function

```bash
firebase deploy --only functions:validateAndSaveScore
```

### View Logs

```bash
firebase functions:log
```

### Test Functions Locally

```bash
firebase emulators:start --only functions
```

## Scheduled Functions

- `resetDailyTasks` - Runs daily at 00:00 UTC
- `resetWeeklyLeaderboard` - Runs weekly (Sunday) at 00:00 UTC

## Firestore Security Rules

Rules are defined in `firestore.rules` and enforce:

1. **User Data**: Only accessible by the user themselves
2. **Levels**: Public read, admin write only
3. **Teams**: Members can read, leaders can write
4. **Leaderboards**: Public read, server write only
5. **Transactions**: Users read their own, server writes only

## Performance Optimization

1. **Batch Operations**: Use batch writes to reduce latency
2. **Indexes**: Composite indexes on frequently queried collections
3. **Pagination**: Load top 100 + user's rank
4. **Caching**: Cache leaderboard snapshots on client

## Monitoring

Monitor Cloud Functions usage:
- Firebase Console > Functions tab
- Check execution count, errors, and latency
- Set up alerts for high error rates

## Anti-Cheat

The `detectAnomalies` function flags suspicious patterns:
- Impossible move speeds
- Score inflation
- Unusual play patterns

Flagged activities are logged in `suspicious_activity` collection for review.
