using EventBus;
using HP;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected int Damage = 2;
        [SerializeField] protected float fireCooldown;
        [SerializeField] protected float altFireCooldown;
        [field: SerializeField] public bool Firing { get; set; }
        [field: SerializeField] public bool AltFiring { get; set; }
        [field: SerializeField] public bool IsEnhanced { get; set; }

        protected float cooldownTo = -1;
        protected float timeLastShot = -1;
        public UnityAction<float> BoostPlayer = delegate { };

        protected Renderer[] modelRenderers;

        protected Renderer[] ModelRenderers
        {
            get
            {
                if (modelRenderers == null)
                    modelRenderers = GetComponentsInChildren<Renderer>();
                return modelRenderers;
            }
        }

        protected virtual void Update()
        {
            if (Firing)
            {
                if (Time.time >= cooldownTo)
                {
                    cooldownTo = Time.time + fireCooldown;
                    Fire();
                }
            }
            else if (AltFiring)
            {
                if (Time.time >= cooldownTo)
                {
                    cooldownTo = Time.time + altFireCooldown;
                    AltFire();
                }
            }
            else
            {
                HandleNotFiring();
            }
        }

        protected virtual void OnEnable()
        {
            Firing = false;
        }

        protected virtual void HandleNotFiring() { }
        /// <summary>
        /// Primary fire of the weapon (left click)
        /// </summary>
        protected abstract void Fire();
        
        /// <summary>
        /// Alternative fire of the weapon (right click)
        /// </summary>
        protected virtual void AltFire() { }

        /// <summary>
        /// Used to hide the weapon models when not needed.
        /// </summary>
        /// <param name="visible"> Visible or not.</param>
        public virtual void SetModelVisible(bool visible)
        {
            foreach (var renderer in ModelRenderers)
            {
                if (renderer != null)
                    renderer.enabled = visible;
            }
        }
#nullable enable
        /// <summary>
        /// Standardasied way of creating a damage field/attack.
        /// </summary>
        /// <param name="radius">Radius of the sphere</param>
        /// <param name="dist">Distance at which the sphere should spawn in front of the player</param>
        /// <param name="damage">Damage dealt</param>
        /// <param name="hits">Used for multi-phase attacks, keeps track of already hit reference, leave null if its a 1 phase attack.</param>
        /// <param name="statusEffect">Optional nullable status effect to inflict onto victims the attack's victims</param>
        public void CreateSphereAttack(float radius, float dist, int damage,HashSet<Transform>? hits = null, StatusEffect? statusEffect = null)
        {
            hits ??= new HashSet<Transform>();
            Collider[] colliders = new Collider[10];
            int nrOfHits = Physics.OverlapSphereNonAlloc(transform.position + dist * transform.forward, radius, colliders, LayerMask.GetMask("Bots"));
            for (int i = 0; i < nrOfHits; i++)
            {
                if (!hits.Contains(colliders[i].transform.root))
                {
                    hits.Add(colliders[i].transform.root);
                    
                    EventBus<TakeDamage>.Raise(colliders[i].transform.root.GetInstanceID(), new TakeDamage(damage, transform.root, colliders[i]));

                    if (statusEffect != null)
                    {
                        EventBus<ApplyStatusEffect>.Raise(colliders[i].transform.root.GetInstanceID(), new ApplyStatusEffect(statusEffect, transform.root, colliders[i]));
                    }
                }
            }
        }
#nullable disable
        
    }
}