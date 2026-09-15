using UnityEngine;
using UnityEngine.UI;
using CandyCrush.Core;
using CandyCrush.Events;
using CandyCrush.Gameplay;

namespace CandyCrush.UI
{
    /// <summary>
    /// Leaderboard display screen
    /// </summary>
    public class LeaderboardScreen : MonoBehaviour
    {
        [SerializeField] private Transform leaderboardContent;
        [SerializeField] private Text playerRankText;
        [SerializeField] private Text playerScoreText;
        [SerializeField] private Prefab leaderboardEntryPrefab;
        [SerializeField] private Button globalButton;
        [SerializeField] private Button friendsButton;
        [SerializeField] private Button weeklyButton;

        private void Start()
        {
            globalButton.onClick.AddListener(() => LoadLeaderboard("global"));
            friendsButton.onClick.AddListener(() => LoadLeaderboard("friends"));
            weeklyButton.onClick.AddListener(() => LoadLeaderboard("weekly"));

            LoadLeaderboard("global");
        }

        private void LoadLeaderboard(string type)
        {
            // TODO: Implement actual leaderboard loading from Firebase
            Debug.Log($"Loading {type} leaderboard");
            
            // Clear existing entries
            foreach (Transform child in leaderboardContent)
            {
                Destroy(child.gameObject);
            }
        }

        private void UpdatePlayerInfo(int rank, int score)
        {
            playerRankText.text = $"Your Rank: #{rank}";
            playerScoreText.text = $"Score: {score}";
        }
    }
}
