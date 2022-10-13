using System.Collections.Generic;

public abstract class BaseState
{
    public List<Transition> transitions;

    public abstract void OnEnter();
    public abstract void OnExecute();
    public abstract void OnExit();

    public BaseState()
    {
        transitions = new List<Transition>();
    }

    public void AddTransition(System.Func<bool> condition, BaseState targetState)
    {
        transitions.Add(new Transition(condition, targetState));
    }
}
