using System.Collections;
using UnityEngine;

namespace Weapons
{
    public class RapidFireBow : Bow
    {
        protected override int AltFireAmmoCost => 1;
        [SerializeField] int ArrowsPerReload = 5;
        int arrowsFiredWithoutReloading = 0;

        protected override void UpdateFireInput()
        {
            if (ammo < FireAmmoCost || Time.time < cooldownTo)
                return;

            cooldownTo = Time.time + fireCooldown;
            Fire();
        }

        protected override void Fire()
        {
            IsEnhanced = true;
            animancer.Play(clip).Time = 0;
            ShootArrow(MaxCharge);
            if(!IsEnhanced)
                ammo -= 1;
            else
            {
                if (arrowsFiredWithoutReloading == ArrowsPerReload)
                {
                    arrowsFiredWithoutReloading = 0;
                    ammo -= 1;
                }
                else arrowsFiredWithoutReloading++;
            }
        }

        protected override void AltFire()
        {
            cooldownTo = Time.time + fireCooldown;
            StartCoroutine(RepeatedAltFireAction(currentCharge));
        }

        private IEnumerator RepeatedAltFireAction(float charge)
        {
            const float TimeBetweenArrows = 0.1f;
            const int ArrowCount = 5;

            animancer.Play(clip).Time = 0;
            cooldownTo = Time.time + altFireCooldown;

            for (int i = 0; i < ArrowCount; i++)
            {
                ShootArrow(charge);
                yield return new WaitForSeconds(TimeBetweenArrows);
                ammo -= 1;
                if (ammo <= 0) break;
            }
        }
    }
}