# Project Setup Guide

## Prerequisites

- **Unity 2022 LTS** (2022.3.x or later)
- **Visual Studio Code** or **JetBrains Rider**
- **Android SDK** (API 26+) for Android builds
- **Xcode 14+** for iOS builds
- **Node.js 16+** for Firebase Cloud Functions
- **Firebase CLI** for deployment

## 1. Unity Project Setup

### 1.1 Create Unity Project

```bash
# Clone repository
git clone https://github.com/atgnews/candy-crush-match3-game.git
cd candy-crush-match3-game

# Create new Unity project (if not cloning URP template)
unity -createProject . -projectName CandyCrush2024
```

### 1.2 Install Essential Packages

In Unity Package Manager (`Window` > `Package Manager`):

```
Required:
- Firebase SDK for Unity (com.google.firebase.database, com.google.firebase.auth, etc.)
- TextMesh Pro (if not already)
- Addressables System (for asset management)
- Google Mobile Ads Unity Plugin (for AdMob)
- Google Play Billing Library
- Facebook SDK for Unity

Optional but recommended:
- DOTween Pro (animations)
- Json.NET (for parsing)
- Newtonsoft.Json (NuGet package)
```

### 1.3 Add via Package Manager

```json
{
  "dependencies": {
    "com.google.firebase.auth": "11.5.0",
    "com.google.firebase.database": "11.5.0",
    "com.google.firebase.firestore": "11.5.0",
    "com.google.firebase.analytics": "11.5.0",
    "com.google.firebase.messaging": "11.5.0",
    "com.google.firebase.storage": "11.5.0",
    "com.google.firebase.functions": "11.5.0",
    "com.google.gms.google-play-services-ads": "20.6.0",
    "com.google.gms.google-play-services-games": "20.6.0",
    "com.facebook.sdk": "16.1.0",
    "com.unity.addressables": "1.21.17",
    "com.unity.textmeshpro": "3.0.6",
    "com.unity.timeline": "1.7.4"
  }
}
```

## 2. Firebase Setup

### 2.1 Create Firebase Project

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Click "Create a project" → Name: "CandyCrush2024"
3. Enable Google Analytics
4. Select or create Google Cloud project

### 2.2 Register Apps

**Android:**

1. Project Settings → Add Android app
2. Package name: `com.atgnews.candycrush`
3. Download `google-services.json` → Place in `Assets/Plugins/Android/`
4. SHA-1 fingerprint from `keytool` (for auth)

**iOS:**

1. Project Settings → Add iOS app
2. Bundle ID: `com.atgnews.candycrush`
3. Download `GoogleService-Info.plist` → Place in Xcode project

### 2.3 Enable Firebase Services

In Firebase Console:

- **Authentication**: Enable Email/Password, Google, Facebook
- **Firestore Database**: Create in production mode
- **Cloud Functions**: Enable (deploy from `Firebase/Functions/`)
- **Cloud Storage**: Enable (for user avatars, event data)
- **Cloud Messaging**: Enable (for push notifications)
- **Analytics**: Enable (auto-collects events)
- **Realtime Database**: Enable for team chat (alternative to Firestore)

### 2.4 Firestore Security Rules

Deploy security rules:

```bash
cd Firebase/
firebase deploy --only firestore:rules
```

### 2.5 Set Firebase Config in Unity

Create `Assets/Resources/firebase-config.json`:

```json
{
  "apiKey": "YOUR_WEB_API_KEY",
  "authDomain": "candycrush2024.firebaseapp.com",
  "projectId": "candycrush2024",
  "storageBucket": "candycrush2024.appspot.com",
  "messagingSenderId": "YOUR_SENDER_ID",
  "appId": "YOUR_APP_ID",
  "databaseURL": "https://candycrush2024.firebaseio.com"
}
```

## 3. Google Play & AdMob Setup

### 3.1 Google Play Console

1. Create app: `com.atgnews.candycrush`
2. Set up app information, screenshots, ratings
3. Create signed APK/AAB for testing
4. Link Firebase project

### 3.2 AdMob Setup

