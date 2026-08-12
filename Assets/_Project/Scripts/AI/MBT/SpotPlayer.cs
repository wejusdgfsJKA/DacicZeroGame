using PlayerController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Weapons;
namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Tasks/SpotPlayer")]
    public class SpotPlayer : Leaf
    {
        [SerializeField] BotGun gun;
        [SerializeField] Vector3Reference targetPos;
        [SerializeField] Transform self;
        [SerializeField] ScannerBotComponent scanner;
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override NodeResult Execute()
        {
            scanner.TargetPosition = targetPos.Value;
            self.LookAt(new Vector3(targetPos.Value.x, self.position.y, targetPos.Value.z));
            scanner.AngularSpeed = 0;
            Collider[] targetColliders = new Collider[10];
            Physics.OverlapSphereNonAlloc(targetPos.Value, 10f, targetColliders, LayerMask.GetMask("Player"));
            var target = targetColliders[0].transform.root;
            var stealthComponent = target.GetComponent<PlayerStealthController>();
            if (stealthComponent != null)
            {
                StartCoroutine(stealthComponent.GetSpottedBy(self));
                return NodeResult.running;
            }
            return NodeResult.failure;
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
