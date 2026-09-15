using UnityEngine;
using GoogleMobileAds.Client;
using GoogleMobileAds.Common;
using CandyCrush.Core;
using CandyCrush.Events;
using System;

namespace CandyCrush.Monetization
{
    /// <summary>
    /// Manages AdMob integration for banner, interstitial, and rewarded ads
    /// </summary>
    public class AdManager : Singleton<AdManager>
    {
        [SerializeField] private string androidAppId = "ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy";
        [SerializeField] private string iosAppId = "ca-app-pub-xxxxxxxxxxxxxxxx~zzzzzzzzzz";
        [SerializeField] private string androidBannerId = "ca-app-pub-3940256099942544/6300978111";
        [SerializeField] private string iosBannerId = "ca-app-pub-3940256099942544/2934735945";
        [SerializeField] private string androidInterstitialId = "ca-app-pub-3940256099942544/1033173712";
        [SerializeField] private string iosInterstitialId = "ca-app-pub-3940256099942544/4411468910";
        [SerializeField] private string androidRewardedId = "ca-app-pub-3940256099942544/5224354917";
        [SerializeField] private string iosRewardedId = "ca-app-pub-3940256099942544/1712485313";

        private BannerView bannerView;
        private InterstitialAd interstitialAd;
        private RewardedAd rewardedAd;
        private int interstitialShowCount = 0;
        private DateTime lastInterstitialTime = DateTime.MinValue;
        private const int INTERSTITIAL_FREQUENCY = 3; // Show every 3 level completions
        private const int INTERSTITIAL_COOLDOWN = 300; // 5 minutes in seconds

        protected override void Awake()
        {
            base.Awake();
            InitializeMobileAds();
        }

        /// <summary>
        /// Initialize Google Mobile Ads SDK
        /// </summary>
        private void InitializeMobileAds()
        {
            MobileAds.Initialize(initStatus =>
            {
                Debug.Log("Google Mobile Ads initialized");
                LoadBannerAd();
                LoadInterstitialAd();
                LoadRewardedAd();
            });
        }

        /// <summary>
        /// Load banner ad
        /// </summary>
        private void LoadBannerAd()
        {
            if (bannerView != null)
            {
                bannerView.Destroy();
            }

            var adRequest = new AdRequest();
            string adUnitId = GetBannerAdUnitId();

            bannerView = new BannerView(adUnitId, AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);
            bannerView.OnBannerAdLoaded += () =>
            {
                Debug.Log("Banner ad loaded");
                GameEvents.InvokeAdLoaded(AdType.Banner);
            };
            bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                Debug.LogWarning($"Banner ad failed to load: {error}");
                GameEvents.InvokeAdFailed(AdType.Banner, error.ToString());
            };

            bannerView.LoadAd(adRequest);
        }