1. Go to [Google AdMob](https://admob.google.com/)
2. Link your Google Play account
3. Create ad units:
   - **Banner Ad**: `ca-app-pub-xxxxxxxxxxxxxxxx/xxxxxxxx`
   - **Interstitial Ad**: `ca-app-pub-xxxxxxxxxxxxxxxx/yyyyyyyy`
   - **Rewarded Ad**: `ca-app-pub-xxxxxxxxxxxxxxxx/zzzzzzzz`

### 3.3 Add AdMob IDs to Unity

Create `Assets/Resources/admob-config.json`:

```json
{
  "androidAppId": "ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy",
  "iosAppId": "ca-app-pub-xxxxxxxxxxxxxxxx~zzzzzzzzzz",
  "androidBannerId": "ca-app-pub-3940256099942544/6300978111",
  "iosBannerId": "ca-app-pub-3940256099942544/2934735945",
  "androidInterstitialId": "ca-app-pub-3940256099942544/1033173712",
  "iosInterstitialId": "ca-app-pub-3940256099942544/4411468910",
  "androidRewardedId": "ca-app-pub-3940256099942544/5224354917",
  "iosRewardedId": "ca-app-pub-3940256099942544/1712485313"
}
```

**Note**: Use test Ad Unit IDs during development

## 4. In-App Purchase (IAP) Setup

### 4.1 Google Play Billing

**Android:**

```xml
<!-- AndroidManifest.xml -->
<uses-permission android:name="com.android.vending.BILLING" />
```

1. Google Play Console → In-app products → Create SKUs:
   - `coins_99_rupees` → ₹99 = 400 coins
   - `coins_499_rupees` → ₹499 = 2500 coins
   - `coins_999_rupees` → ₹999 = 6000 coins
   - `lives_5_pack` → ₹29 = 5 lives
   - `subscription_monthly` → ₹299/month

2. Set up subscription items with auto-renewal

### 4.2 Apple App Store Connect

**iOS:**

1. Create In-App Purchase items with same SKU names
2. Set prices in different countries
3. Configure subscription group for auto-renewal
4. Wait 24 hours for App Store review

### 4.3 Unity IAP Configuration

Enable in `Window` > `Unity IAP` > `IAP Button`

## 5. Social Integration

### 5.1 Facebook SDK

```csharp
// Assets/Scripts/Social/FacebookManager.cs
using Facebook.Unity;

public class FacebookManager : MonoBehaviour
{
    void Start()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(OnFacebookInit);
        }
    }

    void OnFacebookInit()
    {
        if (FB.IsInitialized) FB.ActivateApp();
    }

    public void ShareLevel(int levelId, int score)
    {
        var shareDialog = new ShareDialog();
        shareDialog.Share(
            new Uri($"https://candycrush.atgnews.com/level/{levelId}?score={score}"),
            null,
            new Uri("https://candycrush.atgnews.com/logo.png"),
            "Check out my Candy Crush score!"
        );
    }
}
```

### 5.2 Google Play Games

Enable in Google Play Console:

```csharp
// Achievements & leaderboards
PlayGamesPlatform.Activate();
```

## 6. Build Settings Configuration

### 6.1 Android Build Settings

```
File > Build Settings > Android
- Min API Level: 26
- Target API Level: 33+
- Graphics API: OpenGL ES 3.0
- Scripting Backend: IL2CPP
- API Compatibility Level: .NET 4.x
- Texture Compression: ETC2 (main), DXT5 (fallback)
- Resolution: 1080×1920 (portrait)
```

### 6.2 iOS Build Settings

```
File > Build Settings > iOS
- Min iOS Version: 14.0
- Architecture: ARM64
- Graphics API: Metal
- Scripting Backend: IL2CPP
- Api Compatibility Level: .NET 4.x
```

### 6.3 Player Settings

```
Edit > Project Settings > Player
- Product Name: Candy Crush Match-3
- Package Name: com.atgnews.candycrush
- Icon: 1024×1024 PNG
- Splash Image: 1242×2208 PNG
- Allow Unsafe Code: ON (for IL2CPP)
- Strip Unused Metadata: ON
- Managed Stripping Level: High
```

## 7. Cloud Functions Deployment

### 7.1 Initialize Firebase Functions

```bash
cd Firebase/Functions/
npm install
```

### 7.2 Deploy Functions

```bash
firebase deploy --only functions
```

### 7.3 Verify Deployment

```bash
firebase functions:list
```

## 8. Testing

### 8.1 Local Testing

```bash
# Firebase Emulator Suite
firebase emulators:start
```

### 8.2 Device Testing

**Android:**

```bash
# Build and install
unity -batchmode -executeMethod BuildAndroid -quit
adb install -r Builds/Android/CandyCrush.apk
```

**iOS:**

```bash
# Build for Xcode
unity -batchmode -executeMethod BuildiOS -quit
# Open in Xcode and run on device/simulator
```

### 8.3 Test Cases

- [ ] Level loading and progression
- [ ] Match detection and cascades
- [ ] Score calculation validation
- [ ] Task progress tracking
- [ ] Leaderboard updates
- [ ] IAP purchase flow
- [ ] AdMob ad display
- [ ] Firebase sync
- [ ] Push notifications
- [ ] Team chat

## 9. Performance Optimization

### 9.1 Build Size Optimization

```csharp
// Remove unused code
#if UNITY_EDITOR
[UnityEditor.MenuItem("Build/Optimize")]
static void OptimizeBuild()
{
    // Strip unused assemblies
    // Reduce shader variants
    // Compress textures
}
#endif
```

### 9.2 Memory Profiling

- Use Unity Profiler (`Window` > `Analysis` > `Profiler`)
- Monitor heap size during gameplay
- Target: < 500MB on mid-range devices

## 10. Deployment Checklist

- [ ] Firebase project created and linked
- [ ] AdMob account setup with ad units
- [ ] Google Play Console app created
- [ ] App Store Connect app created (iOS)
- [ ] IAP products configured on both platforms
- [ ] Firebase security rules deployed
- [ ] Cloud Functions deployed
- [ ] Signed APK/AAB generated
- [ ] TestFlight build uploaded (iOS)
- [ ] Ratings & Reviews policies set
- [ ] Privacy policy written and linked
- [ ] Beta testing with 100+ testers
- [ ] Final review and publication

## Troubleshooting

### Firebase Connection Issues

```csharp
// Check initialization
public void CheckFirebase()
{
    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
    {
        if (task.IsCompleted)
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase Ready");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        }
    });
}
```

### AdMob Test Ads

Use test device IDs during development:

```csharp
var requestConfiguration = new RequestConfiguration.Builder()
    .SetTestDeviceIds(new List<string> { "33BE2250B43518CCDA7DE426D04EE232" })
    .build();
MobileAds.SetRequestConfiguration(requestConfiguration);
```
