using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackState : BaseState
{
    private BacteriaClone clone;

    public TrackState(BacteriaClone newClone)
    {
        clone = newClone;
    }

    public override void OnEnter()
    {
    }

    public override void OnExecute()
    {
        if (BacteriaFoodHandler.Food.Count > 0)
        {
            Debug.Log("Tracking target");
            clone.SetTarget();
        }

    }

    public override void OnExit()
    {

    }
}
