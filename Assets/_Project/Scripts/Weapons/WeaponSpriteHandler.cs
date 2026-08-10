using HP;
using PlayerController;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

public class WeaponSpriteHandler : MonoBehaviour
{
    [SerializeField] PlayerWeaponController WeaponController;
    [SerializeField] RawImage Image;
    [SerializeField] Slider ChargeSlider;
    [SerializeField] Image ChargeSliderFill;


    private void Awake()
    {

    }
    private void OnEnable()
    {
        WeaponController.SwitchedActiveWeapon += updateActiveWeaponSprite;
        WeaponController.UpdateWeaponCharge += updateChargeSlider;
        ChargeSlider.maxValue = 100f;
    }

    private void OnDisable()
    {
        WeaponController.SwitchedActiveWeapon -= updateActiveWeaponSprite;
        WeaponController.UpdateWeaponCharge -= updateChargeSlider;
    }

    public void updateActiveWeaponSprite(WeaponBase weapon)
    {
        var sprite = weapon.WeaponSprite;
        if (sprite == null) Image.color = new Color(0, 0, 0, 0); // invis when theres no sprite, tho this shouldnt be the case in the final version.
        else Image.color = Color.white; // fully visible otherwise

        Image.texture = weapon.WeaponSprite;
    }

    public void updateChargeSlider(float charge)
    {
        if(charge >= 100) ChargeSliderFill.color = Color.red;
        else ChargeSliderFill.color = Color.white;
        ChargeSliderFill.color = new Color(ChargeSliderFill.color.r, // you would think i can just set the opacity
                                            ChargeSliderFill.color.g,
                                            ChargeSliderFill.color.b,
                                            0.5f
                                            );
        ChargeSlider.value = charge;
    }
}
