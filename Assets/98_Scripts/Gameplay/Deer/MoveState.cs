using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveState : BaseState
{
    private NavMeshAgent agent;
    private GameObject target;

    public MoveState(NavMeshAgent newAgent, GameObject newTarget)
    {
        agent = newAgent;
        target = newTarget;
    }

    public override void OnEnter()
    {
        agent.isStopped = false;
    }

    public override void OnExecute()
    {
        Move();
    }


    public override void OnExit()
    {
        agent.isStopped = true;
    }
    private void Move()
    {
        agent.SetDestination(target.transform.position);
    }
}
