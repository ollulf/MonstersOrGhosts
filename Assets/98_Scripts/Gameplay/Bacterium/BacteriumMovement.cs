using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BacteriumMovement : MonoBehaviourPun
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private AnimationCurve addingForce;

    private Rigidbody rigidbody;
    private Timer timer;

    private bool timerRunning;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        timer = new Timer();
        timer.SetStartTime(0, false);
        timerRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("HUI");
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
                    Debug.Log("OHH");
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
            Destroy(collision.gameObject);
            PhotonNetwork.Instantiate("BacteriaGame/BacteriaClone", transform.position, Quaternion.identity);
        }
    }

    private void RotationBacterium()
    {
        transform.Rotate(new Vector3(Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")) * rotationSpeed);
    }

    private void Move()
    {
        float velocityMoment = addingForce.Evaluate(timer.CurrentTime);

        if(velocityMoment <= 0)
        {
            velocityMoment = 0;
        }

        rigidbody.velocity = transform.forward * velocityMoment;

    }
}
