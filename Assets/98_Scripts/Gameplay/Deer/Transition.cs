using UnityEngine;

public class Transition : MonoBehaviour
{
    public System.Func<bool> condition;
    public BaseState targetState;

    public Transition(System.Func<bool> condition, BaseState targetState)
    {
        this.condition = condition;
        this.targetState = targetState;
    }
}