public class BacteriaStateMachine : BaseStateMachine
{
    private BacteriaClone bacteriaClone;

    private BaseState trackState, moveState;

    public BacteriaStateMachine(BacteriaClone newClone)
    {
        bacteriaClone = newClone;
        trackState = new TrackState(newClone);
        moveState = new BacteriaMoveState(newClone);
        DefineTransition();
        ChangeCurrentState(trackState);
    }

    public override void DefineTransition()
    {
        trackState.AddTransition(GetTarget, moveState);
        moveState.AddTransition(LostTarget,trackState);
    }

    public bool GetTarget() => bacteriaClone.Target != null;
    public bool LostTarget() => bacteriaClone.Target == null;
}