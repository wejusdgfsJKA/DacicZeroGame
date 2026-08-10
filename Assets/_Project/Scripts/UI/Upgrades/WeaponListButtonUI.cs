using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DacicZero.Data.Weapons;
using System.Linq;

namespace DacicZero.UI.Upgrades {
    public class WeaponListButtonUI : MonoBehaviour {
        
        #region Variables & Properties
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _weaponNameText;
        [SerializeField] private Button _button;

        private WeaponUpgradeSO _upgradeData;
        private UpgradeMenuUI _menuController;
        #endregion

        public void Initialize(WeaponUpgradeSO upgradeData, UpgradeMenuUI menuController) {
            _upgradeData = upgradeData;
            _menuController = menuController;
            RefreshUI();
        }

        private void RefreshUI() {
            if (_upgradeData != null && _weaponNameText != null) {
                int loopGuard = 0; 
                int maxLoops = 50; 

                while (Global.PlayerLoadout.OwnedWeapons.Contains(_upgradeData.UpgradedWeapon)) {
                    
                    loopGuard++;
                    if (loopGuard > maxLoops) {
                        Debug.LogError($"[WeaponListButtonUI] Infinite upgrade loop detected on {_upgradeData.name}! Check your ScriptableObjects for circular references.");
                        break;
                    }

                    var next = _menuController.GetNextUpgrade(_upgradeData.UpgradedWeapon);
                    if (next != null) _upgradeData = next;
                    else  break;
                }

                bool isPurchased = Global.PlayerLoadout.OwnedWeapons.Contains(_upgradeData.UpgradedWeapon);
                var weaponToShow = isPurchased ? _upgradeData.UpgradedWeapon : _upgradeData.CurrentWeapon;
                
                if (weaponToShow != null) _weaponNameText.text = weaponToShow.WeaponId;
            }
        }

        #region Unity Lifecycle
        private void Awake() {
#if UNITY_EDITOR
            if (_weaponNameText == null) Debug.LogError($"[WeaponListButtonUI] Weapon Name Text missing on {gameObject.name}!");
            if (_button == null) Debug.LogError($"[WeaponListButtonUI] Button missing on {gameObject.name}!");
#endif
        }

        private void OnEnable() {
            if (_button != null) _button.onClick.AddListener(OnButtonClicked);
            Global.PlayerLoadout.OnLoadoutChanged += RefreshUI;
        }

        private void OnDisable() {
            if (_button != null) _button.onClick.RemoveListener(OnButtonClicked);
            Global.PlayerLoadout.OnLoadoutChanged -= RefreshUI;
        }

        #endregion
        #region Logic

        private void OnButtonClicked() {
            if (_upgradeData == null) return;
            if (_menuController != null) _menuController.SelectWeaponUpgrade(_upgradeData);
        }
        #endregion
    }
}