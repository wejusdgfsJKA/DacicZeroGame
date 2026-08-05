using AI;
using Detection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
namespace MBT
{

    [AddComponentMenu("")]
    [MBTNode("Tasks/Scan")]
    public class ScanTask : Leaf
    {
        [SerializeField] Transform Bot;
        [SerializeField] TacticalBrain brain;
        [SerializeField] ScannerBotComponent Scan;
        [SerializeField] int currentPatrolPoint = 0;
        public override void OnEnter()
        {
            base.OnEnter();
            if (brain.PatrolPoints != null && brain.PatrolPoints.Length > 0)
            {
                Scan.TargetPosition = brain.PatrolPoints[currentPatrolPoint].position;
            }
        }
        public override NodeResult Execute()
        {
            var targetPos = brain.PatrolPoints[currentPatrolPoint].position;
            if (Scan.ReachedTarget())
            {
                Debug.Log("switching targets");
                currentPatrolPoint = (currentPatrolPoint + 1) % brain.PatrolPoints.Length;
                Scan.TargetPosition = brain.PatrolPoints[currentPatrolPoint].position;
            }
            return NodeResult.running;
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}