using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Photon.Pun;

[RequireComponent(typeof(AudioSource))]
public class NarrationManager : MonoBehaviour
{
    [ShowNonSerializedField] private Charakter localChar;
    [SerializeField] private AudioClip bacteriaClip, birdClip, middleProjectionClip, deerClip, fishClip, machineClip, iceClip;

    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == null) return;

        localChar = (Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"];

        switch (localChar)
        {
            case Charakter.Methanosarcina:
                audioSource.clip = bacteriaClip;
                break;
            case Charakter.ArcticTern:
                audioSource.clip = birdClip;
                break;
            case Charakter.Observer:
                audioSource.clip = middleProjectionClip;
                break;
            case Charakter.Caribou:
                audioSource.clip = deerClip;
                break;
            case Charakter.ArcticCod:
                audioSource.clip = fishClip;
                break;
            case Charakter.Machine:
                audioSource.clip = machineClip;
                break;
            case Charakter.Ice:
                audioSource.clip = iceClip;
                break;
            default:
                Debug.LogWarning("No narration sound playing check Component");
                break;

        }
        audioSource.Play();
    }
}
