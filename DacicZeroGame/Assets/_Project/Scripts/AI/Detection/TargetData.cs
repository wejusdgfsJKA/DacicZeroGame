using EventBus;
using UnityEngine;
namespace Detection
{
    public struct SoundEvent : IEvent
    {
        /// <summary>
        /// How loud was the sound;
        /// </summary>
        public float Intensity;
        /// <summary>
        /// Where did the sound originate?
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// Use this so AI don't constantly investigate sounds made by their buddies.
        /// </summary>
        public int Team;
        public SoundEvent(float intensity, Vector3 position, int team)
        {
            Intensity = intensity;
            Position = position;
            Team = team;
        }
    }
    [System.Serializable]
    public class SoundData
    {
        /// <summary>
        /// How loud was the sound;
        /// </summary>
        public float Intensity;
        /// <summary>
        /// Where did the sound originate?
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// Use this so AI don't constantly investigate sounds made by their buddies.
        /// </summary>
        public int Team;
        public float TimeHeard;
        public SoundData(float intensity, Vector3 position, int team)
        {
            Intensity = intensity;
            Position = position;
            Team = team;
            TimeHeard = Time.time;
        }
        public SoundData(SoundEvent @event)
        {
            Intensity = @event.Intensity;
            Position = @event.Position;
            Team = @event.Team;
            TimeHeard = Time.time;
        }
    }
    [System.Serializable]
    public class TargetData
    {
        protected float awareness = 1;
        public float Awareness
        {
            get
            {
                return awareness;
            }
            set
            {
                awareness = Mathf.Clamp(value, 0, 1);
            }
        }
        public float TimeLastSpotted;
        public Vector3 LastKnownPosition;
        public Transform Transform;
        public bool Spotted = true;
        public TargetData(Transform tr)
        {
            Transform = tr;
            LastKnownPosition = tr.position;
            TimeLastSpotted = Time.time;
        }
    }
}