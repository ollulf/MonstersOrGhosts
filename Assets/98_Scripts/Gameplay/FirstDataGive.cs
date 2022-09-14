using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstDataGive : Singleton<FirstDataGive>
{
    [Header("Start Population")]
    [SerializeField] private int birdStartPopulation, fishStartPopulation, deerStartPopulation;
    [Header("Starving per Seconds")]
    [SerializeField] private int birdPopulationLoss, fishPopulationLoss, deerPopulationLoss;
    [Header("RegularLoss")]
    [SerializeField] private int birdLoss, fishLoss, deerLoss;
    [Header("Realistic Increase per tick")]
    [SerializeField] private int birdPopulation, fishPopulation, deerPopulation;
    [Header("Spawnrate")]
    [SerializeField] private float birdFood, fishFood, DeerFood;
    [Header("Passive CO2 Production")]
    [SerializeField] private int bacteriaProduction;
    [Header("Produced CO2 after Minigame")]
    [SerializeField] private int bacteriaMinigame;
    [Header("Time of Minigame")]
    [SerializeField] private float minigameTime;
    [Header("Compressed CO2 tons per second")]
    [SerializeField] private float deerCompress;
    [Header("Minutes Playing")]
    [SerializeField] private float minutes;
    [Header("Start Money in Million")]
    [SerializeField] private int startMoney;
    [Header("Money income per factory in million per second")]
    [SerializeField] private int income;
    [Header("Income per environmental building in million per second")]
    [SerializeField] private float environmentalIncome;
    [Header("Passiv income in million per second")]
    [SerializeField] private float passiveIncome;
    [Header("Factory cost in million")]
    [SerializeField] private int factoryCost;
    [Header("Environmental cost in million")]
    [SerializeField] private int enviCost;
    [Header("Building spot spawn in sec")]
    [SerializeField] private int buildingSpotSpawn;
    [Header("Produced CO2 per factory per second")]
    [SerializeField] private int factoryCo2;
    [Header("Passive Compressed CO2 per seconds")]
    [SerializeField] private int passiveCompressedCo2;

    public static int BirdStartPopulation { get => Instance.birdStartPopulation;}
    public static int FishStartPopulation { get => Instance.fishStartPopulation;}
    public static int DeerStartPopulation { get => Instance.deerStartPopulation;}
    public static float DeerCompress { get => Instance.deerCompress;}
    public static int BirdPopulation { get => Instance.birdPopulation;}
    public int FishPopulation { get => fishPopulation; set => fishPopulation = value; }
    public static int DeerPopulation { get => Instance.deerPopulation;}
}