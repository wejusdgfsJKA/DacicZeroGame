using HP;
using PlayerController;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

public class WeaponSpriteHandler : MonoBehaviour
{
    [SerializeField] PlayerWeaponController WeaponController;
    [SerializeField] RawImage Image;


    private void Awake()
    {

    }
    private void OnEnable()
    {
        WeaponController.SwitchedActiveWeapon += updateActiveWeaponSprite;
    }

    private void OnDisable()
    {
        WeaponController.SwitchedActiveWeapon -= updateActiveWeaponSprite;
    }

    public void updateActiveWeaponSprite(WeaponBase weapon)
    {
        var sprite = weapon.WeaponSprite;
        if (sprite == null) Image.color = new Color(0, 0, 0, 0); // invis when theres no sprite, tho this shouldnt be the case in the final version.
        else Image.color = Color.white; // fully visible otherwise

        Image.texture = weapon.WeaponSprite;
    }
}
