using UnityEngine;
using System.Collections;
using CandyCrush.Core;
using CandyCrush.Events;

namespace CandyCrush.Gameplay
{
    /// <summary>
    /// Main gameplay controller handling level flow
    /// </summary>
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private GridSystem gridSystem;
        [SerializeField] private LevelData levelData;
        [SerializeField] private float cascadeDelay = 0.3f;

        private MatchDetectionEngine matchEngine;
        private GameState currentState = GameState.Loading;
        private int currentScore = 0;
        private int movesRemaining = 0;
        private float timeRemaining = 0f;
        private bool canInput = false;

        private Candy selectedCandy;
        private Candy targetCandy;

        public GameState CurrentState => currentState;
        public int Score => currentScore;
        public int MovesRemaining => movesRemaining;
        public float TimeRemaining => timeRemaining;

        private void Awake()
        {
            if (gridSystem == null)
                gridSystem = GetComponent<GridSystem>();
        }

        private void Start()
        {
            InitializeLevel();
        }

        private void Update()
        {
            if (currentState == GameState.Playing)
            {
                HandleInput();
                UpdateTimer();
            }
        }

        /// <summary>
        /// Initialize level from level data
        /// </summary>
        public void InitializeLevel()
        {
            SetGameState(GameState.Loading);

            // Initialize grid
            gridSystem.InitializeGrid(levelData?.GetGridData());
            matchEngine = new MatchDetectionEngine(gridSystem);

            // Set level parameters
            currentScore = 0;
            movesRemaining = levelData?.GetMoveLimit() ?? 0;
            timeRemaining = levelData?.GetTimeLimit() ?? 0f;

            // Update UI
            GameEvents.InvokeScoreChanged(currentScore);
            GameEvents.InvokeMovesChanged(movesRemaining);
            GameEvents.InvokeLevelLoaded();

            // Clear any initial matches
            HandleCascade();
            SetGameState(GameState.Playing);
            canInput = true;
        }

