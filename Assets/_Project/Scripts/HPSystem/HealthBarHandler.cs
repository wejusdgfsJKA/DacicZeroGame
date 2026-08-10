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
        [SerializeField] RawImage ColorOverlay;


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

            ColorOverlay.color = new Color(1f, 0f, 0f,
                    Mathf.Lerp(ColorOverlay.color.a,
                    0f,
                    5f*Time.deltaTime
                    )
                );
        }

        private void OnDisable()
        {
            EntityHpComponent.OnDamageTaken -= updateBar;
        }

        public void updateBar(int newHealth)
        {
            ColorOverlay.color = new Color(1f, 0f, 0f, ((float)(HealthBar.value - newHealth)) / 10);
            HealthBar.value = newHealth;
        }

    }
}