using KBCore.Refs;
using UnityEngine;
namespace Detection
{
    public class DetectionSystem : ValidatedMonoBehaviour
    {
        [field: SerializeField] public bool CanSee { get; set; } = true;
        [field: SerializeField] public bool CanHear { get; set; } = true;
        //some entities may share a common detection memory
        public DetectionMemory Memory { get; set; }
        /// <summary>
        /// How far can we hear?
        /// </summary>
        public float AudioRange { get; protected set; }
        /// <summary>
        /// How far can we see?
        /// </summary>
        public float VisualRange { get; protected set; }
        /// <summary>
        /// What is our field of view?
        /// </summary>
        public float VisualAngle { get; protected set; }
        /// <summary>
        /// What can we NOT see through?
        /// </summary>
        LayerMask obstructionMask = 1 << 0;
        public float ProximityRange { get; protected set; }
        protected DetectionStrategy strategy;
        private void OnEnable()
        {
            if (Memory == null)
            {
                Memory = new();
            }
        }
        void Detect()
        {
            //go over all entities in the EntityManager?
        }
        /// <summary>
        /// Check to see if we can detect a given entity.
        /// </summary>
        /// <param name="entity">The entity to check against.</param>
        /// <returns>True if we can detect this entity.</returns>
        bool CanDetect(DetectableTarget entity)
        {
            float dist = Vector3.Distance(entity.transform.position, transform.position);
            if (dist <= ProximityRange)
            {
                return true;
            }
            if (CanHear && dist <= entity.CurrentSound + AudioRange)
            {
                //we can hear this entity
                return true;
            }
            if (CanSee)
            {
                //check distance
                if (dist > VisualRange)
                {
                    return false;
                }
                //check visual angle
                if (Vector3.Angle(transform.forward, entity.transform.position -
                    transform.position) > VisualAngle / 2)
                {
                    return false;
                }
                //check for obstructions
                if (Physics.Linecast(transform.position, entity.transform.position,
                    obstructionMask))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}