        /// <summary>
        /// Handle player input for candy selection and swapping
        /// </summary>
        private void HandleInput()
        {
            if (!canInput)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

                if (gridSystem.WorldToGridPosition(worldPos, out int x, out int y))
                {
                    if (selectedCandy == null)
                    {
                        selectedCandy = gridSystem.GetCandy(x, y);
                    }
                    else if (selectedCandy.GridX == x && selectedCandy.GridY == y)
                    {
                        // Deselect
                        selectedCandy = null;
                    }
                    else
                    {
                        targetCandy = gridSystem.GetCandy(x, y);
                        AttemptSwap(selectedCandy, targetCandy);
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to swap two candies
        /// </summary>
        private void AttemptSwap(Candy candy1, Candy candy2)
        {
            if (candy1 == null || candy2 == null)
            {
                selectedCandy = null;
                return;
            }

            // Check if adjacent
            if (Mathf.Abs(candy1.GridX - candy2.GridX) + Mathf.Abs(candy1.GridY - candy2.GridY) != 1)
            {
                selectedCandy = candy2; // Select new candy
                return;
            }

            // Perform swap
            canInput = false;
            Vector3 pos1 = candy1.transform.position;
            Vector3 pos2 = candy2.transform.position;

            candy1.AnimateSwap(pos2);
            candy2.AnimateSwap(pos1);

            gridSystem.TrySwapCandies(candy1.GridX, candy1.GridY, candy2.GridX, candy2.GridY);

            GameEvents.InvokeCandySwapped(candy1.GridX, candy2.GridX);

            StartCoroutine(ValidateSwapCoroutine(candy1, candy2));
            selectedCandy = null;
        }

        /// <summary>
        /// Validate swap and handle cascade
        /// </summary>
        private IEnumerator ValidateSwapCoroutine(Candy candy1, Candy candy2)
        {
            // Wait for animation
            while (candy1.IsAnimating || candy2.IsAnimating)
                yield return null;

            // Check for matches
            MatchResult result = matchEngine.DetectMatches();
            if (result.HasMatches)
            {
                movesRemaining--;
                GameEvents.InvokeMovesChanged(movesRemaining);
                yield return HandleCascadeCoroutine(result);
            }
            else
            {
                // No match - undo swap
                Vector3 pos1 = candy1.transform.position;
                Vector3 pos2 = candy2.transform.position;

                candy1.AnimateSwap(pos2);
                candy2.AnimateSwap(pos1);

                gridSystem.TrySwapCandies(candy1.GridX, candy1.GridY, candy2.GridX, candy2.GridY);

                while (candy1.IsAnimating || candy2.IsAnimating)
                    yield return null;
            }

            canInput = true;
            CheckLevelComplete();
        }

        /// <summary>
        /// Handle cascade/combo logic
        /// </summary>
        private IEnumerator HandleCascadeCoroutine(MatchResult result)
        {
            GameEvents.InvokeCascadeStarted();
            int cascadeCount = 0;

            while (result.HasMatches)
            {
                cascadeCount++;
                int cascadeScore = result.BaseScore * cascadeCount; // Multiplier for cascades
                currentScore += cascadeScore;
                GameEvents.InvokeScoreChanged(currentScore);

                // Apply special candies
                foreach (var (candy, type) in result.SpecialCandiesCreated)
                {
                    candy.SetSpecialType(type);
                }

                // Clear matches
                gridSystem.ClearMatches(result.MatchedCandies);

                yield return new WaitForSeconds(cascadeDelay);

                // Apply gravity
                gridSystem.ApplyGravity();
                yield return new WaitForSeconds(cascadeDelay);

                // Fill empty spaces
                gridSystem.FillEmptySpaces();
                yield return new WaitForSeconds(cascadeDelay);

                // Check for new matches
                result = matchEngine.DetectMatches();
            }

            GameEvents.InvokeCascadeEnded();
        }

        /// <summary>
        /// Handle cascade of matches
        /// </summary>
        private void HandleCascade()
        {
            StartCoroutine(HandleCascadeCoroutine(matchEngine.DetectMatches()));
        }

        /// <summary>
        /// Update timer for timed levels
        /// </summary>
        private void UpdateTimer()
        {
            if (levelData.GetTimeLimit() > 0)
            {
                timeRemaining -= Time.deltaTime;
                GameEvents.InvokeTimeChanged(timeRemaining);

                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    CheckLevelComplete();
                }
            }
        }

        /// <summary>
        /// Check if level is complete or failed
        /// </summary>
        private void CheckLevelComplete()
        {
            if (levelData == null)
                return;

            bool isComplete = false;
            bool isFailed = false;

            // Check move-based level
            if (levelData.GetMoveLimit() > 0 && movesRemaining <= 0)
            {
                isFailed = currentScore < levelData.GetTargetScores()[0];
                isComplete = !isFailed;
            }

            // Check time-based level
            if (levelData.GetTimeLimit() > 0 && timeRemaining <= 0)
            {
                isFailed = currentScore < levelData.GetTargetScores()[0];
                isComplete = !isFailed;
            }

            if (isComplete)
                CompleteLevel();
            else if (isFailed)
                FailLevel();
        }

        /// <summary>
        /// Handle level completion
        /// </summary>
        private void CompleteLevel()
        {
            SetGameState(GameState.LevelComplete);
            canInput = false;
            int starsEarned = GetStarsEarned();
            GameEvents.InvokeStarsEarned(starsEarned);
            GameEvents.InvokeLevelCompleted();
        }

        /// <summary>
        /// Handle level failure
        /// </summary>
        private void FailLevel()
        {
            SetGameState(GameState.LevelFailed);
            canInput = false;
            GameEvents.InvokeLevelFailed();
        }

        /// <summary>
        /// Calculate stars earned based on score
        /// </summary>
        private int GetStarsEarned()
        {
            int[] targetScores = levelData.GetTargetScores();

            if (currentScore >= targetScores[2])
                return 3;
            if (currentScore >= targetScores[1])
                return 2;
            if (currentScore >= targetScores[0])
                return 1;
            return 0;
        }

        /// <summary>
        /// Set game state and trigger event
        /// </summary>
        private void SetGameState(GameState newState)
        {
            if (currentState != newState)
            {
                currentState = newState;
                GameEvents.InvokeGameStateChanged(newState);
            }
        }
    }
}
