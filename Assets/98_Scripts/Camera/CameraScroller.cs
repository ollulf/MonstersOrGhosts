using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class CameraScroller : MonoBehaviourPun
{
    [SerializeField] Transform farPosition, closePosition, animalSight;
    [SerializeField] AnimationCurve acellerationCurve;
    [SerializeField] float speed = 1f;

    [ShowNonSerializedField] private float distance, currentDistance = 1;

    [SerializeField] private LayerMask mask;

    private int index;
    private Vector3 fakeVector;
    private Quaternion fakeAngle;
    [ShowNonSerializedField] private bool fin;

    private void Awake()
    {
        index = 1;
        fin = false;
        if (!base.photonView.IsMine)
            Destroy(this.gameObject);
    }

    void Start()
    {
        gameObject.transform.position = closePosition.position;
        distance = Vector3.Distance(farPosition.position, closePosition.position);
    }

    public void SetFarPosition(Transform newTransform)
    {
        farPosition = newTransform;
    }

    public void SetIndex(int newInt)
    {
        index = newInt;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(farPosition.position, closePosition.position);
        float t = acellerationCurve.Evaluate(currentDistance);

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) //forward
        {
            index--;
            if (index < 0)
            {
                index = 0;
            }
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) //backwards
        {
            index++;
            if (index > 2)
            {
                index = 2;
            }
        }
        IndexChecker();
        MouseCheck();
    }

    private void IndexChecker()
    {
        switch (index)
        {
            case 0:
                {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, animalSight.position, speed * Time.deltaTime);
                    Quaternion rotation = Quaternion.Lerp(Quaternion.Euler(gameObject.transform.eulerAngles), Quaternion.Euler(animalSight.eulerAngles), speed * Time.deltaTime);
                    gameObject.transform.eulerAngles = rotation.eulerAngles;
                    break;
                }
            case 1:
                {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, closePosition.position, speed * Time.deltaTime);
                    Quaternion rotation = Quaternion.Lerp(Quaternion.Euler(gameObject.transform.eulerAngles), Quaternion.Euler(closePosition.eulerAngles), speed * Time.deltaTime);
                    gameObject.transform.eulerAngles = rotation.eulerAngles;
                    fin = false;
                    break;
                }
            case 2:
                {
                    if (!fin)
                    {
                        fin = true;
                        fakeVector = closePosition.position;
                        fakeAngle = Quaternion.Euler(closePosition.eulerAngles);
                    }

                    fakeVector = Vector3.Lerp(fakeVector, farPosition.localPosition, speed * Time.deltaTime);
                    fakeAngle =  Quaternion.Lerp(fakeAngle, Quaternion.Euler(farPosition.eulerAngles), speed * Time.deltaTime);

                    gameObject.transform.position = fakeVector;
                    gameObject.transform.eulerAngles = fakeAngle.eulerAngles;
                    break;
                }
        }
    }

    private void MouseCheck()
    {
        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Machine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.yellow, Mathf.Infinity);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                {
                    Debug.Log(hit.collider.gameObject);

                    if (hit.collider.gameObject.layer == 8)
                    {
                        hit.collider.gameObject.GetComponent<ShowBuildUI>().CanvasShow();
                    }
                }
            }

        }

    }
}
