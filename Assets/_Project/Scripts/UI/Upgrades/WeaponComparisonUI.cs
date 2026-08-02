using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DacicZero.Data.Weapons;
using DacicZero.Global;
using System.Linq;

namespace DacicZero.UI.Upgrades {
    public class WeaponComparisonUI : MonoBehaviour {
        
        #region Variables & Properties
        [Header("Containers")]
        [SerializeField] private GameObject _comparisonContainer;

        [Header("Current Weapon (Left Panel)")]
        [SerializeField] private TextMeshProUGUI _currentNameText;
        [SerializeField] private TextMeshProUGUI _currentLevelText;
        [SerializeField] private Image _currentIcon;
        [SerializeField] private TextMeshProUGUI _currentStatsText;

        [Header("Upgraded Weapon (Right Panel)")]
        [SerializeField] private TextMeshProUGUI _upgradeNameText;
        [SerializeField] private TextMeshProUGUI _upgradeLevelText;
        [SerializeField] private Image _upgradeIcon;
        [SerializeField] private TextMeshProUGUI _upgradeStatsText;

        [Header("Purchase UI")]
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Button _purchaseButton;

        private WeaponUpgradeSO _activeUpgrade;
        private UpgradeMenuUI _menuController;
        #endregion

        #region Unity Lifecycle
        private void Awake() {
#if UNITY_EDITOR
            if (_comparisonContainer == null) Debug.LogError($"[WeaponComparisonUI] Comparison Container missing on {gameObject.name}!");

            if (_currentNameText == null) Debug.LogError($"[WeaponComparisonUI] Current Name Text missing on {gameObject.name}!");
            if (_currentLevelText == null) Debug.LogError($"[WeaponComparisonUI] Current Level Text missing on {gameObject.name}!");
            if (_currentIcon == null) Debug.LogError($"[WeaponComparisonUI] Current Icon missing on {gameObject.name}!");
            if (_currentStatsText == null) Debug.LogError($"[WeaponComparisonUI] Current Stats Text missing on {gameObject.name}!");

            if (_upgradeNameText == null) Debug.LogError($"[WeaponComparisonUI] Upgrade Name Text missing on {gameObject.name}!");
            if (_upgradeLevelText == null) Debug.LogError($"[WeaponComparisonUI] Upgrade Level Text missing on {gameObject.name}!");
            if (_upgradeIcon == null) Debug.LogError($"[WeaponComparisonUI] Upgrade Icon missing on {gameObject.name}!");
            if (_upgradeStatsText == null) Debug.LogError($"[WeaponComparisonUI] Upgrade Stats Text missing on {gameObject.name}!");

            if (_priceText == null) Debug.LogError($"[WeaponComparisonUI] Price Text missing on {gameObject.name}!");
            if (_purchaseButton == null) Debug.LogError($"[WeaponComparisonUI] Purchase Button missing on {gameObject.name}!");
#endif
        }

        private void OnEnable() { if (_purchaseButton != null) _purchaseButton.onClick.AddListener(OnPurchaseClicked); }
        private void OnDisable() { if (_purchaseButton != null) _purchaseButton.onClick.RemoveListener(OnPurchaseClicked); }
        
        #endregion
        #region UI Logic

        public void Initialize(UpgradeMenuUI menuController) { _menuController = menuController; }

        public void ClearComparison() {
            _activeUpgrade = null;
            if (_comparisonContainer != null) _comparisonContainer.SetActive(false);
        }

        public void DisplayUpgrade(WeaponUpgradeSO upgradeData) {
            Debug.Log($"[WeaponComparisonUI] DisplayUpgrade called with: {(upgradeData != null ? upgradeData.name : "null")}");
            if (upgradeData == null) return;
            
            _activeUpgrade = upgradeData;

            if (_comparisonContainer != null) _comparisonContainer.SetActive(true);

            bool isPurchased = PlayerLoadout.OwnedWeapons.Contains(upgradeData.UpgradedWeapon);
            WeaponDataSO leftWeapon = isPurchased ? upgradeData.UpgradedWeapon : upgradeData.CurrentWeapon;
            
            Debug.Log($"[WeaponComparisonUI] leftWeapon is {(leftWeapon != null ? leftWeapon.name : "null")}");

            if (leftWeapon != null) {
                if (_currentNameText != null) _currentNameText.text = leftWeapon.WeaponId;
                if (_currentLevelText != null) _currentLevelText.text = "Lvl ??"; 
                if (_currentIcon != null) _currentIcon.sprite = leftWeapon.WeaponIcon;
                if (_currentStatsText != null)
                    _currentStatsText.text = $"DMG: {leftWeapon.BaseDamage}\nSPD: {leftWeapon.FireRate}";
            }

            if (isPurchased) { 
                if (upgradeData.UpgradedWeapon != null) {
                    if (_upgradeNameText != null) _upgradeNameText.text = "MAXED";
                    if (_upgradeLevelText != null) _upgradeLevelText.text = "";
                    if (_upgradeIcon != null) _upgradeIcon.sprite = upgradeData.UpgradedWeapon.WeaponIcon;
                    if (_upgradeStatsText != null) _upgradeStatsText.text = "";
                }
            } else { 
                if (upgradeData.UpgradedWeapon != null) {
                    if (_upgradeNameText != null) _upgradeNameText.text = upgradeData.UpgradedWeapon.WeaponId;
                    if (_upgradeLevelText != null) _upgradeLevelText.text = "Lvl ??"; 
                    if (_upgradeIcon != null) _upgradeIcon.sprite = upgradeData.UpgradedWeapon.WeaponIcon;
                    if (_upgradeStatsText != null)
                        _upgradeStatsText.text = $"DMG: {upgradeData.UpgradedWeapon.BaseDamage}\nSPD: {upgradeData.UpgradedWeapon.FireRate}";
                }
            }

            RefreshPurchaseButtonState(PlayerResources.Scrap);
        }

        public void RefreshPurchaseButtonState(int currentScrap) {
            if (_activeUpgrade == null) return;

            bool isPurchased = PlayerLoadout.OwnedWeapons.Contains(_activeUpgrade.UpgradedWeapon);

            if (isPurchased) {
                if (_priceText != null) _priceText.text = "MAXED";
                if (_purchaseButton != null) _purchaseButton.interactable = false;
            } else {
                if (_priceText != null) _priceText.text = $"( {Mathf.Min(currentScrap, _activeUpgrade.ScrapCost)} / {_activeUpgrade.ScrapCost} )";
                if (_purchaseButton != null) _purchaseButton.interactable = currentScrap >= _activeUpgrade.ScrapCost;
            }
        }

        public void OnPurchaseClicked() {
            if (_activeUpgrade == null) return;
            if (_purchaseButton != null) _purchaseButton.interactable = false;

            if (PlayerResources.SpendScrap(_activeUpgrade.ScrapCost)) {
                Debug.Log($"[Upgrades] Successfully purchased {_activeUpgrade.UpgradedWeapon.WeaponId} for {_activeUpgrade.ScrapCost} Scrap!");
                PlayerLoadout.UpgradeWeapon(_activeUpgrade.CurrentWeapon, _activeUpgrade.UpgradedWeapon);
                
                var next = _menuController != null ? _menuController.GetNextUpgrade(_activeUpgrade.UpgradedWeapon) : null;
                
                if (next != null) DisplayUpgrade(next);
                else DisplayUpgrade(_activeUpgrade);
            } else RefreshPurchaseButtonState(PlayerResources.Scrap);
        }
        #endregion
    }
}