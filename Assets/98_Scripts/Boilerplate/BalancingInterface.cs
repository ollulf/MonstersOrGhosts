using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BalancingInterface : Singleton<BalancingInterface>
{
    [MinValue(0), MaxValue(1)]
    public float fishPopulationToBirdFood, fishPopulationToFishPopulation;

    public static int GetBirdfood()
    {
        return Mathf.RoundToInt(PlayerBaseDataHandler.GetFishPopulation() * Instance.fishPopulationToBirdFood);
    }

    public static int GetFishPopulation()
    {
        return Mathf.RoundToInt(PlayerBaseDataHandler.GetFishPopulation() * Instance.fishPopulationToFishPopulation);
    }
}
