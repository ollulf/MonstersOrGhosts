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

    [SerializeField] private AudioSource underwaterAmbient, enterWaterSound, exitWaterSound;
    [ShowNonSerializedField]private bool isCameraUnderwater;

    [ShowNonSerializedField] private float distance, currentDistance = 1;

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

        underwaterAmbient.Play();
        //if (!(gameObject.transform.position.y < 12))
            //underwaterAmbient.Pause();
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



        IndexChecker();
    }

    private void UpdateAmbientSound()
    {
        bool tempUnderWater = isCameraUnderwater;
        isCameraUnderwater = (gameObject.transform.position.y < 12);

        if (isCameraUnderwater != tempUnderWater)
        {
            if (isCameraUnderwater)
            {
                Debug.Log("Enters Water");
                enterWaterSound.Play();
                underwaterAmbient.UnPause();

            }
            if (!isCameraUnderwater)
            {
                Debug.Log("Exits Water");
                exitWaterSound.Play();
                underwaterAmbient.Pause();
            }
        }

    }

    private void IndexChecker()
    {
        switch (index)
        {
            case 0:
                {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, animalSight.position, speed * Time.deltaTime);
                    Vector3 rotation = new Vector3(
                            Mathf.LerpAngle(gameObject.transform.eulerAngles.x, animalSight.eulerAngles.x, speed * Time.deltaTime),
                            Mathf.LerpAngle(gameObject.transform.eulerAngles.y, animalSight.eulerAngles.y, speed * Time.deltaTime),
                            Mathf.LerpAngle(gameObject.transform.eulerAngles.z, animalSight.eulerAngles.z, speed * Time.deltaTime));
                    gameObject.transform.eulerAngles = rotation;
                    break;
                }
            case 1:
                {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, closePosition.position, speed * Time.deltaTime);
                    Vector3 rotation = new Vector3(
                            Mathf.LerpAngle(gameObject.transform.eulerAngles.x, closePosition.eulerAngles.x, speed * Time.deltaTime),
                            Mathf.LerpAngle(gameObject.transform.eulerAngles.y, closePosition.eulerAngles.y, speed * Time.deltaTime),
                            Mathf.LerpAngle(gameObject.transform.eulerAngles.z, closePosition.eulerAngles.z, speed * Time.deltaTime));
                    gameObject.transform.eulerAngles = rotation;
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
                    fakeAngle.eulerAngles = Vector3.Lerp(fakeAngle.eulerAngles, farPosition.eulerAngles, speed * Time.deltaTime);

                    gameObject.transform.position = fakeVector;
                    gameObject.transform.eulerAngles = fakeAngle.eulerAngles;
                    break;
                }
        }
    }
}
