using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine;
using CandyCrush.Core;
using CandyCrush.Events;

namespace CandyCrush.Network
{
    /// <summary>
    /// Firebase service for player data synchronization
    /// </summary>
    public class FirebaseService : Singleton<FirebaseService>
    {
        private FirebaseFirestore db;
        private string currentUserId;

        protected override void Awake()
        {
            base.Awake();
            InitializeFirebase();
        }

        private void InitializeFirebase()
        {
            try
            {
                db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firebase Firestore initialized");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Firebase initialization failed: {e.Message}");
                GameEvents.InvokeSyncError("Firebase initialization failed");
            }
        }

        /// <summary>
        /// Set current user ID
        /// </summary>
        public void SetCurrentUser(string userId)
        {
            currentUserId = userId;
        }

        /// <summary>
        /// Load user profile from Firestore
        /// </summary>
        public async Task<Data.UserProfile> LoadUserProfile(string userId)
        {
            try
            {
                DocumentReference docRef = db.Collection("users").Document(userId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    return snapshot.ConvertTo<Data.UserProfile>();
                }
                else
                {
                    Debug.LogWarning($"User profile not found: {userId}");
                    return null;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load user profile: {e.Message}");
                GameEvents.InvokeSyncError($"Failed to load profile: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Save user profile to Firestore
        /// </summary>
        public async Task<bool> SaveUserProfile(Data.UserProfile profile)
        {
            try
            {
                await db.Collection("users").Document(profile.UserId).SetAsync(profile);
                Debug.Log($"User profile saved: {profile.UserId}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save user profile: {e.Message}");
                GameEvents.InvokeSyncError($"Failed to save profile: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update player coins
        /// </summary>
        public async Task<bool> UpdateCoins(int amount)
        {
            try
            {
                DocumentReference docRef = db.Collection("users").Document(currentUserId);
                await docRef.UpdateAsync("Coins", FieldValue.Increment(amount));
                GameEvents.InvokeCoinsChanged(0); // UI should fetch updated value
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to update coins: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update player gems
        /// </summary>
        public async Task<bool> UpdateGems(int amount)
        {
            try
            {
                DocumentReference docRef = db.Collection("users").Document(currentUserId);
                await docRef.UpdateAsync("Gems", FieldValue.Increment(amount));
                GameEvents.InvokeGemsChanged(0);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to update gems: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update player score and level
        /// </summary>
        public async Task<bool> UpdateScore(int score)
        {
            try
            {
                DocumentReference docRef = db.Collection("users").Document(currentUserId);
                await docRef.UpdateAsync(
                    "TotalScore", FieldValue.Increment(score),
                    "LastPlayedAt", FieldValue.ServerTimestamp()
                );
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to update score: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Save level completion
        /// </summary>
        public async Task<bool> SaveLevelCompletion(int levelId, int stars, int score)
        {
            try
            {
                DocumentReference docRef = db.Collection("users").Document(currentUserId)
                    .Collection("levelProgress").Document(levelId.ToString());

                var progressData = new
                {
                    LevelId = levelId,
                    Completed = true,
                    StarsEarned = stars,
                    BestScore = score,
                    CompletedAt = FieldValue.ServerTimestamp(),
                    Attempts = FieldValue.Increment(1)
                };

                await docRef.SetAsync(progressData, SetOptions.MergeAll);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save level completion: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update task progress
        /// </summary>
        public async Task<bool> UpdateTaskProgress(string taskId, int progress)
        {
            try
            {
                DocumentReference docRef = db.Collection("users").Document(currentUserId)
                    .Collection("tasks").Document(taskId);

                await docRef.UpdateAsync("CurrentProgress", progress);
                GameEvents.InvokeTaskProgress(taskId);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to update task progress: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get global leaderboard
        /// </summary>
        public async Task<Data.LeaderboardEntry[]> GetGlobalLeaderboard(int limit = 100)
        {
            try
            {
                Query query = db.Collection("leaderboards").Document("global")
                    .Collection("entries")
                    .OrderByDescending("Score")
                    .Limit(limit);

                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                var entries = new Data.LeaderboardEntry[snapshot.Count];

                int index = 0;
                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    entries[index++] = doc.ConvertTo<Data.LeaderboardEntry>();
                }

                return entries;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to fetch leaderboard: {e.Message}");
                return new Data.LeaderboardEntry[0];
            }
        }

        /// <summary>
        /// Get player rank
        /// </summary>
        public async Task<int> GetPlayerRank()
        {
            try
            {
                DocumentReference docRef = db.Collection("users").Document(currentUserId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists && snapshot.Contains("GlobalRank"))
                {
                    return snapshot.GetValue<int>("GlobalRank");
                }

                return 0;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to fetch player rank: {e.Message}");
                return 0;
            }
        }
    }
}
