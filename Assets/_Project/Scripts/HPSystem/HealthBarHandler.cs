using EventBus;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HP
{
    public class HealthBarHandler : MonoBehaviour
    {
        [SerializeField] HPComponent EntityHpComponent;
        [SerializeField] Slider HealthBar;


        private void Awake()
        {
            HealthBar.maxValue = EntityHpComponent.MaxHealth;
            HealthBar.value = EntityHpComponent.MaxHealth;
        }
        private void OnEnable()
        {
            EntityHpComponent.OnDamageTaken += updateBar;
        }

        private void OnDisable()
        {
            EntityHpComponent.OnDamageTaken -= updateBar;
        }

        public void updateBar(int newHealth)
        {
            HealthBar.value = newHealth;
        }

    }
}