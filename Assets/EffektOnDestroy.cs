using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EffektOnDestroy : MonoBehaviour, IOnPhotonViewPreNetDestroy
{

    public GameObject objectEffect, soundEffect;

    public void OnPreNetDestroy(PhotonView rootView)
    {
        Debug.LogError("Help I am getting Destroyed");
        if(objectEffect) Instantiate(objectEffect, gameObject.transform);
        if(soundEffect) Instantiate(soundEffect, gameObject.transform);
    }
}
