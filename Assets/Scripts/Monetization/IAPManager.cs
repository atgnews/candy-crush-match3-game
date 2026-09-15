using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using CandyCrush.Core;
using CandyCrush.Events;
using System;

namespace CandyCrush.Monetization
{
    /// <summary>
    /// Manages in-app purchases for coins, lives, boosters, and subscriptions
    /// </summary>
    public class IAPManager : Singleton<IAPManager>, IStoreListener
    {
        private IStoreController storeController;
        private IExtensionProvider extensionProvider;

        // Coins Packs
        private const string SKU_COINS_99 = "coins_99_rupees";      // ₹99 = 400 coins
        private const string SKU_COINS_499 = "coins_499_rupees";    // ₹499 = 2500 coins (+25%)
        private const string SKU_COINS_999 = "coins_999_rupees";    // ₹999 = 6000 coins (+50%)

        // Lives Packs
        private const string SKU_LIVES_5 = "lives_5_pack";          // ₹29 = 5 lives
        private const string SKU_LIVES_24H = "lives_24h_unlimited"; // ₹79 = 24h unlimited
        private const string SKU_LIVES_7D = "lives_7d_unlimited";   // ₹199 = 7 days unlimited

        // Booster Bundles
        private const string SKU_BOOSTER_STARTER = "booster_starter_bundle"; // ₹149
        private const string SKU_BOOSTER_POWER = "booster_power_bundle";     // ₹499

        // Subscription
        private const string SKU_SUBSCRIPTION_MONTHLY = "subscription_premium_monthly"; // ₹299/month

        public bool IsInitialized => storeController != null && extensionProvider != null;

        protected override void Awake()
        {
            base.Awake();
            InitializePurchasing();
        }

        /// <summary>
        /// Initialize Unity Purchasing
        /// </summary>
        private void InitializePurchasing()
        {
            if (IsInitialized)
                return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add all products
            builder.AddProduct(SKU_COINS_99, ProductType.Consumable);
            builder.AddProduct(SKU_COINS_499, ProductType.Consumable);
            builder.AddProduct(SKU_COINS_999, ProductType.Consumable);

            builder.AddProduct(SKU_LIVES_5, ProductType.Consumable);
            builder.AddProduct(SKU_LIVES_24H, ProductType.Consumable);
            builder.AddProduct(SKU_LIVES_7D, ProductType.Consumable);

            builder.AddProduct(SKU_BOOSTER_STARTER, ProductType.Consumable);
            builder.AddProduct(SKU_BOOSTER_POWER, ProductType.Consumable);

            builder.AddProduct(SKU_SUBSCRIPTION_MONTHLY, ProductType.Subscription);

            UnityPurchasing.Initialize(this, builder);
        }

        /// <summary>
        /// Called when purchasing is initialized successfully
        /// </summary>
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            extensionProvider = extensions;
            Debug.Log("Unity Purchasing initialized successfully");
        }

        /// <summary>
        /// Called when purchasing fails to initialize
        /// </summary>
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"Unity Purchasing initialization failed: {error}");
        }

        /// <summary>
        /// Process the purchase
        /// </summary>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var product = purchaseEvent.purchasedProduct;

            Debug.Log($"Purchase successful: {product.definition.id}");

            switch (product.definition.id)
            {
                // Coins Packs
                case SKU_COINS_99:
                    ProcessCoinsPurchase(400);
                    break;
                case SKU_COINS_499:
                    ProcessCoinsPurchase(2500);
                    break;
                case SKU_COINS_999:
                    ProcessCoinsPurchase(6000);
                    break;

                // Lives Packs
                case SKU_LIVES_5:
                    ProcessLivesPurchase(5);
                    break;
                case SKU_LIVES_24H:
                    ProcessUnlimitedLives(24);
                    break;
                case SKU_LIVES_7D:
                    ProcessUnlimitedLives(168); // 7 days in hours
                    break;

                // Booster Bundles
                case SKU_BOOSTER_STARTER:
                    ProcessBoosterBundle(5);
                    break;
                case SKU_BOOSTER_POWER:
                    ProcessBoosterBundle(20);
                    break;

                // Subscription
                case SKU_SUBSCRIPTION_MONTHLY:
                    ProcessSubscription();
                    break;
            }

            GameEvents.InvokePurchaseSucceeded(product.definition.id);

            // Verify receipt if needed
            if (ValidateReceipt(product))
            {
                return PurchaseProcessingResult.Complete;
            }
            else
            {
                return PurchaseProcessingResult.Pending;
            }
        }

        /// <summary>
        /// Handle purchase failure
        /// </summary>
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"Purchase failed for {product.definition.id}: {failureReason}");
            GameEvents.InvokePurchaseFailed(product.definition.id, failureReason.ToString());
        }

        /// <summary>
        /// Buy coins pack
        /// </summary>
        public void BuyCoinsPack(string sku)
        {
            if (IsInitialized)
            {
                storeController.InitiatePurchase(sku);
            }
        }

        /// <summary>
        /// Buy lives pack
        /// </summary>
        public void BuyLivesPack(string sku)
        {
            if (IsInitialized)
            {
                storeController.InitiatePurchase(sku);
            }
        }

        /// <summary>
        /// Buy booster bundle
        /// </summary>
        public void BuyBoosterBundle(string sku)
        {
            if (IsInitialized)
            {
                storeController.InitiatePurchase(sku);
            }
        }

        /// <summary>
        /// Subscribe to premium
        /// </summary>
        public void SubscribeToPremium()
        {
            if (IsInitialized)
            {
                storeController.InitiatePurchase(SKU_SUBSCRIPTION_MONTHLY);
            }
        }

        /// <summary>
        /// Process coins purchase
        /// </summary>
        private void ProcessCoinsPurchase(int coinsAmount)
        {
            var inventoryManager = InventoryManager.Instance;
            inventoryManager.AddCoins(coinsAmount);
            Debug.Log($"Added {coinsAmount} coins");
        }

        /// <summary>
        /// Process lives purchase
        /// </summary>
        private void ProcessLivesPurchase(int livesAmount)
        {
            var inventoryManager = InventoryManager.Instance;
            inventoryManager.AddLife(livesAmount);
            Debug.Log($"Added {livesAmount} lives");
        }

        /// <summary>
        /// Process unlimited lives
        /// </summary>
        private void ProcessUnlimitedLives(int hours)
        {
            // TODO: Store unlimited lives expiration time
            Debug.Log($"Unlimited lives for {hours} hours activated");
        }

        /// <summary>
        /// Process booster bundle
        /// </summary>
        private void ProcessBoosterBundle(int boosterCount)
        {
            var inventoryManager = InventoryManager.Instance;
            foreach (BoosterType type in System.Enum.GetValues(typeof(BoosterType)))
            {
                inventoryManager.AddBooster(type, boosterCount);
            }
            Debug.Log($"Added {boosterCount} of each booster");
        }

        /// <summary>
        /// Process subscription
        /// </summary>
        private void ProcessSubscription()
        {
            // TODO: Set subscription flags and expiration
            // - Unlimited lives
            // - 10 free daily boosters
            // - Ad-free
            // - Exclusive skins
            GameEvents.InvokeSubscriptionActivated();
            Debug.Log("Premium subscription activated");
        }

        /// <summary>
        /// Validate purchase receipt
        /// </summary>
        private bool ValidateReceipt(Product product)
        {
            // TODO: Implement server-side receipt validation
            // For now, trust the client
            return true;
        }
    }
}
