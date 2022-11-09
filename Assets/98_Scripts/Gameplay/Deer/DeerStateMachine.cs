using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerStateMachine : BaseStateMachine
{
    private BaseState iDLEState, moveState, eatState;
    private Deer deer;

    public DeerStateMachine(Deer newDeer)
    {
        deer = newDeer;
        iDLEState = new IDLEState();
        moveState = new MoveState(deer.Agent, deer.Target);
        eatState = new EatState();
        DefineTransition();
        ChangeCurrentState(iDLEState);
    }

    public override void DefineTransition()
    {
        iDLEState.AddTransition(TargetNotInRange, moveState);
        moveState.AddTransition(TargetInRange, iDLEState);
    }

    private bool TargetNotInRange()
    {
        return Vector3.Distance(deer.Target.transform.position, deer.transform.position) > deer.MaxDistance;
    }

    private bool TargetInRange()
    {
        return Vector3.Distance(deer.Target.transform.position, deer.transform.position) < deer.MaxDistance;
    }
}