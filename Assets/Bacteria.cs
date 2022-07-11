using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bacteria : MonoBehaviour
{
    [SerializeField] private float forceIntensity = 1;

    private Transform center;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        //center = GetComponentInParent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (center == null)
        return;

        Vector3 pushDirection = center.position - gameObject.transform.position;
        rb.AddForce(pushDirection * forceIntensity);
    }

    public void SetCenterTransform(Transform transform)
    {
        center = transform;
    }
}
