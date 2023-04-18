public abstract class BaseStateMachine
{
    protected BaseState currentState = null;

    public virtual void ChangeCurrentState(BaseState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }

        currentState = newState;

        currentState.OnEnter();
    }

    public virtual void ExecuteState()
    {
            currentState.OnExecute();
            CheckTransitions();
    }

    public virtual void CheckTransitions()
    {
        foreach (var transition in currentState.transitions)
        {
            if (transition.condition())
            {
                ChangeCurrentState(transition.targetState);
                break;
            }
        }
    }

    public abstract void DefineTransition();
}
