using EventBus;
using UnityEngine;
namespace Detection
{
    /// <summary>
    /// This is meant to fire whenever the entity's sound level changes.
    /// </summary>
    public struct OnSoundChanged : IEvent
    {
        public float Value;
        public OnSoundChanged(float Value)
        {
            this.Value = Value;
        }
    }
    public abstract class DetectableTarget : MonoBehaviour
    {
        /// <summary>
        /// How loud the entity is.
        /// </summary>
        public float CurrentSound { get; protected set; } = 0;
        protected virtual void OnEnable()
        {
            if (!EventBus<OnSoundChanged>.AddBinding(transform.GetInstanceID()))
            {
                Debug.LogError($"Unable to add OnSoundChanged binding for entity {transform}.");
            }
            else
            {
                if (!EventBus<OnSoundChanged>.AddActions(transform.GetInstanceID(), UpdateSound))
                {
                    Debug.LogError($"Unable to add action for OnSoundChanged binding for entity {transform}.");
                }
            }
        }
        public void UpdateSound(OnSoundChanged @event)
        {
            CurrentSound += @event.Value;
        }
        protected virtual void OnDisable()
        {
            if (!EventBus<OnSoundChanged>.RemoveBinding(transform.GetInstanceID()))
            {
                Debug.LogError($"Unable to remove OnSoundChanged binding for entity {transform}.");
            }
        }
    }
}