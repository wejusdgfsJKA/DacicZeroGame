using UnityEngine;
namespace Detection
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DetectionParams")]
    [System.Serializable]
    public class DetectionParameters : ScriptableObject
    {
        [field: SerializeField] public float UpdateCooldown { get; protected set; }
        [field: SerializeField] public float TimeToForgetTarget { get; protected set; }
        [field: SerializeField] public float AwarenessBuildRate { get; protected set; }
        [field: SerializeField] public float AwarenessLossRate { get; protected set; }
        [field: SerializeField] public float TimeToForgetSound { get; protected set; }
        /// <summary>
        /// How far can we hear?
        /// </summary>
        [field: SerializeField] public float AudioRange { get; protected set; }
        /// <summary>
        /// How far can we see?
        /// </summary>
        [field: SerializeField] public float VisualRange { get; protected set; }
        /// <summary>
        /// What is our field of view?
        /// </summary>
        [field: SerializeField] public float VisualAngle { get; protected set; }
        /// <summary>
        /// What can we NOT see through?
        /// </summary>
        [field: SerializeField] public LayerMask ObstructionMask { get; protected set; } = 1 << 0;
        [field: SerializeField] public float ProximityRange { get; protected set; }
    }
}
