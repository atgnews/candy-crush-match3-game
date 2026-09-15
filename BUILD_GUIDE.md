# Candy Crush Match-3 - Build & Release Guide

## Android Build

### Prerequisites

- Android SDK (API 33+)
- JDK 11+
- Keystore file for signing

### Generate Keystore

```bash
keytool -genkey -v -keystore candy_crush.keystore \
  -alias candy_crush_key \
  -keyalg RSA -keysize 2048 -validity 10000
```

### Configure Player Settings

```
Edit > Project Settings > Player (Android)
- Company Name: atgnews
- Product Name: Candy Crush Match-3
- Package Name: com.atgnews.candycrush
- Minimum API Level: 26
- Target API Level: 33
- Version: 1.0.0
- Build Number: 1
```

### Build APK

```bash
unity -batchmode \
  -projectPath /path/to/project \
  -executeMethod BuildAndroid \
  -quit
```

### Build AAB (App Bundle)

```bash
# Use Play Console to build from APK, or
gradlew bundleRelease
```

### Submit to Google Play Console

1. Create app in Google Play Console
2. Fill in app details:
   - Screenshots (5 minimum)
   - Icon (512x512)
   - Featured graphic (1024x500)
   - Description (4000 chars max)
   - Privacy policy
   - Contact email
3. Upload AAB
4. Configure pricing and distribution
5. Submit for review (24-48 hour review time)

## iOS Build

### Prerequisites

- Xcode 14+
- iOS SDK 14+
- Apple Developer Account
- Provisioning profiles

### Configure Player Settings

```
Edit > Project Settings > Player (iOS)
- Company Name: atgnews
- Product Name: Candy Crush Match-3
- Bundle Identifier: com.atgnews.candycrush
- Version: 1.0.0
- Build: 1
- Target Minimum iOS Version: 14.0
- Architecture: ARM64
```

### Build for Xcode

```
File > Build Settings > iOS
- Select Xcode project destination
- Click Build
```

### Archive in Xcode

```bash
xcodebuild archive \
  -project project.pbxproj \
  -scheme CandyCrush \
  -configuration Release \
  -archivePath build/CandyCrush.xcarchive
```

### Submit to App Store Connect

1. Create app in App Store Connect
2. Fill in app information:
   - Screenshots (2-5 per device)
   - Icon (1024x1024)
   - Description (4000 chars)
   - Keywords
   - Support URL
   - Privacy policy
   - Age rating
3. Upload archive from Xcode Organizer
4. Submit for review (24-48 hour review time)

## Version Management

### Semantic Versioning

```
MAJOR.MINOR.PATCH
1.0.0 = Initial release
1.1.0 = New features
1.0.1 = Bug fixes
2.0.0 = Major overhaul
```

### Build Increments

- APK/AAB: Increment build number with each release
- iOS: Match build to Android for consistency

## Release Process

### Pre-Release (1 week before)

1. [ ] Freeze feature development
2. [ ] Create release branch
3. [ ] Run full test suite
4. [ ] Performance benchmark
5. [ ] Beta test with 100+ users
6. [ ] Collect feedback and fix critical issues

### Release Day

1. [ ] Final QA pass
2. [ ] Update version numbers
3. [ ] Create release notes
4. [ ] Build final APK/AAB
5. [ ] Submit to stores
6. [ ] Monitor crash reports
7. [ ] Prepare rollback plan

### Post-Release

1. [ ] Monitor analytics
2. [ ] Track crash rates
3. [ ] Respond to user feedback
4. [ ] Patch critical bugs immediately
5. [ ] Plan next release

## Release Notes Template

```markdown
# Version 1.0.0

## New Features
- 500+ levels across 25 worlds
- Team system with collaborative tasks
- Weekly competitive events
- Premium subscription

## Improvements
- Better match detection algorithm
- Improved UI responsiveness
- Optimized battery usage

## Bug Fixes
- Fixed cascade animation glitch
- Fixed leaderboard sync issues
- Fixed IAP purchase verification

## Known Issues
- None at this time

## Technical Details
- Unity 2022 LTS
- Firebase backend
- Google Play Billing v5
- AdMob ads

Thank you for playing!
```

## Monitoring & Analytics

### Firebase Console

- Crash rate and frequency
- Performance metrics
- User retention
- Revenue tracking

### Google Play Console

- Install trends
- Rating and reviews
- Crash reports
- ANR (Application Not Responding) reports

### App Store Connect

- Download trends
- Ratings and reviews
- Crash reports
- Performance metrics

## Rollback Procedures

### If Critical Bug Found

1. Immediately notify team
2. Prepare patched version
3. Request expedited review (if store)
4. Push emergency update
5. Document incident

### Rollback to Previous Version

- Google Play: "Manage beta" > Remove current
- App Store: Reduce version availability
- Inform users of rollback

## Build Automation

### GitHub Actions (CI/CD)

```yaml
name: Build and Release

on:
  push:
    tags:
      - 'v*'

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: game-ci/unity-builder@v2
        with:
          targetPlatform: Android
          buildName: CandyCrush
      - uses: actions/upload-artifact@v2
        with:
          name: CandyCrush.apk
          path: build/
```

## Security Checklist

- [ ] All API keys secure (not in code)
- [ ] No hardcoded passwords
- [ ] Firebase rules restrict unauthorized access
- [ ] IAP receipt validation enabled
- [ ] SSL certificate pinning (if sensitive data)
- [ ] User data encrypted at rest
- [ ] PII compliance (GDPR, CCPA)
- [ ] Privacy policy published

## Performance Before Release

- [ ] FPS stable at 60
- [ ] Memory < 500MB
- [ ] Load times < 3s
- [ ] Battery drain acceptable
- [ ] Crashes < 0.5%
- [ ] No ANRs
- [ ] All devices tested

## Support

For build issues:

1. Check Unity build logs
2. Verify Android SDK/NDK versions
3. Check Xcode build output
4. Review Firebase and AdMob configurations
5. Check app signing credentials
6. Contact platform support if needed
