using UnityEngine;

public class FalseFlock : MonoBehaviour
{
    private BaseStateMachine fishControll;

    [SerializeField] private float movementSpeed, maxDistance, offSetToTarget;

    private GameObject target;
    private Rigidbody _rigidbody;

    public float MovementSpeed => movementSpeed;
    public float MaxDistance => maxDistance;
    public GameObject Target => target;
    public float OffSetToTarget => offSetToTarget;
    public Rigidbody _Rigidbody => _rigidbody;

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