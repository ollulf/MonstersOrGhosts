using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    public float rotationSpeedX = 0f, rotationSpeedY = 0f, rotationSpeedZ = 0f;

    void Update()
    {
        transform.Rotate(
            rotationSpeedX * Time.deltaTime,
            rotationSpeedY * Time.deltaTime,
            rotationSpeedZ * Time.deltaTime
            );
    }
}
