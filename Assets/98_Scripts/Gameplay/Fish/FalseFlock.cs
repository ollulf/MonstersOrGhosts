using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseFlock : MonoBehaviour
{
    private BaseStateMachine fishControll;

    [SerializeField] private float movementSpeed, maxDistance, offSetToTarget;

    private GameObject target;
    private Rigidbody _rigidbody;

    public float MovementSpeed { get => movementSpeed;}
    public float MaxDistance { get => maxDistance;}
    public GameObject Target { get => target;}
    public float OffSetToTarget { get => offSetToTarget;}
    public Rigidbody _Rigidbody { get => _rigidbody;}

    void Update()
    {
        if (target != null)
        {
            fishControll.ExecuteState();
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        _rigidbody = GetComponent<Rigidbody>();
        fishControll = new FishStateMachine(this);

    }
}
