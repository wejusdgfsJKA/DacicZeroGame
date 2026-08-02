using UnityEngine;

namespace DacicZero.Data.Weapons {
    [CreateAssetMenu(fileName = "NewWeaponUpgrade", menuName = "DacicZero/Weapon Upgrade", order = 3)]
    public class WeaponUpgradeSO : ScriptableObject {
        
        [Tooltip("weapon required to purchase this upgrade.")]
        [SerializeField] private WeaponDataSO _currentWeapon;

        [Tooltip("weapon received after purchasing this upgrade.")]
        [SerializeField] private WeaponDataSO _upgradedWeapon;

        [Tooltip("scrap material cost required for upgrade.")]
        [Min(0)] // constrait for unity inspector
        [SerializeField] private int _scrapCost;

        public WeaponDataSO CurrentWeapon => _currentWeapon;
        public WeaponDataSO UpgradedWeapon => _upgradedWeapon;
        public int ScrapCost => _scrapCost;
    }
}