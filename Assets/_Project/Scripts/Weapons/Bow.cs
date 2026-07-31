using Animancer;
using UnityEngine;

namespace Weapons
{
    public class Bow : WeaponBase
    {
        protected enum ChargeType { None, Fire, AltFire }

        [SerializeField] protected Projectile projectile;
        [SerializeField] protected float projectileVelocity;
        [SerializeField] protected AnimationClip clip;
        [SerializeField] protected int ammo = 10;

        protected AnimancerComponent animancer;

        protected const float MaxCharge = 100f;
        protected const float MinChargeToFire = 30f;

        [SerializeField] protected float chargeTime = 0.7f;
        protected float chargeIncrement = -1f;
        protected float currentCharge = 0f;
        protected ChargeType chargeType = ChargeType.None;

        protected virtual int FireAmmoCost => 1;
        protected virtual int AltFireAmmoCost => 3;

        protected void Awake()
        {
            animancer = GetComponent<AnimancerComponent>();
        }

        protected void Start()
        {
            chargeIncrement = MaxCharge / chargeTime;
        }

        protected override void Update()
        {
            if (Firing && !AltFiring)
                UpdateFireInput();
            else if (AltFiring && !Firing)
                UpdateAltFireInput();
            else if (Firing && AltFiring)
                CancelCharge();
            else
                ReleaseCharge();
        }

        protected virtual void UpdateFireInput()
        {
            if (ammo < FireAmmoCost || Time.time < cooldownTo)
                return;

            AccumulateCharge();
            chargeType = ChargeType.Fire;
        }

        protected virtual void UpdateAltFireInput()
        {
            if (ammo < AltFireAmmoCost || Time.time < cooldownTo)
                return;

            AccumulateCharge();
            chargeType = ChargeType.AltFire;
        }

        protected void AccumulateCharge()
        {
            currentCharge += chargeIncrement * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0, MaxCharge);
        }

        protected virtual void CancelCharge()
        {
            currentCharge = 0;
        }

        protected virtual void ReleaseCharge()
        {
            if (currentCharge > MinChargeToFire)
            {
                if (chargeType == ChargeType.Fire) Fire();
                else if (chargeType == ChargeType.AltFire) AltFire();
            }
            currentCharge = 0;
            HandleNotFiring();
        }

        protected override void Fire()
        {
            const int AmmoUsed = 1;

            animancer.Play(clip).Time = 0;
            cooldownTo = Time.time + fireCooldown;
            ShootArrow(currentCharge);
            ammo -= AmmoUsed;
        }

        protected override void AltFire()
        {
            const float SideArrowYawOffset = 7.5f;
            const int AmmoUsed = 3;

            animancer.Play(clip).Time = 0;
            cooldownTo = Time.time + altFireCooldown;

            ShootArrow(currentCharge);
            ShootArrow(currentCharge, SideArrowYawOffset);
            ShootArrow(currentCharge, -SideArrowYawOffset);

            ammo -= AmmoUsed;
        }

        /// <summary>
        /// Standardised way to shoot an arrow.
        /// </summary>
        /// <param name="charge">The current charge for the bow, used to determine the damage and velocity of the arrow.</param>
        /// <param name="yawOffset"> Amount of degrees the arrow should be offset by on the XoZ plane. Can be used for random spread or multi-arrow attacks.</param>
        protected void ShootArrow(float charge, float yawOffset = 0)
        {
            Projectile newProjectile = Instantiate(projectile, transform.position, transform.rotation);
            newProjectile.Owner = transform;
            newProjectile.velocity = projectileVelocity * charge / MaxCharge;
            newProjectile.damage = (int)(Damage * charge / MaxCharge);
            newProjectile.hasGravity = true;
            newProjectile.yawOffset = yawOffset;
            newProjectile.gameObject.SetActive(true);
        }
    }
}