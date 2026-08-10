using Detection;
using UnityEngine;
using UnityEngine.AI;
namespace MBT {
    [AddComponentMenu("")]
    [MBTNode("Tasks/Observe")]
    public class ObserveTask : Leaf {
        [SerializeField] protected TransformReference pos;
        [SerializeField] DetectionSystem detectionSystem;
        [SerializeField] NavMeshAgent agent;
        public override void OnEnter() {
            base.OnEnter();
            if (agent != null) {
                agent.isStopped = true;
            }
            if (pos != null) {
                detectionSystem.PosToLookAt = pos.Value.position;
            }
        }
        public override NodeResult Execute() {
            return NodeResult.success;
        }
        public override void OnExit() {
            base.OnExit();
            if (pos != null) {
                detectionSystem.PosToLookAt = null;
            }
        }
    }
}
