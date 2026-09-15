using UnityEngine;
using System.Collections.Generic;
using CandyCrush.Core;
using CandyCrush.Events;

namespace CandyCrush.Gameplay
{
    /// <summary>
    /// Manages player inventory (coins, gems, lives, boosters)
    /// </summary>
    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private int maxLives = 5;
        [SerializeField] private int lifeRefillIntervalSeconds = 1800; // 30 minutes

        private int coins = 0;
        private int gems = 0;
        private int lives = 5;
        private float lifeRefillTimer = 0f;
        private Dictionary<BoosterType, int> boosters = new Dictionary<BoosterType, int>();

        public int Coins => coins;
        public int Gems => gems;
        public int Lives => lives;

        protected override void Awake()
        {
            base.Awake();
            InitializeBoosters();
        }

        private void Start()
        {
            // Load from PlayerPrefs or Firebase
            LoadInventory();
        }

        private void Update()
        {
            UpdateLifeRefill();
        }

        /// <summary>
        /// Initialize booster dictionary
        /// </summary>
        private void InitializeBoosters()
        {
            boosters.Clear();
            foreach (BoosterType type in System.Enum.GetValues(typeof(BoosterType)))
            {
                boosters[type] = 0;
            }
        }

        /// <summary>
        /// Add coins to inventory
        /// </summary>
        public void AddCoins(int amount)
        {
            coins += amount;
            GameEvents.InvokeCoinsChanged(coins);
            SaveInventory();
        }

        /// <summary>
        /// Spend coins
        /// </summary>
        public bool SpendCoins(int amount)
        {
            if (coins >= amount)
            {
                coins -= amount;
                GameEvents.InvokeCoinsChanged(coins);
                SaveInventory();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add gems to inventory
        /// </summary>
        public void AddGems(int amount)
        {
            gems += amount;
            GameEvents.InvokeGemsChanged(gems);
            SaveInventory();
        }

        /// <summary>
        /// Spend gems
        /// </summary>
        public bool SpendGems(int amount)
        {
            if (gems >= amount)
            {
                gems -= amount;
                GameEvents.InvokeGemsChanged(gems);
                SaveInventory();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add booster to inventory
        /// </summary>
        public void AddBooster(BoosterType type, int amount = 1)
        {
            if (boosters.ContainsKey(type))
            {
                boosters[type] += amount;
                GameEvents.InvokeBoosterChanged(type, boosters[type]);
                SaveInventory();
            }
        }

        /// <summary>
        /// Use booster
        /// </summary>
        public bool UseBooster(BoosterType type)
        {
            if (boosters.ContainsKey(type) && boosters[type] > 0)
            {
                boosters[type]--;
                GameEvents.InvokeBoosterChanged(type, boosters[type]);
                SaveInventory();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get booster count
        /// </summary>
        public int GetBoosterCount(BoosterType type)
        {
            if (boosters.ContainsKey(type))
                return boosters[type];
            return 0;
        }

        /// <summary>
        /// Consume one life
        /// </summary>
        public bool ConsumeLife()
        {
            if (lives > 0)
            {
                lives--;
                GameEvents.InvokeLivesChanged(lives);
                SaveInventory();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add life
        /// </summary>
        public void AddLife(int amount = 1)
        {
            lives = Mathf.Min(lives + amount, maxLives);
            lifeRefillTimer = 0f; // Reset timer when manually adding
            GameEvents.InvokeLivesChanged(lives);
            SaveInventory();
        }

        /// <summary>
        /// Update life refill timer
        /// </summary>
        private void UpdateLifeRefill()
        {
            if (lives < maxLives)
            {
                lifeRefillTimer += Time.deltaTime;

                if (lifeRefillTimer >= lifeRefillIntervalSeconds)
                {
                    lifeRefillTimer = 0f;
                    AddLife(1);
                }
            }
        }

        /// <summary>
        /// Save inventory to local storage
        /// </summary>
        private void SaveInventory()
        {
            PlayerPrefs.SetInt("Inventory_Coins", coins);
            PlayerPrefs.SetInt("Inventory_Gems", gems);
            PlayerPrefs.SetInt("Inventory_Lives", lives);

            foreach (var kvp in boosters)
            {
                PlayerPrefs.SetInt($"Booster_{kvp.Key}", kvp.Value);
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load inventory from local storage
        /// </summary>
        private void LoadInventory()
        {
            coins = PlayerPrefs.GetInt("Inventory_Coins", 500);
            gems = PlayerPrefs.GetInt("Inventory_Gems", 0);
            lives = PlayerPrefs.GetInt("Inventory_Lives", maxLives);

            foreach (BoosterType type in System.Enum.GetValues(typeof(BoosterType)))
            {
                boosters[type] = PlayerPrefs.GetInt($"Booster_{type}", 0);
            }

            GameEvents.InvokeCoinsChanged(coins);
            GameEvents.InvokeGemsChanged(gems);
            GameEvents.InvokeLivesChanged(lives);
        }
    }
}
