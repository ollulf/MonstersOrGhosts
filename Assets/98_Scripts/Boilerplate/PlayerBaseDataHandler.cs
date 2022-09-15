using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerBaseDataHandler : Singleton<PlayerBaseDataHandler>
{
    [ShowNonSerializedField] private int deerPopulation, oilRigs, fisherBoats, birdFood;
    [ShowNonSerializedField] private float temperature;

    [ShowNonSerializedField] private FishMovment fishPopulation;
    [ShowNonSerializedField] private BirdController birdPopulation;

    public static FishMovment FishPopulation { get => Instance.fishPopulation;}
    public static BirdController BirdPopulation { get => Instance.birdPopulation;}
    public static int DeerPopulation { get => Instance.deerPopulation;}
    public static int OilRigs { get => Instance.oilRigs;}
    public static int FisherBoats { get => Instance.fisherBoats;}
    public static float Temperature { get => Instance.temperature;}

    public static void SetFish(FishMovment newFish)
    {
        Instance.fishPopulation = newFish;
    }

    public static void SetBird(BirdController newBird)
    {
        Instance.birdPopulation = newBird;
    }


    public static int GetFishPopulation()
    {
        return FishPopulation.Population;
    }

    public static int GetBirdPopulation()
    {
        return BirdPopulation.BirdPopulation;
    }

    public static int GetOilRigs()
    {
        return ShipHandler.Ship.Count;
    }

    public static void RaiseBirdFood(int newFood)
    {
        Instance.birdFood += newFood;
    }

    public static void ReduceBirdFood(int newFood)
    {
        Instance.birdFood -= newFood;
    }

    public static int GetBirdFood()
    {
        return Instance.birdFood;
    }
} 