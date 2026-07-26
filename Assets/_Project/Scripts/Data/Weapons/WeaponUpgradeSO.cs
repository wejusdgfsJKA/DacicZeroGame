using UnityEngine;

namespace DacicZero.Data.Weapons {
    /// <summary> scriptableobject defining weapon upgrade requirements and outcome. </summary>
    [CreateAssetMenu(fileName = "NewWeaponUpgrade", menuName = "DacicZero/Weapon Upgrade", order = 3)]
    public class WeaponUpgradeSO : ScriptableObject {
        [field: Tooltip("weapon required to purchase this upgrade.")]
        [field: SerializeField] public WeaponDataSO CurrentWeapon { get; private set; }

        [field: Tooltip("weapon received after purchasing this upgrade.")]
        [field: SerializeField] public WeaponDataSO UpgradedWeapon { get; private set; }

        [field: Tooltip("scrap material cost required for upgrade.")]
        [field: SerializeField] public int ScrapCost { get; private set; }
    }
}
