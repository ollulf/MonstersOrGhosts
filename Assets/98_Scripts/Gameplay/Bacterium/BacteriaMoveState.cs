using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaMoveState : BaseState
{
    private BacteriaClone clone;

    public BacteriaMoveState(BacteriaClone newClone)
    {
        clone = newClone;
    }

    public override void OnEnter()
    {
        Debug.Log(clone.Target);
    }

    public override void OnExecute()
    {
        if (clone.Target != null)
        {
            Vector3 direction = (clone.Target.transform.position - clone.transform.position).normalized;

            clone._Rigidbody.transform.position += direction * clone.MovementSpeed * Time.deltaTime;
            Debug.Log("Move to target");
        }
    }

    public override void OnExit()
    {

    }
}
