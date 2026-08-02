using UnityEngine;

namespace DacicZero.Data.Weapons {
    public enum WeaponType { Primary, Secondary, Melee }

    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "DacicZero/Weapon Data", order = 2)]
    public class WeaponDataSO : ScriptableObject {
        [field: Header("Classification")]
        [field: Tooltip("weapon slot type.")]
        [field: SerializeField] public WeaponType Type { get; private set; }

        [field: Header("Identity")]
        [field: Tooltip("unique identifier for the weapon.")]
        [field: SerializeField] public string WeaponId { get; private set; }

        [field: Tooltip("current upgrade level of the weapon.")]
        [field: Min(1)]
        [field: SerializeField] public int WeaponLevel { get; private set; } = 1;

        [field: Tooltip("display name in the inventory.")]
        [field: SerializeField] public string WeaponName { get; private set; }

        [field: Tooltip("description of the weapon's mechanics.")]
        [field: TextArea(3, 5)]
        [field: SerializeField] public string WeaponDescription { get; private set; }

        [field: Header("Combat Stats")]
        [field: Tooltip("base damage dealt per hit.")]
        [field: SerializeField] public float BaseDamage { get; private set; }

        [field: Tooltip("time in seconds between attacks.")]
        [field: SerializeField] public float FireRate { get; private set; }

        [field: Header("Visuals & Prefabs")]
        [field: Tooltip("icon displayed in the UI.")]
        [field: SerializeField] public Sprite WeaponIcon { get; private set; }

        [field: Tooltip("the actual 3D/2D model spawned in the game.")]
        [field: SerializeField] public GameObject WeaponPrefab { get; private set; }
    }
}
