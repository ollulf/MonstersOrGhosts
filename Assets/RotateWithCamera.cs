using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithCamera : MonoBehaviour
{
    public Transform orientationTransform;

    public float speedH = 1.0f, speedV = 1.0f;
    public float yawMin = -45f, yawMax = 45f, pitchMin = -20f, pitchMax = 20f;

    private float yaw = 0.0f, pitch = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        yaw = Mathf.Clamp(yaw, yawMin, yawMax);
        //the rotation range
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        //the rotation range

        Vector3 tempAngles = orientationTransform.eulerAngles;
        transform.eulerAngles = new Vector3(pitch + tempAngles.x, yaw + tempAngles.y, 0.0f);
    }
}
