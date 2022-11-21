using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class TutorialScreens : MonoBehaviour
{
    [ShowNonSerializedField] private Charakter localChar;
    [SerializeField] private GameObject bacteriaScreen, birdScreen, cpScreen, deerScreen, fishScreen, machineScreen, iceScreen;

    private void OnEnable()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == null) return;

        localChar = (Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"];

        switch (localChar)
        {
            case Charakter.Bacteria:
                bacteriaScreen.SetActive(true);
                break;
            case Charakter.Bird:
                birdScreen.SetActive(true);
                break;
            case Charakter.MiddleProjection:
                cpScreen.SetActive(true);
                break;
            case Charakter.Deer:
                deerScreen.SetActive(true);
                break;
            case Charakter.Fish:
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
