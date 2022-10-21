using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishNet : MonoBehaviour
{
    private int fishLoss;

    private void Start()
    {
        fishLoss = FirstDataGive.FishNetLoss;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FishMovment>())
        {
                other.GetComponent<FishMovment>().ReducePopulation(fishLoss);
        }
    }
}
