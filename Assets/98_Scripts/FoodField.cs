using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FoodField : MonoBehaviourPun
{
    [SerializeField] private GameObject particleEffect, soundEffect;

    private void Start()
    {
        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] != Charakter.ArcticCod) Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FishMovment>())
        {
            other.GetComponent<FishMovment>().FeedFish(FirstDataGive.FishPopulation);
            
            Instantiate(particleEffect, this.transform);
            Instantiate(soundEffect, this.transform);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
