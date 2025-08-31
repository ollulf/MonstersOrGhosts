using UnityEngine;

public class InitialVelocity : MonoBehaviour
{
    [SerializeField] private float initialForce;
    private Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.linearVelocity = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)) * initialForce;
    }
}