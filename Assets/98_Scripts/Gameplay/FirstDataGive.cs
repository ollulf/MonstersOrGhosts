using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstDataGive : Singleton<FirstDataGive>
{
    [Header("Start Population")]
    [SerializeField] private int birdStartPopulation, fishStartPopulation, deerStartPopulation;
    [Header("Starving per Seconds")]
    [SerializeField] private int birdPopulationLoss, deerPopulationLoss;
    [Header("RegularLoss")]
    [SerializeField] private int birdLoss, fishLoss, deerLoss;
    [Header("Realistic Increase per tick")]
    [SerializeField] private int birdPopulation, deerPopulation;
    [Header("Population increase per Food")]
    [SerializeField] private int fishPopulation;
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
    [Header("Money income per Ship in million per second")]
    [SerializeField] private int income;
    [Header("Income per environmental building in million per second")]
    [SerializeField] private float environmentalIncome;
    [Header("Passiv income in million per second")]
    [SerializeField] private float passiveIncome;
    [Header("Ship cost in million")]
    [SerializeField] private int shipCost;
    [Header("Environmental cost in million")]
    [SerializeField] private int enviCost;
    [Header("Building spot spawn in sec")]
    [SerializeField] private int buildingSpotSpawn;
    [Header("Produced CO2 per InduShip per second")]
    [SerializeField] private int induShipCo2;
    [Header("Passive CO2 per seconds")]
    [SerializeField] private int passiveCo2;
    [Header("Invest cost multi for Industrie")]
    [SerializeField] private float induShipMulti;
    [Header("Invest cost multi for Eco")]
    [SerializeField] private float ecoShipMulti;
    [Header("CO2 for 1°C")]
    [SerializeField] private float cO2NeededFor1C;

    public static int BirdStartPopulation { get => Instance.birdStartPopulation; }
    public static int FishStartPopulation { get => Instance.fishStartPopulation; }
    public static int DeerStartPopulation { get => Instance.deerStartPopulation; }
    public static float DeerCompress { get => Instance.deerCompress; }
    public static int BirdPopulation { get => Instance.birdPopulation; }
    public static int FishPopulation { get => Instance.fishPopulation;}
    public static int DeerPopulation { get => Instance.deerPopulation; }
    public static int ShipCost { get => Instance.shipCost; }
    public static float PassiveIncome { get => Instance.passiveIncome; }
    public static int Income { get => Instance.income; }
    public static int StartMoney { get => Instance.startMoney; }
    public static float InduShipMulti { get => Instance.induShipMulti; }
    public static float EcoShipMulti { get => Instance.ecoShipMulti; }
    public static int InduShipCo2 { get => Instance.induShipCo2; }
    public static int EnviCost { get => Instance.enviCost; }
    public static float EnvironmentalIncome { get => Instance.environmentalIncome; }
    public static float CO2NeededFor1C { get => Instance.cO2NeededFor1C;}
    public static int PassiveCo2 { get => Instance.passiveCo2;}
}
