using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FishNet : MonoBehaviourPun
{
    private int fishLoss;

    private void Start()
    {
        fishLoss = FirstDataGive.FishNetLoss;
        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] != Charakter.Fish) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FishMovment>())
        {
                other.GetComponent<FishMovment>().ReducePopulation(fishLoss);
        }
    }
}
