using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BirdFood : MonoBehaviourPun
{
    public int foodAmount = 15;
    private SphereCollider clickCollider;

    void Start()
    {
        clickCollider = GetComponent<SphereCollider>();
    }


    internal void DestroySelf()
    {
        base.photonView.RequestOwnership();
        PhotonNetwork.Instantiate("BirdGame/CollectEffect", transform.position, transform.rotation);
        PhotonNetwork.Destroy(gameObject);
    }
}
