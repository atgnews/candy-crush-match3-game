using UnityEngine;
using UnityEngine.UI;
using CandyCrush.Core;
using CandyCrush.Events;
using CandyCrush.Gameplay;

namespace CandyCrush.UI
{
    /// <summary>
    /// Post-level results screen showing stars, rewards, and next level button
    /// </summary>
    public class LevelResultsScreen : MonoBehaviour
    {
        [SerializeField] private Text levelNameText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text targetScoreText;
        [SerializeField] private Image[] starImages;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Text coinsRewardText;
        [SerializeField] private CanvasGroup canvasGroup;

        private int starsEarned = 0;
        private int coinsEarned = 0;

        private void OnEnable()
        {
            GameEvents.OnLevelCompleted += ShowCompletionScreen;
            GameEvents.OnLevelFailed += ShowFailureScreen;
        }

        private void OnDisable()
        {
            GameEvents.OnLevelCompleted -= ShowCompletionScreen;
            GameEvents.OnLevelFailed -= ShowFailureScreen;
        }

        private void Start()
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            retryButton.onClick.AddListener(OnRetryClicked);
        }

        private void ShowCompletionScreen()
        {
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;

            levelNameText.text = $"Level {LevelManager.Instance.CurrentLevelId} Complete!";
            scoreText.text = $"Score: {GameplayController.Instance.Score}";
            coinsEarned = CalculateCoinsReward(starsEarned);
            coinsRewardText.text = $"+ {coinsEarned} Coins";

            DisplayStars(starsEarned);
        }

        private void ShowFailureScreen()
        {
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;

            levelNameText.text = $"Level {LevelManager.Instance.CurrentLevelId} Failed";
            scoreText.text = $"Score: {GameplayController.Instance.Score}";
            nextLevelButton.gameObject.SetActive(false);
            retryButton.gameObject.SetActive(true);
        }

        private void DisplayStars(int count)
        {
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null)
                {
                    starImages[i].enabled = i < count;
                }
            }
        }

        private int CalculateCoinsReward(int stars)
        {
            return stars * 500; // 500 coins per star
        }

        private void OnNextLevelClicked()
        {
            LevelManager.Instance.CompleteCurrentLevel();
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameplayScene");
        }

        private void OnRetryClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameplayScene");
        }
    }
}
