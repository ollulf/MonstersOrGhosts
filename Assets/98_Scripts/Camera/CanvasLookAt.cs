using UnityEngine;

public class CanvasLookAt : MonoBehaviour
{
    Camera mainCam;

    void Start() => mainCam = Camera.main;

    void Update()
    {
        transform.LookAt(mainCam.transform);
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
    }
}