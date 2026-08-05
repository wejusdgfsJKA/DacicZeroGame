using UnityEngine;
using UnityEngine.AI;
using Weapons;
namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Tasks/Swing Melee Weapon")]
    public class SwingMeleeWeapon : Leaf
    {
        [SerializeField] BotWeapon weapon;
        [SerializeField] Vector3Reference target;
        [SerializeField] NavMeshAgent agent;
        [SerializeField] Transform self;
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override NodeResult Execute()
        {
            if ((target.Value - self.position).magnitude >= weapon.Range) { weapon.Firing = false;  return NodeResult.failure;  }
            if (!weapon.Firing)
            {
                self.LookAt(new Vector3(target.Value.x, self.position.y, target.Value.z));
                weapon.transform.LookAt(target.Value);
                if(weapon.cooldownTo < Time.time)
                    weapon.Firing = true;
            }
            return NodeResult.running;
        }
        public override void OnExit()
        {
            base.OnExit();
            weapon.Firing = false;
        }
    }
}
