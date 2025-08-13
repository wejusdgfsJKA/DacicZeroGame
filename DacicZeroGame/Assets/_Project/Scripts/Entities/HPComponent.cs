using EventBus;
using UnityEngine;

namespace Entity
{
    public class HPComponent : MonoBehaviour
    {
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        protected void Awake()
        {
            EventBus<TakeDamage>.AddActions(transform.GetInstanceID(), TakeDamage);
        }
        protected void OnEnable()
        {
            CurrentHealth = MaxHealth;
        }
        public void TakeDamage(TakeDamage dmg)
        {
            CurrentHealth -= dmg.Damage;
            if (CurrentHealth < 0)
            {
                Die();
            }
        }
        protected void Die()
        {
            gameObject.SetActive(false);
        }
        protected void OnDestroy()
        {
            EventBus<TakeDamage>.RemoveActions(transform.GetInstanceID(), TakeDamage);
        }
    }
}