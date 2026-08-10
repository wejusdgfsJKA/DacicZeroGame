using EventBus;
namespace Weapons {
    public struct StunEvent : IEvent {
        public float Duration;
        public StunEvent(float duration) {
            Duration = duration;
        }
    }
}
