using EventBus;
using HP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Weapons
{
    public class BotWeapon : WeaponBase
    {
        [SerializeField] public float Range = 4;
        [SerializeField] Transform EnemyTransform;
        [SerializeField] NavMeshAgent EnemyAgent;
        [Tooltip("Cosmetic stuff")]
        [SerializeField] protected AudioClip audioClip;

        float maxRaycastDist;
        protected AudioSource audioSource;
        protected LineRenderer lineRenderer;
        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            audioSource = GetComponent<AudioSource>();
            maxRaycastDist = GlobalSettings.MaxRaycastDist;
        }
        protected override void Update()
        {
            base.Update();
        }
        protected override void Fire()
        {
            StartCoroutine(MultiStepAttack());
        }

        private IEnumerator MultiStepAttack()
        {
            var oldSpeed = EnemyAgent.speed;
            EnemyAgent.speed = 0;
            for(int i = 0; i < 10; i++)
            {
                EnemyTransform.position -= 0.1f * transform.forward;
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.2f);
            CreateBotSphereAttack(Range / 2, Range / 2, Damage);
            EnemyTransform.position += 3 * transform.forward;
            yield return new WaitForSeconds(0.01f);
            EnemyAgent.speed = oldSpeed;
            Firing = false;
        }
        public void CreateBotSphereAttack(float radius, float dist, int damage, HashSet<Transform>? hits = null, StatusEffect? statusEffect = null)
            {
                hits ??= new HashSet<Transform>();
                Collider[] colliders = new Collider[10];
                int nrOfHits = Physics.OverlapSphereNonAlloc(transform.position + dist * transform.forward, radius, colliders, LayerMask.GetMask("Player"));
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
        }
    }

