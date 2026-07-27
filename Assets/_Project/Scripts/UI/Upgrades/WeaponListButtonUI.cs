using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DacicZero.Data.Weapons;

namespace DacicZero.UI.Upgrades {
    /// <summary> visual button representing an available weapon upgrade in the list. </summary>
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

            if (_upgradeData != null && _upgradeData.CurrentWeapon != null && _weaponNameText != null)
                _weaponNameText.text = _upgradeData.CurrentWeapon.WeaponId;
        }

        #region Unity Lifecycle
        private void Awake() {
#if UNITY_EDITOR
            if (_weaponNameText == null) Debug.LogError($"[WeaponListButtonUI] Weapon Name Text missing on {gameObject.name}!");
            if (_button == null) Debug.LogError($"[WeaponListButtonUI] Button missing on {gameObject.name}!");
#endif
        }

        private void OnEnable() { if (_button != null) _button.onClick.AddListener(OnButtonClicked); }

        private void OnDisable() { if (_button != null) _button.onClick.RemoveListener(OnButtonClicked); }
        #endregion

        #region Logic
        private void OnButtonClicked() {
            if (_upgradeData == null) return;
            if (_menuController != null) _menuController.SelectWeaponUpgrade(_upgradeData);
        }
        #endregion
    }
}
