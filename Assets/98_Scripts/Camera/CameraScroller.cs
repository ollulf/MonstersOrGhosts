using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class CameraScroller : MonoBehaviourPun
{
    [SerializeField] Transform farPosition, closePosition;
    [SerializeField] AnimationCurve acellerationCurve;
    [SerializeField] float speed = 1f;

    [ShowNonSerializedField ] private float distance, currentDistance = 1;
    private void Awake()
    {
        if (!base.photonView.IsMine) 
            Destroy(this.gameObject);
    }

    void Start()
    {
        gameObject.transform.position = farPosition.position;
        distance = Vector3.Distance(farPosition.position, closePosition.position);
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(farPosition.position, closePosition.position);
        float t = acellerationCurve.Evaluate(currentDistance);

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) //forward
        {
            currentDistance = Mathf.Clamp(currentDistance -= distance * speed * Time.deltaTime, 0, 1);
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) //backwards
        {
           currentDistance = Mathf.Clamp(currentDistance += distance * speed * Time.deltaTime, 0, 1);
           
        }

        Vector3 rotation = new Vector3 (
            Mathf.LerpAngle(farPosition.transform.eulerAngles.x, closePosition.transform.eulerAngles.x, t),
            Mathf.LerpAngle(farPosition.transform.eulerAngles.y, closePosition.transform.eulerAngles.y, t),
            Mathf.LerpAngle(farPosition.transform.eulerAngles.z, closePosition.transform.eulerAngles.z, t));
        gameObject.transform.eulerAngles = rotation;

        Vector3 position = Vector3.Lerp(farPosition.position, closePosition.position, t);
        gameObject.transform.position = position;

    }
}
