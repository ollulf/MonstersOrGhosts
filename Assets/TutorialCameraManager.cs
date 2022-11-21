using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class TutorialCameraManager : MonoBehaviour
{
    [ShowNonSerializedField] private Charakter localChar;
    [SerializeField] private GameObject bacteriaScreen, birdScreen, cpScreen, deerScreen, fishScreen, machineScreen, iceScreen;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == null) return;

        localChar = (Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"];

        switch (localChar)
        {
            case Charakter.Methanosarcina:
                bacteriaScreen.SetActive(true);
                break;
            case Charakter.ArcticTern:
                birdScreen.SetActive(true);
                break;
            case Charakter.Observer:
                cpScreen.SetActive(true);
                break;
            case Charakter.Caribou:
                deerScreen.SetActive(true);
                break;
            case Charakter.ArcticCod:
                fishScreen.SetActive(true);
                break;
            case Charakter.Machine:
                machineScreen.SetActive(true);
                break;
            case Charakter.Ice:
                iceScreen.SetActive(true);
                break;
            default:
                Debug.LogWarning("No tutorial screen enabled, check component");
                break;

        }
    }
}
