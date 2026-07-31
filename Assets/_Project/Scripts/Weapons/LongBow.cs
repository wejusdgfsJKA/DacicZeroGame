using EventBus;
using HP;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            if (!IsEnhanced)
            {
                ShootArrow(currentCharge);
                ammo -= 1;
            }
            else
            {
                const float ArrowSpread = 3f;
                ShootArrow(currentCharge);
                ShootArrow(currentCharge, ArrowSpread);
                ShootArrow(currentCharge, (-1) * ArrowSpread);
                ammo -= 1;
            }
        }

        protected override void AltFire()
        {
            StartCoroutine(AltFireAction());
            cooldownTo = Time.time + fireCooldown;
        }

        IEnumerator AltFireAction()
        {
            const float SphereRadius = 2f;
            const float SphereDistance = 0f;
            const float TpSlashDelay = 0.1f;
            if (IsEnhanced) 
            {
                var pos = getNextToEnemyPosition();
                var rot = getEnemyRotation();
                if (pos != null)
                {
                    TeleportPlayer.Invoke((Vector3)pos, rot);
                    yield return new WaitForSeconds(TpSlashDelay);
                }
            }

            animancer.Play(clip).Time = 0;
            CreateSphereAttack(SphereRadius, SphereDistance, Damage);
        }

        Transform getClosestEnemyTransform()
        {
            const float SCAN_RADIUS = 20f;
            var hits = new HashSet<Transform>();
            Collider[] colliders = new Collider[10];
            int nrOfHits = Physics.OverlapSphereNonAlloc(transform.position, SCAN_RADIUS, colliders, LayerMask.GetMask("Bots"));
            if (colliders[0] != null)
            {
                var closestEnemyTransform = colliders[0].transform.root;
                foreach (Collider collider in colliders) 
                {
                    var transform = colliders[0].transform.root;
                    if((transform.position - gameObject.transform.position).magnitude < (closestEnemyTransform.position - gameObject.transform.position).magnitude)
                        closestEnemyTransform = transform;
                }
                return closestEnemyTransform;
            }
            return null;
            
        }

        Vector3? getNextToEnemyPosition()
        {
            var closestEnemyTransform = getClosestEnemyTransform();
            if (closestEnemyTransform == null) return null;

            var pos = closestEnemyTransform.position + closestEnemyTransform.forward * (-2);
            return pos;
        }

        Quaternion? getEnemyRotation()
        {
            var closestEnemyTransform = getClosestEnemyTransform();
            if (closestEnemyTransform == null) return null;

            var rot = closestEnemyTransform.rotation;
            return rot;
        }



    }
}