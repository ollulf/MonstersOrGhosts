using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaClone : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private GameObject clone;

    private GameObject target;

    private BaseStateMachine stateMachine;

    private Rigidbody rigidbody;



    public GameObject Target { get => target;}
    public Rigidbody _Rigidbody { get => rigidbody;}
    public float MovementSpeed { get => movementSpeed; }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        stateMachine = new BacteriaStateMachine(this);
    }

    void Update()
    {
        stateMachine.ExecuteState();
    }

    public void SetTarget() => target = BacteriaFoodHandler.GetFood();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BacteriaFood")
        {
            collision.gameObject.GetComponent<BacteriaFood>().RemoveFromList();
            Destroy(collision.gameObject);
            Instantiate(clone, transform.position, Quaternion.identity);
            //PhotonNetwork.Instantiate("BacteriaGame/BacteriaClone", transform.position, Quaternion.identity);
        }
    }
}
