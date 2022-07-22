using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ShowInterface : MonoBehaviour
{
    [ShowNonSerializedField]
    private int fishPopulation, birdPopulation, oilRigs, fishPopulationAfterBalance;


    // Update is called once per frame
    void Update()
    {
        if (PlayerBaseDataHandler.FishPopulation != null)
        {
            fishPopulation = PlayerBaseDataHandler.GetFishPopulation();
            fishPopulationAfterBalance = BalancingInterface.GetFishPopulation();
        }
        if (PlayerBaseDataHandler.BirdPopulation != null)
        {
            birdPopulation = PlayerBaseDataHandler.GetBirdPopulation();
        }
        oilRigs = PlayerBaseDataHandler.GetOilRigs();

    }
}
