using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Deer : MonoBehaviour
{
    private BaseStateMachine deerControll;

    [SerializeField] private float movementSpeed, maxDistance;

    [SerializeField] private GameObject target;

    private NavMeshAgent agent;

    public GameObject Target { get => target;}
    public float MovementSpeed { get => movementSpeed;}
    public float MaxDistance { get => maxDistance;}
    public NavMeshAgent Agent { get => agent;}

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        deerControll = new DeerStateMachine(this);
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            deerControll.ExecuteState();
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }
}