        /// <summary>
        /// Load interstitial ad
        /// </summary>
        private void LoadInterstitialAd()
        {
            var adRequest = new AdRequest();
            string adUnitId = GetInterstitialAdUnitId();

            InterstitialAd.Load(adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogWarning($"Interstitial ad failed to load: {error}");
                    GameEvents.InvokeAdFailed(AdType.Interstitial, error?.ToString() ?? "Unknown error");
                    return;
                }

                interstitialAd = ad;
                Debug.Log("Interstitial ad loaded");
                GameEvents.InvokeAdLoaded(AdType.Interstitial);

                RegisterInterstitialEvents(ad);
            });
        }

        /// <summary>
        /// Load rewarded ad
        /// </summary>
        private void LoadRewardedAd()
        {
            var adRequest = new AdRequest();
            string adUnitId = GetRewardedAdUnitId();

            RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogWarning($"Rewarded ad failed to load: {error}");
                    GameEvents.InvokeAdFailed(AdType.Rewarded, error?.ToString() ?? "Unknown error");
                    return;
                }

                rewardedAd = ad;
                Debug.Log("Rewarded ad loaded");
                GameEvents.InvokeAdLoaded(AdType.Rewarded);

                RegisterRewardedEvents(ad);
            });
        }

        /// <summary>
        /// Show banner ad
        /// </summary>
        public void ShowBanner()
        {
            if (bannerView != null)
            {
                bannerView.Show();
                GameEvents.InvokeAdDisplayed(AdType.Banner);
            }
        }

        /// <summary>
        /// Hide banner ad
        /// </summary>
        public void HideBanner()
        {
            if (bannerView != null)
            {
                bannerView.Hide();
                GameEvents.InvokeAdClosed(AdType.Banner);
            }
        }

        /// <summary>
        /// Show interstitial ad
        /// </summary>
        public void ShowInterstitial()
        {
            // Check frequency cap
            if (interstitialShowCount >= 4) // Max 4 per session
            {
                Debug.Log("Interstitial frequency cap reached");
                return;
            }

            // Check cooldown
            if ((DateTime.Now - lastInterstitialTime).TotalSeconds < INTERSTITIAL_COOLDOWN)
            {
                Debug.Log("Interstitial on cooldown");
                return;
            }

            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                interstitialAd.Show();
                interstitialShowCount++;
                lastInterstitialTime = DateTime.Now;
                GameEvents.InvokeAdDisplayed(AdType.Interstitial);
            }
            else
            {
                Debug.LogWarning("Interstitial ad not ready");
                LoadInterstitialAd(); // Reload
            }
        }

        /// <summary>
        /// Show rewarded ad
        /// </summary>
        public void ShowRewarded(System.Action onRewardEarned = null)
        {
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log($"Rewarded ad watched: {reward.Amount} {reward.Type}");
                    GameEvents.InvokeAdRewarded(AdType.Rewarded);
                    onRewardEarned?.Invoke();
                });
                GameEvents.InvokeAdDisplayed(AdType.Rewarded);
            }
            else
            {
                Debug.LogWarning("Rewarded ad not ready");
                LoadRewardedAd(); // Reload
            }
        }

        /// <summary>
        /// Try to show interstitial on level completion
        /// </summary>
        public void OnLevelCompleted()
        {
            interstitialShowCount++;
            if (interstitialShowCount >= INTERSTITIAL_FREQUENCY)
            {
                interstitialShowCount = 0;
                ShowInterstitial();
            }
        }

        /// <summary>
        /// Register interstitial ad events
        /// </summary>
        private void RegisterInterstitialEvents(InterstitialAd ad)
        {
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad closed");
                GameEvents.InvokeAdClosed(AdType.Interstitial);
                LoadInterstitialAd(); // Reload for next time
            };

            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogWarning($"Interstitial ad failed to display: {error}");
                GameEvents.InvokeAdFailed(AdType.Interstitial, error.ToString());
                LoadInterstitialAd();
            };
        }

        /// <summary>
        /// Register rewarded ad events
        /// </summary>
        private void RegisterRewardedEvents(RewardedAd ad)
        {
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded ad closed");
                GameEvents.InvokeAdClosed(AdType.Rewarded);
                LoadRewardedAd(); // Reload for next time
            };

            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogWarning($"Rewarded ad failed to display: {error}");
                GameEvents.InvokeAdFailed(AdType.Rewarded, error.ToString());
                LoadRewardedAd();
            };
        }

        private string GetBannerAdUnitId()
        {
#if UNITY_ANDROID
            return androidBannerId;
#elif UNITY_IOS
            return iosBannerId;
#else
            return "ca-app-pub-3940256099942544/6300978111"; // Test ID
#endif
        }

        private string GetInterstitialAdUnitId()
        {
#if UNITY_ANDROID
            return androidInterstitialId;
#elif UNITY_IOS
            return iosInterstitialId;
#else
            return "ca-app-pub-3940256099942544/1033173712"; // Test ID
#endif
        }

        private string GetRewardedAdUnitId()
        {
#if UNITY_ANDROID
            return androidRewardedId;
#elif UNITY_IOS
            return iosRewardedId;
#else
            return "ca-app-pub-3940256099942544/5224354917"; // Test ID
#endif
        }

        private void OnDestroy()
        {
            if (bannerView != null)
                bannerView.Destroy();
        }
    }
}
