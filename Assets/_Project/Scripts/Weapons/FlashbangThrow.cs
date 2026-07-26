using UnityEngine;
namespace Weapons {
    public class FlashbangThrow : WeaponBase {
        [SerializeField] Flashbang prefab;
        [SerializeField] int ammo = 1;
        protected override void Fire() {
            if (ammo > 0) {
                ammo -= 1;
                var rb = Instantiate(prefab, transform.position, transform.rotation);
                rb.Owner = transform;
                rb.gameObject.SetActive(true);
            }
        }
    }
}
