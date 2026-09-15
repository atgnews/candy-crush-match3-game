using UnityEngine;
using System.Collections.Generic;
using CandyCrush.Core;

namespace CandyCrush.Gameplay
{
    /// <summary>
    /// Serializable level data configuration
    /// </summary>
    [System.Serializable]
    public class LevelData
    {
        public int levelId;
        public int world;
        public int episode;
        public LevelType type;
        public Difficulty difficulty;
        public int moveLimit;
        public int timeLimit;
        public int[] targetScores = new int[3]; // 1-star, 2-star, 3-star
        public CandyType[] gridData = new CandyType[64]; // 8x8
        public BlockerType[] blockers = new BlockerType[64];
        public string description;
        public string[] allowedBoosters;
        public string[] specialGoals;

        public int GetMoveLimit() => moveLimit;
        public int GetTimeLimit() => timeLimit;
        public int[] GetTargetScores() => targetScores;
        public CandyType[] GetGridData() => gridData;
    }

    /// <summary>
    /// Manages level progression and data loading
    /// </summary>
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField] private TextAsset[] levelDataFiles; // JSON files with level data
        private Dictionary<int, LevelData> levels = new Dictionary<int, LevelData>();
        private int currentLevelId = 1;
        private int completedLevels = 0;

        public int CurrentLevelId => currentLevelId;
        public int CompletedLevels => completedLevels;

        protected override void Awake()
        {
            base.Awake();
            LoadAllLevels();
        }

        /// <summary>
        /// Load all level data from JSON files
        /// </summary>
        private void LoadAllLevels()
        {
            if (levelDataFiles != null)
            {
                foreach (var file in levelDataFiles)
                {
                    var levelDataWrapper = JsonUtility.FromJson<LevelDataWrapper>(file.text);
                    if (levelDataWrapper?.levels != null)
                    {
                        foreach (var level in levelDataWrapper.levels)
                        {
                            levels[level.levelId] = level;
                        }
                    }
                }
            }

            Debug.Log($"Loaded {levels.Count} levels");
        }

        /// <summary>
        /// Get level data by ID
        /// </summary>
        public LevelData GetLevel(int levelId)
        {
            if (levels.TryGetValue(levelId, out var level))
                return level;

            Debug.LogWarning($"Level {levelId} not found");
            return CreateDefaultLevel(levelId);
        }

        /// <summary>
        /// Set current level
        /// </summary>
        public void SetCurrentLevel(int levelId)
        {
            if (levels.ContainsKey(levelId) || levelId <= 0)
            {
                currentLevelId = levelId;
            }
        }

        /// <summary>
        /// Get next level
        /// </summary>
        public LevelData GetNextLevel()
        {
            return GetLevel(currentLevelId + 1);
        }

        /// <summary>
        /// Complete current level
        /// </summary>
        public void CompleteCurrentLevel()
        {
            completedLevels++;
            currentLevelId++;
        }

        /// <summary>
        /// Create default level if not found
        /// </summary>
        private LevelData CreateDefaultLevel(int levelId)
        {
            var level = new LevelData
            {
                levelId = levelId,
                world = (levelId - 1) / 20 + 1,
                episode = (levelId - 1) % 20 + 1,
                type = LevelType.Jelly,
                difficulty = GetDifficultyForLevel(levelId),
                moveLimit = 25,
                timeLimit = 0,
                targetScores = new int[] { 5000, 10000, 15000 },
                description = $"Level {levelId}",
                allowedBoosters = new string[] { "lollipop", "striped" }
            };

            // Generate random grid
            for (int i = 0; i < 64; i++)
            {
                level.gridData[i] = (CandyType)Random.Range(1, 7);
            }

            return level;
        }

        /// <summary>
        /// Get difficulty based on level number
        /// </summary>
        private Difficulty GetDifficultyForLevel(int levelId)
        {
            if (levelId <= 30) return Difficulty.Easy;
            if (levelId <= 100) return Difficulty.Medium;
            if (levelId <= 200) return Difficulty.Hard;
            return Difficulty.Expert;
        }

        [System.Serializable]
        private class LevelDataWrapper
        {
            public List<LevelData> levels = new List<LevelData>();
        }
    }
}
