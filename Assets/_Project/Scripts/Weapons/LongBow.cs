using UnityEngine;

namespace Weapons
{
    public class LongBow : Bow
    {
        protected override void UpdateAltFireInput()
        {
            if (Time.time < cooldownTo)
                return;

            currentCharge += chargeIncrement * Time.deltaTime;
            AltFire();
        }

        protected override void Fire()
        {
            animancer.Play(clip).Time = 0;
            cooldownTo = Time.time + fireCooldown;
            ShootArrow(currentCharge);
            ammo -= 1;
        }

        protected override void AltFire()
        {
            const float SphereRadius = 2f;
            const float SphereDistance = 0f;
            const int SphereDamage = 1;

            animancer.Play(clip).Time = 0;
            cooldownTo = Time.time + fireCooldown;
            CreateSphereAttack(SphereRadius, SphereDistance, SphereDamage);
        }
    }
}