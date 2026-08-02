using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Weapons;
namespace PlayerController
{
    public class PlayerWeaponController : MonoBehaviour
    {
        [SerializeField] Rigidbody PlayerBody;
        [SerializeField] protected InputReader inputReader;
        [SerializeField] protected CameraController cameraController;
        [SerializeField] protected List<WeaponBase> weapons = new();
        protected int selectedWeaponIndex = 0;

        private void OnEnable()
        {
            inputReader.Fire += OnFire;
            inputReader.AltFire += OnAltFire;
            inputReader.SwitchWeapon += OnSwitchWeapon;
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].SetModelVisible(false);
            }
            OnSwitchWeapon(0);
        }
        private void OnDisable()
        {
            inputReader.Fire -= OnFire;
            inputReader.AltFire -= OnAltFire;
            inputReader.SwitchWeapon -= OnSwitchWeapon;
        }
        /// <summary>
        /// Takes in an input context. Triggers the selected weapon's primary fire.
        /// </summary>
        /// <param name="context">The input context that the selectedWeaponIndex must receive.</param>
        protected void OnFire(InputAction.CallbackContext context)
        {
            if (selectedWeaponIndex < 0 || selectedWeaponIndex >= weapons.Count)
            {
                Debug.LogError($"{transform} attempted to use nonexistant selectedWeaponIndex {selectedWeaponIndex}.");
                return;
            }
            if (context.ReadValue<float>() == 1)
            {
                weapons[selectedWeaponIndex].Firing = true;
            }
            else
            {
                weapons[selectedWeaponIndex].Firing = false;
            }
        }

        /// <summary>
        /// Takes in an input context. Triggers the selected weapon's alternative fire.
        /// </summary>
        /// <param name="context">The input context that the selectedWeaponIndex must receive.</param>
        protected void OnAltFire(InputAction.CallbackContext context)
        {
            if (selectedWeaponIndex < 0 || selectedWeaponIndex >= weapons.Count)
            {
                Debug.LogError($"{transform} attempted to use nonexistant selectedWeaponIndex {selectedWeaponIndex}.");
                return;
            }
            if (context.ReadValue<float>() == 1)
            {
                weapons[selectedWeaponIndex].AltFiring = true;
            }
            else
            {
                weapons[selectedWeaponIndex].AltFiring = false;
            }
        }
        /// <summary>
        /// Used for weapon abilities that affect player's velocity such as lunges.
        /// </summary>
        /// <param name="velocity"> The foward force we add to the player. </param>
        protected void OnBoostPlayer(float velocity)
        {
            PlayerBody.AddForce(transform.forward * velocity);
        }

        protected void OnTeleportPlayer(Vector3 position, Quaternion? rotation = null)
        {
            PlayerBody.position = position;
            if(rotation != null)
                cameraController.SetCameraRotation((Quaternion)rotation);
            PlayerBody.linearVelocity = Vector3.zero;
        }

        /// <summary>
        /// Used to switch between weapons.
        /// Hides the previously selected weapon and shows the new one.
        /// </summary>
        /// <param name="weaponNumber"> The index of the weapon we want to switch to.</param>
        protected void OnSwitchWeapon(int weaponNumber)
        {
            weapons[selectedWeaponIndex].Firing = false;
            weapons[selectedWeaponIndex].AltFiring = false;
            clearWeaponACtions(selectedWeaponIndex);
            bindWeaponActions(weaponNumber);
            weapons[selectedWeaponIndex].SetModelVisible(false);
            weapons[weaponNumber].SetModelVisible(true);
            selectedWeaponIndex = weaponNumber;
        }

        void bindWeaponActions(int weaponNumber)
        {
            weapons[weaponNumber].BoostPlayer += OnBoostPlayer;
            weapons[weaponNumber].TeleportPlayer += OnTeleportPlayer;
        }
        void clearWeaponACtions(int weaponNumber)
        {
            weapons[weaponNumber].BoostPlayer -= OnBoostPlayer;
            weapons[weaponNumber].TeleportPlayer -= OnTeleportPlayer;

        }
    }
}