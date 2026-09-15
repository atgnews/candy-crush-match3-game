using Firebase.Firestore;
using System;

namespace CandyCrush.Network.Data
{
    /// <summary>
    /// User profile data stored in Firestore
    /// </summary>
    [FirestoreData]
    public class UserProfile
    {
        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string Username { get; set; }

        [FirestoreProperty]
        public int Level { get; set; }

        [FirestoreProperty]
        public int TotalScore { get; set; }

        [FirestoreProperty]
        public int Coins { get; set; }

        [FirestoreProperty]
        public int Gems { get; set; }

        [FirestoreProperty]
        public int Lives { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty]
        public DateTime LastPlayedAt { get; set; }

        [FirestoreProperty]
        public int CurrentLevelId { get; set; }

        [FirestoreProperty]
        public int GlobalRank { get; set; }

        [FirestoreProperty]
        public int Trophies { get; set; }

        [FirestoreProperty]
        public string TeamId { get; set; }

        public UserProfile() { }

        public UserProfile(string userId, string username)
        {
            UserId = userId;
            Username = username;
            Level = 1;
            TotalScore = 0;
            Coins = 500; // Starting coins
            Gems = 0;
            Lives = 5;
            CreatedAt = DateTime.Now;
            LastPlayedAt = DateTime.Now;
            CurrentLevelId = 1;
            GlobalRank = 0;
            Trophies = 0;
            TeamId = "";
        }
    }

    /// <summary>
    /// Level completion data
    /// </summary>
    [FirestoreData]
    public class LevelProgress
    {
        [FirestoreProperty]
        public int LevelId { get; set; }

        [FirestoreProperty]
        public bool Completed { get; set; }

        [FirestoreProperty]
        public int StarsEarned { get; set; }

        [FirestoreProperty]
        public int BestScore { get; set; }

        [FirestoreProperty]
        public DateTime CompletedAt { get; set; }

        [FirestoreProperty]
        public int Attempts { get; set; }
    }

    /// <summary>
    /// Task progress data
    /// </summary>
    [FirestoreData]
    public class TaskProgress
    {
        [FirestoreProperty]
        public string TaskId { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public int CurrentProgress { get; set; }

        [FirestoreProperty]
        public int Goal { get; set; }

        [FirestoreProperty]
        public bool Completed { get; set; }

        [FirestoreProperty]
        public bool Claimed { get; set; }

        [FirestoreProperty]
        public DateTime StartedAt { get; set; }

        [FirestoreProperty]
        public DateTime? CompletedAt { get; set; }

        [FirestoreProperty]
        public string RewardType { get; set; } // coins, gems, booster

        [FirestoreProperty]
        public int RewardAmount { get; set; }

        public TaskProgress() { }

        public TaskProgress(string taskId, string name, int goal)
        {
            TaskId = taskId;
            Name = name;
            Goal = goal;
            CurrentProgress = 0;
            Completed = false;
            Claimed = false;
            StartedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Team data
    /// </summary>
    [FirestoreData]
    public class TeamData
    {
        [FirestoreProperty]
        public string TeamId { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public int Level { get; set; }

        [FirestoreProperty]
        public int XP { get; set; }

        [FirestoreProperty]
        public string LeaderId { get; set; }

        [FirestoreProperty]
        public int MemberCount { get; set; }

        [FirestoreProperty]
        public int TotalScore { get; set; }

        [FirestoreProperty]
        public int WeeklyScore { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty]
        public string Badge { get; set; }

        public TeamData() { }

        public TeamData(string name, string leaderId)
        {
            Name = name;
            LeaderId = leaderId;
            Level = 1;
            XP = 0;
            MemberCount = 1;
            TotalScore = 0;
            WeeklyScore = 0;
            CreatedAt = DateTime.Now;
            Badge = "Bronze";
        }
    }

    /// <summary>
    /// Leaderboard entry
    /// </summary>
    [FirestoreData]
    public class LeaderboardEntry
    {
        [FirestoreProperty]
        public int Rank { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string Username { get; set; }

        [FirestoreProperty]
        public int Score { get; set; }

        [FirestoreProperty]
        public int Level { get; set; }

        [FirestoreProperty]
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Transaction record for IAP and rewards
    /// </summary>
    [FirestoreData]
    public class Transaction
    {
        [FirestoreProperty]
        public string TransactionId { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string Type { get; set; } // iap, ad_reward, task_reward, daily_login

        [FirestoreProperty]
        public int Amount { get; set; }

        [FirestoreProperty]
        public string CurrencyType { get; set; } // coins, gems

        [FirestoreProperty]
        public string ProductId { get; set; }

        [FirestoreProperty]
        public string Status { get; set; } // pending, completed, failed

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        public Transaction() { }

        public Transaction(string userId, string type, int amount, string currencyType)
        {
            TransactionId = Guid.NewGuid().ToString();
            UserId = userId;
            Type = type;
            Amount = amount;
            CurrencyType = currencyType;
            Status = "pending";
            CreatedAt = DateTime.Now;
        }
    }
}
