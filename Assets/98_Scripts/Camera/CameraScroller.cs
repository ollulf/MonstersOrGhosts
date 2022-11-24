using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;
using System;

public class CameraScroller : MonoBehaviourPun
{
    [SerializeField] Transform scientificPosition, farPosition, closePosition, animalSight;
    [SerializeField] AnimationCurve acellerationCurve;
    [SerializeField] float speed = 1f, fishFog, overviewFog;

    [SerializeField] private AudioSource characterAmbient, arcticAmbient, underwaterAmbient, enterWaterSound, exitWaterSound;
    [SerializeField] private AudioClip characterAmbientClip, arcticAmbientClip, underwaterAmbientClip, characterAmbientVariantClip, arcticAmbientVariantClip, underwaterAmbientVariantClip; 
    [ShowNonSerializedField] private bool isCameraUnderwater, camerChange;

    [ShowNonSerializedField] private float distance, currentDistance = 1;

    [SerializeField] private LayerMask mask;

    private GameObject selectedShip;
    private int index, lastIndex;
    private Vector3 fakeVector;
    private Quaternion fakeAngle;
    [ShowNonSerializedField] private bool fin, materialChange;

    [SerializeField] private GameObject scientificCamera, mainCamera, scientificButton;

    [SerializeField] private GameObject firstPersonVolume;

    private void Awake()
    {
        index = 1;
        lastIndex = index;
        IndexChecker();
        fin = false;
        materialChange = false;
        if (!mainCamera.GetComponent<PhotonView>().IsMine)
        {
            Destroy(scientificCamera);
            Destroy(mainCamera);
            Destroy(gameObject);
        }

        camerChange = false;
    }

    void Start()
    {
        mainCamera.transform.position = closePosition.position;
        distance = Vector3.Distance(farPosition.position, closePosition.position);
        RenderSettings.fogDensity = overviewFog;

        if (!(gameObject.transform.position.y < 0))
        {
            arcticAmbient.Play();
        }
        else
        {
            underwaterAmbient.Play();
        }
        if (characterAmbient.clip) characterAmbient.Play();
    }

    public void SetFarPosition(Transform newFarPosition, Transform newScientificPosition)
    {
        farPosition = newFarPosition;
        scientificPosition = newScientificPosition;
    }

    public void SetIndex(int newInt)
    {
        index = newInt;
    }

    // Update is called once per frame
    void Update()
    {
        if (camerChange == false)
        {
            if (selectedShip == null && (Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Machine)
            {
                SelectStartShip();
            }
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

            if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Machine)
            {
                MouseCheck();
            }
        }
    }

    private void LateUpdate()
    {
        //Debug.LogError(camerChange);
        if (camerChange == false)
        {
            IndexChecker();
        }
        else
        {
            Science();
        }
    }

    private void UpdateAmbientSound()
    {
        bool tempUnderWater = isCameraUnderwater;
        isCameraUnderwater = (mainCamera.transform.position.y < 0);

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

    private void TogglePerceptionSounds()
    {
        if (lastIndex != index)
        {
            if (underwaterAmbient.clip) 
            {
                if (index == 0) underwaterAmbient.clip = underwaterAmbientVariantClip;
                else if (index == 1) underwaterAmbient.clip = underwaterAmbientClip;
                underwaterAmbient.Play();
            }

            if (arcticAmbient.clip)
            {
                if (index == 0) arcticAmbient.clip = arcticAmbientVariantClip;
                else if (index == 1) arcticAmbient.clip = arcticAmbientClip;
                arcticAmbient.Play();
            }

            if (characterAmbient.clip) 
            { 
                if (index == 0) characterAmbient.clip = characterAmbientVariantClip;
                else if (index == 1) characterAmbient.clip = characterAmbientClip;
                 characterAmbient.Play();
            }

            lastIndex = index;
        }
        
    }

    private void IndexChecker()
    {

        switch (index)
        {
            case 0:
                {
                    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, animalSight.position, speed);
                    Quaternion rotation = Quaternion.Lerp(Quaternion.Euler(mainCamera.transform.eulerAngles), Quaternion.Euler(animalSight.eulerAngles), speed * Time.deltaTime);
                    mainCamera.transform.eulerAngles = rotation.eulerAngles;
                    firstPersonVolume.SetActive(true);
                    TogglePerceptionSounds();
                    if (!materialChange)
                    {
                        MaterialChange();
                    }
                    break;
                }
            case 1:
                {
                    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, closePosition.position, speed);
                    Quaternion rotation = Quaternion.Lerp(Quaternion.Euler(mainCamera.transform.eulerAngles), Quaternion.Euler(closePosition.eulerAngles), speed * Time.deltaTime);
                    mainCamera.transform.eulerAngles = rotation.eulerAngles;
                    fin = false;
                    scientificButton.SetActive(false);
                    if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Machine)
                    {
                        if (selectedShip != null && selectedShip.GetComponent<ShipMovement>().IsSelected)
                        {
                            selectedShip.GetComponent<ShipMovement>().SetIsSelected();
                        }
                    }
                    if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.ArcticCod)
                    {
                        RenderSettings.fogDensity = fishFog;
                    }
                    firstPersonVolume.SetActive(false);
                    TogglePerceptionSounds();
                    if (materialChange)
                    {
                        MaterialChange();
                    }

