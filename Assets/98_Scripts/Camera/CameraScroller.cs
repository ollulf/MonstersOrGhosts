using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;
using System;

public class CameraScroller : MonoBehaviourPun
{
    [SerializeField] Transform farPosition, closePosition, animalSight;
    [SerializeField] AnimationCurve acellerationCurve;
    [SerializeField] float speed = 1f;

    [SerializeField] private AudioSource arcticAmbient, underwaterAmbient, enterWaterSound, exitWaterSound;
    [ShowNonSerializedField] private bool isCameraUnderwater;

    [ShowNonSerializedField] private float distance, currentDistance = 1;

    [SerializeField] private LayerMask mask;

    private GameObject selectedShip;
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


        if (!(gameObject.transform.position.y < 0))
        {
            arcticAmbient.Play();
        }
        else
        {
            underwaterAmbient.Play();
        }
            
        

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
        UpdateAmbientSound();

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
        MouseCheck();
    }

    private void LateUpdate()
    {
        IndexChecker();
    }

    private void UpdateAmbientSound()
    {
        bool tempUnderWater = isCameraUnderwater;
        isCameraUnderwater = (gameObject.transform.position.y < 0);

        if (isCameraUnderwater != tempUnderWater)
        {
            if (isCameraUnderwater)
            {
                Debug.Log("Enters Water");
                enterWaterSound.Play();
                underwaterAmbient.UnPause();
                arcticAmbient.Pause();

            }
            if (!isCameraUnderwater)
            {
                Debug.Log("Exits Water");
                exitWaterSound.Play();
                underwaterAmbient.Pause();
                arcticAmbient.UnPause();
            }
        }

    }
    private void IndexChecker()
    {
        switch (index)
        {
            case 0:
                {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, animalSight.position, speed);
                    Quaternion rotation = Quaternion.Lerp(Quaternion.Euler(gameObject.transform.eulerAngles), Quaternion.Euler(animalSight.eulerAngles), speed * Time.deltaTime);
                    gameObject.transform.eulerAngles = rotation.eulerAngles;
                    break;
                }
            case 1:
                {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, closePosition.position, speed);
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

                    fakeVector = Vector3.Lerp(fakeVector, farPosition.localPosition, 1 * Time.deltaTime);
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

                //Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.yellow, Mathf.Infinity);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject.tag == "Ship")
                    {                       
                        Debug.Log(hit.collider.gameObject);
                        closePosition = hit.collider.transform.GetChild(1);
                        speed = hit.collider.GetComponent<ShipMovement>().MovementSpeed;
                        hit.collider.GetComponent<ShipMovement>().IsSelected();
                        if(selectedShip != null)
                        {
                            selectedShip.GetComponent<ShipMovement>().IsSelected();
                        }
                        selectedShip = hit.collider.gameObject;
                    }
                }
            }

        }

    }
}
