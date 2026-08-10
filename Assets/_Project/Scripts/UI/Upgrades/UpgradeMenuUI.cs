using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DacicZero.Global;
using DacicZero.Data.Weapons;

namespace DacicZero.UI.Upgrades {
    public class UpgradeMenuUI : MonoBehaviour {
        #region Variables & Properties
        public bool IsOpen => gameObject.activeSelf;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _scrapText;
        [SerializeField] private WeaponComparisonUI _comparisonUI;

        [Header("Dynamic Lists")]
        [SerializeField] private WeaponListButtonUI _weaponButtonPrefab;
        [SerializeField] private Transform _weaponListContainer;
        [SerializeField] private WeaponUpgradeSO[] _allAvailableUpgrades;

        private readonly List<WeaponListButtonUI> _activeButtons = new();
        private readonly List<WeaponListButtonUI> _buttonPool = new();

        private readonly Dictionary<WeaponDataSO, WeaponUpgradeSO> _upgradeDictionary = new();
        
        #endregion
        #region Unity Lifecycle

        private void Awake() {
            if (_weaponButtonPrefab == null) Debug.LogError("Weapon Button Prefab missing on " + gameObject.name);
            if (_weaponListContainer == null) Debug.LogError("Weapon List Container missing on " + gameObject.name);
            if (_comparisonUI == null) Debug.LogWarning("Comparison UI is not assigned on " + gameObject.name);

            if (_allAvailableUpgrades != null) 
                foreach (var upgrade in _allAvailableUpgrades) 
                    if (upgrade != null && upgrade.CurrentWeapon != null) 
                        _upgradeDictionary[upgrade.CurrentWeapon] = upgrade;
        }

        private void Start() {if (_comparisonUI != null) _comparisonUI.Initialize(this); }
        
        private void OnEnable() {
            PlayerResources.OnScrapChanged += UpdateScrapUI;
            
            UpdateScrapUI(PlayerResources.Scrap);
            if (_comparisonUI != null) _comparisonUI.ClearComparison();
            
            PopulateUpgradeList();
        }
        private void OnDisable() { PlayerResources.OnScrapChanged -= UpdateScrapUI; }

        #endregion
        #region UI Logic

        private void PopulateUpgradeList() {
            if (_weaponButtonPrefab == null || _weaponListContainer == null) return;

            foreach (var btn in _activeButtons) {
                btn.gameObject.SetActive(false);
                _buttonPool.Add(btn);
            }
            _activeButtons.Clear();

            if (_allAvailableUpgrades != null) {
                foreach (var upgrade in _allAvailableUpgrades) {
                    if (upgrade == null) continue;

                    // uncomment to only show upgrades for weapons the player owns:
                    // if (!PlayerLoadout.OwnedWeapons.Contains(upgrade.CurrentWeapon)) continue;

                    WeaponListButtonUI btn;

                    if (_buttonPool.Count > 0) {
                        int lastIdx = _buttonPool.Count - 1;
                        btn = _buttonPool[lastIdx];
                        _buttonPool.RemoveAt(lastIdx);
                        btn.gameObject.SetActive(true);
                    } else btn = Instantiate(_weaponButtonPrefab, _weaponListContainer);

                    btn.Initialize(upgrade, this);
                    _activeButtons.Add(btn);
                }
            }
        }

        public WeaponUpgradeSO GetNextUpgrade(WeaponDataSO currentWeapon) {
            if (currentWeapon != null && _upgradeDictionary.TryGetValue(currentWeapon, out WeaponUpgradeSO nextUpgrade)) 
                return nextUpgrade;
            return null;
        }

        private void UpdateScrapUI(int scrapAmount) {
            if (_scrapText != null) _scrapText.text = $"Scrap: {scrapAmount}";
            if (_comparisonUI != null) _comparisonUI.RefreshPurchaseButtonState(scrapAmount);
        }

        public void SelectWeaponUpgrade(WeaponUpgradeSO upgradeData) { 
            if (_comparisonUI != null) _comparisonUI.DisplayUpgrade(upgradeData); 
        }
        #endregion
    }
}