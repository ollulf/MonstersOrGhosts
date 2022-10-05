using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FoodField : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FishMovment>())
        {
            other.GetComponent<FishMovment>().FeedFish(FirstDataGive.FishPopulation);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
