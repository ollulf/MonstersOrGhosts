using UnityEngine;

public class DeerStateMachine : BaseStateMachine
{
    private BaseState iDLEState, moveState;
    private Deer deer;

    public DeerStateMachine(Deer newDeer)
    {
        deer = newDeer;
        iDLEState = new IDLEState();
        moveState = new MoveState(deer.Agent, deer.Target);
        DefineTransition();
        ChangeCurrentState(iDLEState);
    }

    public override void DefineTransition()
    {
        iDLEState.AddTransition(TargetNotInRange, moveState);
        moveState.AddTransition(TargetInRange, iDLEState);
    }

    private bool TargetNotInRange() => Vector3.Distance(deer.Target.transform.position, deer.transform.position) > deer.MaxDistance && deer.Target != null;

    private bool TargetInRange() => Vector3.Distance(deer.Target.transform.position, deer.transform.position) < deer.MaxDistance;
}