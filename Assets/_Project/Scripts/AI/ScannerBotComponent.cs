

using AI;
using Detection;
using UnityEngine;

public class ScannerBotComponent : MonoBehaviour
{
    [SerializeField] public float AngularSpeed;
    [SerializeField] public float TargetSwitchCooldown;
    public float TargetSwitchCooldownTo;
    public Vector3 TargetPosition;
    bool isTurning;

    void Update()
    {
        if (isTurning)
        {
            RotateTowardsTarget();
        }
        else if (TargetSwitchCooldownTo < Time.time)
        {
            isTurning = true;
        }
    }

    void RotateTowardsTarget()
    {
        Vector3 dir = TargetPosition - transform.position;
        if (ReachedTarget())
        {
            TargetSwitchCooldownTo = Time.time + TargetSwitchCooldown;
            isTurning = false;
        }

        Quaternion targetRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            AngularSpeed * Time.deltaTime
        );
    }

    public bool ReachedTarget()
    {
        Vector3 dir = TargetPosition - transform.position;
        var angleDifference = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(dir));
        return angleDifference < 0.1f;
    }
}