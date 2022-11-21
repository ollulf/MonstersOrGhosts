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
            case Charakter.Bacteria:
                audioSource.clip = bacteriaClip;
                break;
            case Charakter.Bird:
                audioSource.clip = birdClip;
                break;
            case Charakter.MiddleProjection:
                audioSource.clip = middleProjectionClip;
                break;
            case Charakter.Deer:
                audioSource.clip = deerClip;
                break;
            case Charakter.Fish:
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
