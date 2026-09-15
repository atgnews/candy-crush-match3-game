# Candy Crush Match-3 - Testing & QA Guide

## Test Scenarios

### Gameplay Testing

#### Level Completion
- [ ] Level loads correctly
- [ ] Grid displays all 8x8 candies
- [ ] Score updates in real-time
- [ ] Move counter decreases with each move
- [ ] Timer counts down for timed levels
- [ ] Match detection works correctly
- [ ] Cascades display properly
- [ ] Level completes when goals met
- [ ] Stars awarded based on score
- [ ] Results screen shows correct data

#### Match Detection
- [ ] 3-match detected and cleared
- [ ] 4-match creates striped candy
- [ ] 5-match creates color bomb
- [ ] Cascades propagate correctly
- [ ] Gravity applies properly
- [ ] No matches result in swap undo

#### User Interface
- [ ] All buttons are clickable
- [ ] Text displays correctly
- [ ] Images load without artifacts
- [ ] Animations are smooth
- [ ] No UI overlap or clipping
- [ ] Portrait mode on all screens
- [ ] Text is readable on small screens

### Monetization Testing

#### AdMob
- [ ] Banner ad loads and displays
- [ ] Interstitial ad shows on level complete
- [ ] Rewarded ad plays and grants reward
- [ ] No ads show during gameplay
- [ ] Ads don't interfere with input
- [ ] Test ads display when configured

#### IAP
- [ ] Coins pack purchase works
- [ ] Lives pack purchase works
- [ ] Booster bundle purchase works
- [ ] Subscription purchase works
- [ ] Receipt validation succeeds
- [ ] Rewards granted after purchase
- [ ] Can't purchase twice (consumables)
- [ ] Subscription renewals work (if implemented)

### Progression Testing

#### Level Progression
- [ ] Can unlock next level after completion
- [ ] Level select shows completed status
- [ ] World map shows progress
- [ ] Episode unlock gates work
- [ ] Level difficulty increases appropriately

#### Task System
- [ ] Daily tasks appear
- [ ] Task progress updates
- [ ] Task completion recognized
- [ ] Rewards granted on completion
- [ ] Tasks reset daily
- [ ] Completed tasks show in history

#### Leaderboard
- [ ] Score updates on leaderboard
- [ ] Rank calculation is correct
- [ ] Leaderboard displays top 100
- [ ] Player's rank shown
- [ ] Weekly leaderboard resets

### Network Testing

#### Firebase Sync
- [ ] Player data saves to Firestore
- [ ] Data loads on app launch
- [ ] Score syncs to server
- [ ] Profile updates sync
- [ ] Handles offline gracefully
- [ ] Retries failed operations
- [ ] No data loss on disconnect

#### Authentication
- [ ] Guest login works
- [ ] Email/password login works
- [ ] Google login works
- [ ] Facebook login works
- [ ] Logout clears local data
- [ ] Session persists on app restart

### Performance Testing

#### Frame Rate
- [ ] Maintains 60 FPS during gameplay
- [ ] No frame drops during matches
- [ ] Smooth cascades and animations
- [ ] Menu transitions are fluid

#### Memory
- [ ] Memory under 500MB during gameplay
- [ ] No memory leaks after extended play
- [ ] Memory stable after level completion
- [ ] Proper cleanup on scene change

#### Loading Times
- [ ] App launch < 3 seconds
- [ ] Level load < 3 seconds
- [ ] Menu transitions < 1 second
- [ ] Leaderboard load < 2 seconds

### Device Testing

#### Android
- [ ] API 26+ compatibility
- [ ] Works on 6" phones
- [ ] Works on 7" tablets
- [ ] Landscape and portrait
- [ ] Touch input responsive
- [ ] Notch handling correct

#### iOS
- [ ] iOS 14+ compatibility
- [ ] Works on iPhone SE
- [ ] Works on iPhone Pro Max
- [ ] Landscape and portrait
- [ ] Touch input responsive
- [ ] Safe area respected

### Edge Cases

- [ ] No lives available
- [ ] Out of coins for purchase
- [ ] Network timeout during save
- [ ] Failed ad load
- [ ] Interrupted purchase
- [ ] Game quit mid-level
- [ ] App backgrounded during sync
- [ ] Device out of storage
- [ ] Permission denied
- [ ] Multiple quick swaps

## Performance Benchmarks

### Target Metrics

| Metric | Target | Actual |
|--------|--------|--------|
| Startup Time | < 3s | |
| Level Load Time | < 3s | |
| Frame Rate | 60 FPS | |
| Memory Usage | < 500MB | |
| Battery Drain | < 50mA | |
| Network Latency | < 500ms | |
| Crash Rate | < 0.5% | |

## Bug Reporting Template

**Title**: Brief description of issue

**Device**: Phone model, OS version, app version

**Steps to Reproduce**:
1. Step 1
2. Step 2
3. Step 3

**Expected Behavior**: What should happen

**Actual Behavior**: What actually happened

**Screenshots/Videos**: Attach if applicable

**Logs**: Include relevant error messages

**Severity**: Critical / High / Medium / Low

## Testing Tools

### Unity Tools
- Unity Profiler (Performance)
- Frame Debugger (Graphics)
- Memory Profiler (Memory)
- Console (Logging)

### Device Tools
- Android Profiler
- Xcode Instruments
- Firebase Console (Crash reporting)
- AdMob Dashboard (Ad performance)

### Testing Services
- TestFlight (iOS beta)
- Google Play Console (Android beta)
- Firebase Test Lab (Automated testing)
- Firebase Crashlytics (Crash analytics)

## Regression Test Checklist

Run before each release:

- [ ] All levels playable
- [ ] No crashes on startup
- [ ] Save/load works
- [ ] Ads display correctly
- [ ] IAP works end-to-end
- [ ] Leaderboard updates
- [ ] Tasks progress correctly
- [ ] Network sync reliable
- [ ] Performance within targets
- [ ] No new memory leaks

## Known Issues & Workarounds

| Issue | Workaround | Status |
|-------|-----------|--------|
| | | |

## Release Criteria

Before releasing to production:

- [ ] All critical bugs fixed
- [ ] Performance meets targets
- [ ] Crash rate < 0.5%
- [ ] Test pass rate > 95%
- [ ] Beta testers satisfied (rating > 4.0)
- [ ] Analytics configured
- [ ] Monitoring alerts setup
- [ ] Rollback plan ready
