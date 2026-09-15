using Firebase.Analytics;
using CandyCrush.Core;
using CandyCrush.Events;
using UnityEngine;

namespace CandyCrush.Analytics
{
    /// <summary>
    /// Analytics tracking for gameplay, monetization, and retention metrics
    /// </summary>
    public class AnalyticsService : Singleton<AnalyticsService>
    {
        protected override void Awake()
        {
            base.Awake();
            InitializeAnalytics();
        }

        private void OnEnable()
        {
            // Subscribe to events
            GameEvents.OnLevelCompleted += OnLevelCompleted;
            GameEvents.OnLevelFailed += OnLevelFailed;
            GameEvents.OnTaskCompleted += OnTaskCompleted;
            GameEvents.OnPurchaseSucceeded += OnPurchaseSucceeded;
            GameEvents.OnPurchaseFailed += OnPurchaseFailed;
            GameEvents.OnAdRewarded += OnAdRewarded;
            GameEvents.OnSubscriptionActivated += OnSubscriptionActivated;
        }

        private void OnDisable()
        {
            GameEvents.OnLevelCompleted -= OnLevelCompleted;
            GameEvents.OnLevelFailed -= OnLevelFailed;
            GameEvents.OnTaskCompleted -= OnTaskCompleted;
            GameEvents.OnPurchaseSucceeded -= OnPurchaseSucceeded;
            GameEvents.OnPurchaseFailed -= OnPurchaseFailed;
            GameEvents.OnAdRewarded -= OnAdRewarded;
            GameEvents.OnSubscriptionActivated -= OnSubscriptionActivated;
        }

        /// <summary>
        /// Initialize Firebase Analytics
        /// </summary>
        private void InitializeAnalytics()
        {
            // Firebase Analytics auto-initializes on first use
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            Debug.Log("Analytics initialized");
        }

        /// <summary>
        /// Log level completion event
        /// </summary>
        private void OnLevelCompleted()
        {
            var parameters = new Parameter[]
            {
                new Parameter("level_id", LevelManager.Instance.CurrentLevelId),
                new Parameter("score", GameplayController.Instance?.Score ?? 0),
                new Parameter("difficulty", "medium") // TODO: Get actual difficulty
            };

            FirebaseAnalytics.LogEvent("level_completed", parameters);
            Debug.Log("Analytics: level_completed");
        }

        /// <summary>
        /// Log level failure event
        /// </summary>
        private void OnLevelFailed()
        {
            var parameters = new Parameter[]
            {
                new Parameter("level_id", LevelManager.Instance.CurrentLevelId),
                new Parameter("score", GameplayController.Instance?.Score ?? 0),
                new Parameter("reason", "out_of_moves")
            };

            FirebaseAnalytics.LogEvent("level_failed", parameters);
            Debug.Log("Analytics: level_failed");
        }

        /// <summary>
        /// Log task completion event
        /// </summary>
        private void OnTaskCompleted(string taskId)
        {
            var parameters = new Parameter[]
            {
                new Parameter("task_id", taskId),
                new Parameter("task_type", "daily") // TODO: Get actual task type
            };

            FirebaseAnalytics.LogEvent("task_completed", parameters);
            Debug.Log($"Analytics: task_completed - {taskId}");
        }

        /// <summary>
        /// Log purchase event
        /// </summary>
        private void OnPurchaseSucceeded(string productId)
        {
            var parameters = new Parameter[]
            {
                new Parameter("product_id", productId),
                new Parameter("transaction_type", "iap")
            };

            FirebaseAnalytics.LogEvent("purchase_successful", parameters);
            Debug.Log($"Analytics: purchase_successful - {productId}");
        }

        /// <summary>
        /// Log purchase failure
        /// </summary>
        private void OnPurchaseFailed(string productId, string error)
        {
            var parameters = new Parameter[]
            {
                new Parameter("product_id", productId),
                new Parameter("error_reason", error)
            };

            FirebaseAnalytics.LogEvent("purchase_failed", parameters);
            Debug.Log($"Analytics: purchase_failed - {productId}");
        }

        /// <summary>
        /// Log ad reward event
        /// </summary>
        private void OnAdRewarded(AdType adType)
        {
            var parameters = new Parameter[]
            {
                new Parameter("ad_type", adType.ToString()),
                new Parameter("reward_type", "coins") // TODO: Get actual reward type
            };

            FirebaseAnalytics.LogEvent("ad_reward_earned", parameters);
            Debug.Log($"Analytics: ad_reward_earned - {adType}");
        }

        /// <summary>
        /// Log subscription activation
        /// </summary>
        private void OnSubscriptionActivated()
        {
            var parameters = new Parameter[]
            {
                new Parameter("subscription_type", "premium_monthly"),
                new Parameter("price", "299")
            };

            FirebaseAnalytics.LogEvent("subscription_activated", parameters);
            Debug.Log("Analytics: subscription_activated");
        }

        /// <summary>
        /// Log custom event
        /// </summary>
        public void LogCustomEvent(string eventName, Parameter[] parameters = null)
        {
            if (parameters != null)
            {
                FirebaseAnalytics.LogEvent(eventName, parameters);
            }
            else
            {
                FirebaseAnalytics.LogEvent(eventName);
            }

            Debug.Log($"Analytics: {eventName}");
        }

        /// <summary>
        /// Set user properties
        /// </summary>
        public void SetUserProperty(string name, string value)
        {
            FirebaseAnalytics.SetUserProperty(name, value);
            Debug.Log($"Analytics: Set user property {name} = {value}");
        }
    }
}
