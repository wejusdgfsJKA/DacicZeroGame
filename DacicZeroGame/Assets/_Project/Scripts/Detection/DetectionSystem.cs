using EventBus;
using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Detection
{
    public class DetectionSystem : ValidatedMonoBehaviour
    {
        #region Parameters
        [SerializeField] protected float UpdateCooldown;
        [SerializeField] protected float TimeToLose;
        [SerializeField] protected float TimeToForgetTarget;
        [SerializeField] protected float awarenessBuildRate;
        [SerializeField] protected float awarenessLossRate;
        [SerializeField] protected float TimeToForgetSound;
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
        [SerializeField] protected LayerMask obstructionMask = 1 << 0;
        [field: SerializeField] public float ProximityRange { get; protected set; }
        #endregion
        #region Other fields
        //cache this for performance/convenience and debugging
        [SerializeField] protected LayerMask targetMask;
        public Dictionary<Transform, TargetData> Targets { get; } = new();
        public TargetData ClosestTarget { get; protected set; }
        public HashSet<SoundData> Sounds { get; } = new();
        protected WaitForSeconds wait;
        protected Coroutine coroutine;
        protected Collider[] targetBuffer = new Collider[GlobalSettings.MaxTargets];
        #endregion
        #region Debugging
        private void OnDrawGizmosSelected()
        {
            //show detection ranges
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, VisualRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, AudioRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ProximityRange);
        }
        private void OnDrawGizmos()
        {
            foreach (var target in Targets.Values)
            {
                if (target.LastKnownPosition != null)
                {
                    Gizmos.color = new Color(target.Awareness, 0, 0);
                    Gizmos.DrawLine(transform.position, (Vector3)target.LastKnownPosition);
                }
            }

            foreach (var sound in Sounds)
            {
                Gizmos.color = new Color(0, (TimeToForgetSound - (Time.time - sound.TimeHeard)) / TimeToForgetSound, 0);
                Gizmos.DrawLine(transform.position, sound.Position);
            }
        }
        #endregion
        #region Setup
        protected void Awake()
        {
            wait = new WaitForSeconds(UpdateCooldown);
            targetMask = GlobalSettings.TargetMasks[gameObject.layer];
        }
        protected void OnEnable()
        {
            if (!EventBus<SoundEvent>.AddActions(0, HeardSound))
            {
                Debug.LogError($"{transform} unable to add action to SoundEvent bus.");
            }
            Targets.Clear();
            Sounds.Clear();
            coroutine = StartCoroutine(enumerator());
        }
        protected void OnDisable()
        {
            if (!EventBus<SoundEvent>.RemoveActions(0, HeardSound))
            {
                Debug.LogError($"{transform} unable to remove action from SoundEvent bus.");
            }
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        #endregion
        #region Main methods
        protected IEnumerator enumerator()
        {
            while (true)
            {
                yield return wait;
                Detect();
                ProcessInformation();
            }
        }
        protected void Detect()
        {
            //gather all nearby targets
            int targetCount = Physics.OverlapSphereNonAlloc(transform.position,
                VisualRange, targetBuffer, targetMask);
            for (int i = 0; i < targetCount; i++)
            {
                var tr = targetBuffer[i].transform.root;
                //check for proximity
                if (Vector3.Distance(transform.position, tr.position) <= ProximityRange)
                {
                    Detected(tr);
                    continue;
                }
                //check for visual
                Vector3 VectorToTarget = tr.position - transform.position;
                VectorToTarget.Normalize();
                if (Vector3.Dot(VectorToTarget, transform.forward) < Mathf.Cos(Mathf.Deg2Rad * VisualAngle / 2))
                {
                    continue;
                }
                if (Physics.Linecast(transform.position, tr.position, obstructionMask))
                {
                    continue;
                }
                Detected(tr);
            }
        }
        public void Detected(Transform target)
        {
            TargetData targetData;
            if (Targets.TryGetValue(target, out targetData))
            {
                targetData.TimeLastSpotted = Time.time;
                targetData.Awareness += awarenessBuildRate;
                targetData.LastKnownPosition = target.position;
            }
            else
            {
                Targets.Add(target, new TargetData(target));
            }
        }
        protected void ProcessInformation()
        {
            #region Targets
            Queue<TargetData> targetsToRemove = new();
            foreach (var target in Targets.Values)
            {
                if (target.Transform == null ||
                    target.Transform.gameObject == null ||
                    !target.Transform.gameObject.activeSelf ||
                    Time.time - target.TimeLastSpotted > TimeToForgetTarget)
                {
                    targetsToRemove.Enqueue(target);
                    continue;
                }
                if (target.Awareness >= 0.5f)
                {
                    target.LastKnownPosition = target.Transform.position;
                }
                target.Awareness -= awarenessLossRate;
            }
            TargetData target2;
            while (targetsToRemove.TryDequeue(out target2))
            {
                Targets.Remove(target2.Transform);
            }
            #endregion
            #region Sounds
            Queue<SoundData> soundsToRemove = new();
            foreach (var sound in Sounds)
            {
                if (Time.time - sound.TimeHeard > TimeToForgetSound)
                {
                    soundsToRemove.Enqueue(sound);
                    continue;
                }
            }
            SoundData sound2;
            while (soundsToRemove.TryDequeue(out sound2))
            {
                Sounds.Remove(sound2);
            }
            #endregion
        }
        public void HeardSound(SoundEvent soundEvent)
        {
            //this sound was made by a friend
            if (soundEvent.Team == gameObject.layer) return;
            //this sound is too far away
            if (soundEvent.Intensity + AudioRange < Vector3.Distance(transform.position, soundEvent.Position)) return;
            Sounds.Add(new SoundData(soundEvent));
        }
        #endregion
    }
}