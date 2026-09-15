using System;
using CandyCrush.Core;

namespace CandyCrush.Events
{
    /// <summary>
    /// Global game events system
    /// </summary>
    public static class GameEvents
    {
        // Gameplay Events
        public static event Action<int, int> OnCandySwapped;              // (fromX, toX)
        public static event Action<int, int> OnCandyMatched;              // (row, col)
        public static event Action OnCascadeStarted;
        public static event Action OnCascadeEnded;
        public static event Action<int> OnScoreChanged;                   // (newScore)
        public static event Action<int> OnMovesChanged;                   // (remainingMoves)
        public static event Action<float> OnTimeChanged;                  // (remainingTime)
        public static event Action<GameState> OnGameStateChanged;

        // Level Events
        public static event Action OnLevelLoaded;
        public static event Action OnLevelCompleted;
        public static event Action OnLevelFailed;
        public static event Action<int> OnStarsEarned;                    // (starCount)

        // Task Events
        public static event Action<string> OnTaskProgress;                // (taskId)
        public static event Action<string> OnTaskCompleted;               // (taskId)
        public static event Action<string, int> OnTaskRewarded;           // (taskId, reward)

        // Team Events
        public static event Action<string> OnTeamCreated;                 // (teamId)
        public static event Action<string> OnTeamJoined;                  // (teamId)
        public static event Action<string, int> OnTeamMilestoneReached;   // (teamId, milestone%)
        public static event Action<string, string> OnTeamMessageReceived; // (teamId, message)

        // Leaderboard Events
        public static event Action<int> OnRankChanged;                    // (newRank)
        public static event Action OnLeaderboardUpdated;

        // Inventory Events
        public static event Action<int> OnCoinsChanged;                   // (newAmount)
        public static event Action<int> OnGemsChanged;                    // (newAmount)
        public static event Action<int> OnLivesChanged;                   // (newAmount)
        public static event Action<BoosterType, int> OnBoosterChanged;    // (type, amount)

        // IAP Events
        public static event Action<string> OnPurchaseSucceeded;           // (productId)
        public static event Action<string, string> OnPurchaseFailed;      // (productId, error)
        public static event Action OnSubscriptionActivated;

        // Ad Events
        public static event Action<AdType> OnAdLoaded;                    // (adType)
        public static event Action<AdType, string> OnAdFailed;            // (adType, error)
        public static event Action<AdType> OnAdDisplayed;                 // (adType)
        public static event Action<AdType> OnAdClosed;                    // (adType)
        public static event Action<AdType> OnAdRewarded;                  // (adType)

        // Notification Events
        public static event Action<string> OnNotificationReceived;        // (message)
        public static event Action OnDailyRewardAvailable;

        // Network Events
        public static event Action OnConnectionLost;
        public static event Action OnConnectionRestored;
        public static event Action<string> OnSyncError;                   // (errorMessage)

        // Invoke Methods
        public static void InvokeCandySwapped(int fromX, int toX) => OnCandySwapped?.Invoke(fromX, toX);
        public static void InvokeCandyMatched(int row, int col) => OnCandyMatched?.Invoke(row, col);
        public static void InvokeCascadeStarted() => OnCascadeStarted?.Invoke();
        public static void InvokeCascadeEnded() => OnCascadeEnded?.Invoke();
        public static void InvokeScoreChanged(int newScore) => OnScoreChanged?.Invoke(newScore);
        public static void InvokeMovesChanged(int remainingMoves) => OnMovesChanged?.Invoke(remainingMoves);
        public static void InvokeTimeChanged(float remainingTime) => OnTimeChanged?.Invoke(remainingTime);
        public static void InvokeGameStateChanged(GameState newState) => OnGameStateChanged?.Invoke(newState);
        public static void InvokeLevelLoaded() => OnLevelLoaded?.Invoke();
        public static void InvokeLevelCompleted() => OnLevelCompleted?.Invoke();
        public static void InvokeLevelFailed() => OnLevelFailed?.Invoke();
        public static void InvokeStarsEarned(int starCount) => OnStarsEarned?.Invoke(starCount);
        public static void InvokeTaskProgress(string taskId) => OnTaskProgress?.Invoke(taskId);
        public static void InvokeTaskCompleted(string taskId) => OnTaskCompleted?.Invoke(taskId);
        public static void InvokeTaskRewarded(string taskId, int reward) => OnTaskRewarded?.Invoke(taskId, reward);
        public static void InvokeCoinsChanged(int newAmount) => OnCoinsChanged?.Invoke(newAmount);
        public static void InvokeGemsChanged(int newAmount) => OnGemsChanged?.Invoke(newAmount);
        public static void InvokeLivesChanged(int newAmount) => OnLivesChanged?.Invoke(newAmount);
        public static void InvokeBoosterChanged(BoosterType type, int amount) => OnBoosterChanged?.Invoke(type, amount);
        public static void InvokePurchaseSucceeded(string productId) => OnPurchaseSucceeded?.Invoke(productId);
        public static void InvokePurchaseFailed(string productId, string error) => OnPurchaseFailed?.Invoke(productId, error);
        public static void InvokeSubscriptionActivated() => OnSubscriptionActivated?.Invoke();
        public static void InvokeAdLoaded(AdType adType) => OnAdLoaded?.Invoke(adType);
        public static void InvokeAdFailed(AdType adType, string error) => OnAdFailed?.Invoke(adType, error);
        public static void InvokeAdDisplayed(AdType adType) => OnAdDisplayed?.Invoke(adType);
        public static void InvokeAdClosed(AdType adType) => OnAdClosed?.Invoke(adType);
        public static void InvokeAdRewarded(AdType adType) => OnAdRewarded?.Invoke(adType);
        public static void InvokeNotificationReceived(string message) => OnNotificationReceived?.Invoke(message);
        public static void InvokeDailyRewardAvailable() => OnDailyRewardAvailable?.Invoke();
        public static void InvokeConnectionLost() => OnConnectionLost?.Invoke();
        public static void InvokeConnectionRestored() => OnConnectionRestored?.Invoke();
        public static void InvokeSyncError(string errorMessage) => OnSyncError?.Invoke(errorMessage);
    }
}
