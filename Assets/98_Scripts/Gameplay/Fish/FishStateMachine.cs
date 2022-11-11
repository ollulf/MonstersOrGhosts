using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishStateMachine : BaseStateMachine
{
    private BaseState iDleState, moveState;
    private FalseFlock falseFlock;

    public FishStateMachine(FalseFlock newFalseFlock)
    {
        falseFlock = newFalseFlock;
        iDleState = new IDLEState();
        moveState = new FishMoveState(newFalseFlock);
        DefineTransition();
        ChangeCurrentState(iDleState);
    }

    public override void DefineTransition()
    {
        iDleState.AddTransition(TargetOutOfRange, moveState);
        moveState.AddTransition(TargetInRange, iDleState);
    }

    public bool TargetOutOfRange() => Vector3.Distance(falseFlock.transform.position, falseFlock.Target.transform.position) > falseFlock.MaxDistance;
    public bool TargetInRange() => Vector3.Distance(falseFlock.transform.position, falseFlock.Target.transform.position) <= falseFlock.MaxDistance;
}
