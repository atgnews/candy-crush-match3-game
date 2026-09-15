using UnityEngine;
using UnityEngine.UI;
using CandyCrush.Gameplay;
using CandyCrush.Monetization;

namespace CandyCrush.UI
{
    /// <summary>
    /// Shop screen for purchasing coins, lives, and boosters
    /// </summary>
    public class ShopScreen : MonoBehaviour
    {
        [SerializeField] private Button coinsPackButton99;
        [SerializeField] private Button coinsPackButton499;
        [SerializeField] private Button coinsPackButton999;
        [SerializeField] private Button livesPackButton;
        [SerializeField] private Button boosterBundleButton;
        [SerializeField] private Button premiumSubscriptionButton;
        [SerializeField] private Text coinsBalanceText;
        [SerializeField] private Text gemsBalanceText;

        private IAPManager iapManager;

        private void Start()
        {
            iapManager = IAPManager.Instance;

            coinsPackButton99.onClick.AddListener(() => iapManager.BuyCoinsPack("coins_99_rupees"));
            coinsPackButton499.onClick.AddListener(() => iapManager.BuyCoinsPack("coins_499_rupees"));
            coinsPackButton999.onClick.AddListener(() => iapManager.BuyCoinsPack("coins_999_rupees"));
            livesPackButton.onClick.AddListener(() => iapManager.BuyLivesPack("lives_5_pack"));
            boosterBundleButton.onClick.AddListener(() => iapManager.BuyBoosterBundle("booster_starter_bundle"));
            premiumSubscriptionButton.onClick.AddListener(() => iapManager.SubscribeToPremium());

            UpdateBalanceDisplay();
        }

        private void OnEnable()
        {
            UpdateBalanceDisplay();
        }

        private void UpdateBalanceDisplay()
        {
            var inventory = InventoryManager.Instance;
            coinsBalanceText.text = $"Coins: {inventory.Coins}";
            gemsBalanceText.text = $"Gems: {inventory.Gems}";
        }
    }
}
