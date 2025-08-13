using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] protected InputReader inputReader;
    private void OnEnable()
    {
        ConnectEvents();
    }
    void ConnectEvents()
    {
        inputReader.Weapon += OnWeapon;
    }
    void DisconnectEvents()
    {
        inputReader.Weapon -= OnWeapon;
    }
    private void OnDisable()
    {
        DisconnectEvents();
    }
    /// <summary>
    /// Takes in an input context for a certain weapon. Displays an error log if the weapon is not found.
    /// </summary>
    /// <param name="context">The input context that the weapon must receive.</param>
    /// <param name="weapon">The weapon that must receive the input.</param>
    protected void OnWeapon(InputAction.CallbackContext context, int weapon)
    {
        Debug.Log($"Context value of {context.ReadValue<float>()} for weapon {weapon}");
        //if (weapon < 0 || weapon >= weapons.Count)
        //{
        //    Debug.LogError($"{transform} attempted to use nonexistant weapon {weapon}.");
        //    return;
        //}
        //if (context.ReadValue<float>() == 1)
        //{
        //    weapons[weapon].Firing = true;
        //}
        //else
        //{
        //    weapons[weapon].Firing = false;
        //}
    }
}
