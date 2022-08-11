using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bacteria : MonoBehaviour
{
    [SerializeField] private float forceIntensity = 1;

    private Transform center;
    private Rigidbody rb;

    private bool explode;
    // Start is called before the first frame update
    void Start()
    {
        //center = GetComponentInParent<Transform>();
        rb = GetComponent<Rigidbody>();
        explode = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (center == null)
            return;

        Vector3 pushDirection = center.position - gameObject.transform.position;
        if (!explode)
        {
            rb.AddForce(pushDirection * forceIntensity);
        }
        else
        {
            rb.AddForce(-pushDirection * forceIntensity);
        }
    }

    public void SetCenterTransform(Transform transform)
    {
        center = transform;
    }

    public void LetsExplode()
    {
        explode = true;
    }
}