                    break;
                }
            case 2:
                {
                    if (!fin)
                    {
                        fin = true;
                        scientificButton.SetActive(true);
                        fakeVector = closePosition.position;
                        fakeAngle = Quaternion.Euler(closePosition.eulerAngles);
                    }

                    fakeVector = Vector3.Lerp(fakeVector, farPosition.localPosition, 1 * Time.deltaTime);
                    fakeAngle = Quaternion.Lerp(fakeAngle, Quaternion.Euler(farPosition.eulerAngles), speed * Time.deltaTime);

                    mainCamera.transform.position = fakeVector;
                    mainCamera.transform.eulerAngles = fakeAngle.eulerAngles;
                    RenderSettings.fogDensity = overviewFog;
                    if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Machine)
                    {
                        if (selectedShip != null && !selectedShip.GetComponent<ShipMovement>().IsSelected)
                        {
                            selectedShip.GetComponent<ShipMovement>().SetIsSelected();
                        }
                    }
                    firstPersonVolume.SetActive(false);
                    break;
                }
        }
    }

    private void Science()
    {
        scientificCamera.transform.position = scientificPosition.position;
        Quaternion rotation = Quaternion.Euler(scientificPosition.eulerAngles);
        scientificCamera.transform.eulerAngles = rotation.eulerAngles;
    }

    private void MaterialChange()
    {
        if(!materialChange)
        {
            MaterialHandler.StartEvent();
            materialChange = true;
        }
        else
        {
            MaterialHandler.StartDefaultEvent();
            materialChange = false;
        }
    }

    private void MouseCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "Ship")
                {
                    closePosition = hit.collider.transform.GetChild(1);
                    speed = hit.collider.GetComponent<ShipMovement>().MovementSpeed;
                    hit.collider.GetComponent<ShipMovement>().SetIsSelected();
                    if (selectedShip != null && selectedShip.GetComponent<ShipMovement>().IsSelected)
                    {
                        selectedShip.GetComponent<ShipMovement>().SetIsSelected();
                    }
                    selectedShip = hit.collider.gameObject;
                }
            }
        }
    }

    private void SelectStartShip()
    {
        selectedShip = ShipHandler.StartShip();
        animalSight = selectedShip.transform.GetChild(1);
        closePosition = selectedShip.transform.GetChild(1);
        speed = selectedShip.GetComponent<ShipMovement>().MovementSpeed;
        selectedShip.GetComponent<ShipMovement>().SetIsSelected();
    }

    public void ChangeCameraOnClick()
    {
        camerChange = !camerChange;
        mainCamera.SetActive(!camerChange);
        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Machine)
        {
            transform.parent.gameObject.GetComponent<MachineController>().ChangeButtonActive(!camerChange);
        }
            scientificCamera.SetActive(camerChange);
        scientificPosition.GetComponent<CameraMovement>().SciencesButton.SetActive(camerChange);
    }

}
