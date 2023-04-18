using UnityEngine;
using UnityEngine.AI;

public class Deer : MonoBehaviour
{
    private BaseStateMachine deerControll;

    [SerializeField] private float movementSpeed, maxDistance;

    [SerializeField] private GameObject target;

    private NavMeshAgent agent;

    public GameObject Target => target;
    public float MovementSpeed => movementSpeed;
    public float MaxDistance => maxDistance;
    public NavMeshAgent Agent => agent;

    void Start() => agent = GetComponent<NavMeshAgent>();

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
        deerControll = new DeerStateMachine(this);
    }
}