using UnityEngine;

public class BacteriaClone : MonoBehaviour
{
    [SerializeField] private float movementSpeed, lifeTime;
    [SerializeField] private GameObject clone;

    private GameObject target;

    private BaseStateMachine stateMachine;

    private Rigidbody rigidbody;

    private Timer timer;

    public GameObject Target { get => target;}
    public Rigidbody _Rigidbody { get => rigidbody;}
    public float MovementSpeed { get => movementSpeed; }

    void Start()
    {
        timer = new Timer(lifeTime, true);
        rigidbody = GetComponent<Rigidbody>();
        stateMachine = new BacteriaStateMachine(this);
    }

    void Update()
    {
        timer.Tick();
        stateMachine.ExecuteState();

        if(timer.CurrentTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget() => target = BacteriaFoodHandler.GetFood();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BacteriaFood")
        {
            collision.gameObject.GetComponent<BacteriaFood>().RemoveFromList();
            Destroy(collision.gameObject);
            Instantiate(clone, transform.position, Quaternion.identity);
        }
    }
}
