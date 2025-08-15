using EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Detection
{
    public class DetectionSystem : MonoBehaviour
    {
        #region Parameters
        [SerializeField] protected DetectionParameters @params;
        #endregion
        #region Targets & Sounds
        public Dictionary<Transform, TargetData> Targets { get; } = new();
        public TargetData ClosestTarget { get; protected set; }
        protected float closestTargetDist;
        public HashSet<SoundData> Sounds { get; } = new();
        public SoundData ClosestSound { get; protected set; }
        protected float closestSoundDist;
        #endregion
        #region Other fields
        //cache this for performance/convenience and debugging
        protected LayerMask targetMask;
        protected WaitForSeconds wait;
        protected Coroutine coroutine;
        protected Collider[] targetBuffer = new Collider[GlobalSettings.MaxTargets];
        #endregion
        #region Debugging
        private void OnDrawGizmosSelected()
        {
            #region Visual 
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, @params.VisualRange);

            float halfFov = @params.VisualAngle / 2;

            // Calculate left & right and up & down directions for fov
            Quaternion leftRotation = Quaternion.Euler(0, -halfFov, 0);
            Quaternion rightRotation = Quaternion.Euler(0, halfFov, 0);
            Quaternion upRotation = Quaternion.Euler(halfFov, 0, 0);
            Quaternion downRotation = Quaternion.Euler(-halfFov, 0, 0);

            Vector3 leftDirection = leftRotation * transform.forward;
            Vector3 rightDirection = rightRotation * transform.forward;
            Vector3 upDirection = upRotation * transform.forward;
            Vector3 downDirection = downRotation * transform.forward;

            Gizmos.DrawLine(transform.position, transform.position + leftDirection * @params.VisualRange);
            Gizmos.DrawLine(transform.position, transform.position + rightDirection * @params.VisualRange);
            Gizmos.DrawLine(transform.position, transform.position + upDirection * @params.VisualRange);
            Gizmos.DrawLine(transform.position, transform.position + downDirection * @params.VisualRange);
            #endregion

            #region Audible
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, @params.AudioRange);
            #endregion

            #region Proximity
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, @params.ProximityRange);
            #endregion
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
                Gizmos.color = new Color(0, (@params.TimeToForgetSound - (Time.time - sound.TimeHeard)) / @params.TimeToForgetSound, 0);
                Gizmos.DrawLine(transform.position, sound.Position);
            }
        }
        #endregion
        #region Setup
        protected void Awake()
        {
            wait = new WaitForSeconds(@params.UpdateCooldown);
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
                @params.VisualRange, targetBuffer, targetMask);
            for (int i = 0; i < targetCount; i++)
            {
                var tr = targetBuffer[i].transform.root;
                //check for proximity
                if (Vector3.Distance(transform.position, tr.position) <= @params.ProximityRange)
                {
                    Detected(tr);
                    continue;
                }
                //check for visual
                if (!CanSee(tr.position)) continue;
                Detected(tr);
            }
        }
        public bool CanSee(Vector3 pos)
        {
            Vector3 VectorToTarget = pos - transform.position;
            VectorToTarget.Normalize();
            if (Vector3.Dot(VectorToTarget, transform.forward) < Mathf.Cos(Mathf.Deg2Rad * @params.VisualAngle / 2))
            {
                return false;
            }
            if (Physics.Linecast(transform.position, pos, @params.ObstructionMask))
            {
                return false;
            }
            return true;
        }
        public void Detected(Transform target)
        {
            TargetData targetData;
            if (Targets.TryGetValue(target, out targetData))
            {
                targetData.TimeLastSpotted = Time.time;
                targetData.Awareness += @params.AwarenessBuildRate;
                targetData.LastKnownPosition = target.position;
                targetData.Spotted = true;
            }
            else
            {
                Targets.Add(target, new TargetData(target));
            }
        }
        protected bool Invalid(TargetData target)
        {
            return target.Transform == null ||
                    target.Transform.gameObject == null ||
                    !target.Transform.gameObject.activeSelf ||
                    Time.time - target.TimeLastSpotted > @params.TimeToForgetTarget ||
                    (CanSee(target.LastKnownPosition) && !target.Spotted && target.Awareness < 0.5f);

        }
        protected void ProcessInformation()
        {
            #region Targets
            ClosestTarget = null;
            Queue<TargetData> targetsToRemove = new();
            foreach (var target in Targets.Values)
            {
                if (Invalid(target))
                {
                    targetsToRemove.Enqueue(target);
                    continue;
                }
                target.Spotted = false;
                if (target.Awareness >= 0.5f)
                {
                    target.LastKnownPosition = target.Transform.position;
                }
                target.Awareness -= @params.AwarenessLossRate;
                if (ClosestTarget == null)
                {
                    ClosestTarget = target;
                    closestTargetDist = Vector3.Distance(transform.position, target.LastKnownPosition);
                    continue;
                }
                if (ClosestTarget.Awareness >= 0.5f && target.Awareness < 0.5f) continue;
                float newDist = Vector3.Distance(transform.position, target.LastKnownPosition);
                if (newDist < closestTargetDist)
                {
                    closestTargetDist = newDist;
                    ClosestTarget = target;
                }
            }
            TargetData target2;
            while (targetsToRemove.TryDequeue(out target2))
            {
                Targets.Remove(target2.Transform);
            }
            #endregion
            #region Sounds
            ClosestSound = null;
            Queue<SoundData> soundsToRemove = new();
            foreach (var sound in Sounds)
            {
                if (Time.time - sound.TimeHeard > @params.TimeToForgetSound)
                {
                    soundsToRemove.Enqueue(sound);
                    continue;
                }
                if (ClosestSound == null)
                {
                    ClosestSound = sound;
                    closestSoundDist = Vector3.Distance(transform.position, sound.Position);
                    continue;
                }
                float newDist = Vector3.Distance(transform.position, sound.Position);
                if (newDist < closestSoundDist)
                {
                    ClosestSound = sound;
                    closestSoundDist = newDist;
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
            if (soundEvent.Intensity + @params.AudioRange < Vector3.Distance(transform.position, soundEvent.Position)) return;
            Sounds.Add(new SoundData(soundEvent));
        }
        #endregion
    }
}