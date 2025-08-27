using UnityEngine;

public class FishMoveState : BaseState
{
    private FalseFlock falseFlock;
    private float offsetX, offsetY, offsetZ;

    public FishMoveState(FalseFlock newFalseFlock) => falseFlock = newFalseFlock;

    public float OffsetX => offsetX;
    public float OffsetY => offsetY;
    public float OffsetZ => offsetZ;

    public override void OnEnter()
    {
        offsetX = Random.Range(-falseFlock.OffSetToTarget, falseFlock.OffSetToTarget);
        offsetY = Random.Range(-falseFlock.OffSetToTarget, falseFlock.OffSetToTarget);
        offsetZ = Random.Range(-falseFlock.OffSetToTarget, falseFlock.OffSetToTarget);
    }

    public override void OnExecute()
    {
        Vector3 direction = (new Vector3(falseFlock.Target.transform.position.x + offsetX, falseFlock.Target.transform.position.y + offsetY, falseFlock.Target.transform.position.z + offsetZ) - falseFlock.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
        Vector3 rotation = lookRotation.eulerAngles;
        falseFlock.transform.LookAt(new Vector3(falseFlock.Target.transform.position.x + offsetX, falseFlock.Target.transform.position.y + offsetY, falseFlock.Target.transform.position.z + offsetZ));

        if (Vector3.Distance(new Vector3(falseFlock.Target.transform.position.x + offsetX, falseFlock.Target.transform.position.y + offsetY, falseFlock.Target.transform.position.z + offsetZ), falseFlock.transform.position) > falseFlock.MaxDistance)
        {
            falseFlock._Rigidbody.linearVelocity = direction * falseFlock.MovementSpeed;
        }
        else
        {
            falseFlock._Rigidbody.linearVelocity = Vector3.zero;
        }
    }

    public override void OnExit()
    {

    }
}
