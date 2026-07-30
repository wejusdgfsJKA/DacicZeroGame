using UnityEngine;
using TMPro;
using EventBus;
using DacicZero.Global;
using DacicZero.Data.Weapons;

namespace DacicZero.UI.Upgrades {
    /// <summary> main controller for the weapon upgrade screen. </summary>
    public class UpgradeMenuUI : MonoBehaviour, IClosable {
        #region Variables & Properties
        public bool IsOpen => _upgradeCanvas != null && _upgradeCanvas.activeSelf;

        [Header("UI References")]
        [SerializeField] private GameObject _upgradeCanvas;
        [SerializeField] private TextMeshProUGUI _scrapText;
        [SerializeField] private WeaponComparisonUI _comparisonUI;

        [Header("Dynamic Lists")]
        [SerializeField] private WeaponListButtonUI _weaponButtonPrefab;
        [SerializeField] private Transform _weaponListContainer;
        [SerializeField] private WeaponUpgradeSO[] _allAvailableUpgrades;
        #endregion

        #region Unity Lifecycle
        private void Awake() {
#if UNITY_EDITOR
            if (_upgradeCanvas == null) Debug.LogError($"[UpgradeMenuUI] Upgrade Canvas missing on {gameObject.name}!");
            if (_scrapText == null) Debug.LogError($"[UpgradeMenuUI] Scrap Text missing on {gameObject.name}!");
#endif
            if (_upgradeCanvas != null) _upgradeCanvas.SetActive(false);
        }

        private void OnEnable() {
            EventBus<Dialog.DialogActionFiredEvent>.AddActions(0, OnDialogAction);
            PlayerResources.OnScrapChanged += UpdateScrapUI;
        }

        private void OnDisable() {
            EventBus<Dialog.DialogActionFiredEvent>.RemoveActions(0, OnDialogAction);
            PlayerResources.OnScrapChanged -= UpdateScrapUI;
        }

        private void Start() => UpdateScrapUI(PlayerResources.Scrap);
        #endregion

        #region Core Logic
        private void OnDialogAction(Dialog.DialogActionFiredEvent evt) {
            if (string.Equals(evt.ActionID, "OpenUpgradeMenu", System.StringComparison.OrdinalIgnoreCase))
                OpenMenu();
        }

        public void OpenMenu() {
            if (_upgradeCanvas != null) _upgradeCanvas.SetActive(true);
            UpdateScrapUI(PlayerResources.Scrap);
            if (_comparisonUI != null) _comparisonUI.ClearComparison();
            PopulateUpgradeList();
            UIManager.RegisterMenu(this);
        }

        private void PopulateUpgradeList() {
            if (_weaponButtonPrefab == null || _weaponListContainer == null) return;

            foreach (Transform child in _weaponListContainer)
                Destroy(child.gameObject);

            if (_allAvailableUpgrades != null)
                foreach (var upgrade in _allAvailableUpgrades)
                    if (upgrade != null) {
                        var buttonInstance = Instantiate(_weaponButtonPrefab, _weaponListContainer);
                        buttonInstance.Initialize(upgrade, this);
                    }
        }

        public void Close() {
            if (_upgradeCanvas != null) _upgradeCanvas.SetActive(false);
            UIManager.UnregisterMenu(this);
        }

        private void UpdateScrapUI(int scrapAmount) {
            if (_scrapText != null) _scrapText.text = $"Scrap: {scrapAmount}";
            if (_comparisonUI != null) _comparisonUI.RefreshPurchaseButtonState(scrapAmount);
        }

        public void SelectWeaponUpgrade(WeaponUpgradeSO upgradeData) { if (_comparisonUI != null) _comparisonUI.DisplayUpgrade(upgradeData); }
        #endregion
    }
}
