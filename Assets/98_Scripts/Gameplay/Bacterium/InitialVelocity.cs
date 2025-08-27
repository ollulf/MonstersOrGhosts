using UnityEngine;

public class InitialVelocity : MonoBehaviour
{
    [SerializeField] private float initialForce;
    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.linearVelocity = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)) * initialForce;
    }
}