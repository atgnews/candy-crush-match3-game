using UnityEngine;
using UnityEngine.UI;
using CandyCrush.Core;
using CandyCrush.Events;
using CandyCrush.Gameplay;

namespace CandyCrush.UI
{
    /// <summary>
    /// In-game HUD showing score, moves, time, and lives
    /// </summary>
    public class GameplayHUD : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text movesText;
        [SerializeField] private Text timeText;
        [SerializeField] private Text livesText;
        [SerializeField] private Image livesBar;
        [SerializeField] private GameplayController gameplayController;

        private void OnEnable()
        {
            GameEvents.OnScoreChanged += UpdateScore;
            GameEvents.OnMovesChanged += UpdateMoves;
            GameEvents.OnTimeChanged += UpdateTime;
            GameEvents.OnLivesChanged += UpdateLives;
        }

        private void OnDisable()
        {
            GameEvents.OnScoreChanged -= UpdateScore;
            GameEvents.OnMovesChanged -= UpdateMoves;
            GameEvents.OnTimeChanged -= UpdateTime;
            GameEvents.OnLivesChanged -= UpdateLives;
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null)
                scoreText.text = $"Score: {score}";
        }

        private void UpdateMoves(int moves)
        {
            if (movesText != null)
                movesText.text = $"Moves: {moves}";
        }

        private void UpdateTime(float time)
        {
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }

        private void UpdateLives(int lives)
        {
            if (livesText != null)
                livesText.text = $"Lives: {lives}";

            if (livesBar != null)
            {
                livesBar.fillAmount = lives / 5f; // Assuming max 5 lives
            }
        }
    }
}
