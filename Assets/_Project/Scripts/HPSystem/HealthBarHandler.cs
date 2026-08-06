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
        [SerializeField] Slider FancyHealthBarBackground;


        private void Awake()
        {
            HealthBar.maxValue = EntityHpComponent.MaxHealth;
            HealthBar.value = EntityHpComponent.MaxHealth;

            FancyHealthBarBackground.maxValue = EntityHpComponent.MaxHealth;
            FancyHealthBarBackground.value = EntityHpComponent.MaxHealth;
        }
        private void OnEnable()
        {
            EntityHpComponent.OnDamageTaken += updateBar;
        }

        public void Update()
        {
            FancyHealthBarBackground.value = Mathf.Lerp(FancyHealthBarBackground.value, 
                HealthBar.value, 
                Mathf.Max(1.5f, (FancyHealthBarBackground.value + HealthBar.value)/2) * Time.deltaTime);
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