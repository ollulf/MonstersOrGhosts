using UnityEngine;
using Photon.Pun;

public class BacteriumMovement : MonoBehaviourPun
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private AnimationCurve addingForce;
    [SerializeField] private GameObject clone;

    private Rigidbody _rigidbody;
    private Timer timer;

    private bool timerRunning;

    private int acetateCount;

    public int AcetateCount => acetateCount;

    void Start()
    {
        acetateCount = 0;
        _rigidbody = GetComponent<Rigidbody>();
        timer = new Timer(0, false);
        timerRunning = false;
        CallInEndValues.SetBacteria(this);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                timerRunning = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            RotationBacterium();

            if (timerRunning)
            {
                timer.Tick();
                Move();

                if (addingForce.Evaluate(timer.CurrentTime) <= 0)
                {
                    timer.ResetTimer();
                    timerRunning = false;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "BacteriaFood")
        {
            collision.gameObject.GetComponent<BacteriaFood>().RemoveFromList();
            Destroy(collision.gameObject);
            Instantiate(clone, transform.position, Quaternion.identity);
            acetateCount++;
        }
    }

    private void RotationBacterium()
    {
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"),0) * rotationSpeed);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }

    private void Move()
    {
        float velocityMoment = addingForce.Evaluate(timer.CurrentTime);

        if(velocityMoment <= 0)
        {
            velocityMoment = 0;
        }

        _rigidbody.linearVelocity = transform.forward * velocityMoment;
    }
}
