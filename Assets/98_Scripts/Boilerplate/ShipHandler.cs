using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;
using Photon.Pun;

public class ShipHandler : Singleton<ShipHandler>
{
    private List<GameObject> ship;

    private float money, passiveIncome, moneyPerShip, shipCost, enviCost, enviIncome;

    [SerializeField] private List<GameObject> prefab;
    [SerializeField] private NetSpawner netSpawner;
    [SerializeField] private float timerForMoney, indexTime;
    [SerializeField] private int amountOfNPCShips, amountOfEnvi;
    private Timer timer;
    private int carbonProduced;
    private AudioSource audioSource;

    public static float Money { get => Instance.money; }
    public static float ShipCost { get => Instance.shipCost; }
    public static List<GameObject> Ship { get => Instance.ship; }
    public static float EnviCost { get => Instance.enviCost;}

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ship = new List<GameObject>();
        timer = new Timer();
        timer.SetStartTime(timerForMoney, true);
        money = FirstDataGive.StartMoney;
        moneyPerShip = FirstDataGive.Income;
        shipCost = FirstDataGive.ShipCost;
        enviCost = FirstDataGive.EnviCost;
        passiveIncome = FirstDataGive.PassiveIncome;
        enviIncome = FirstDataGive.EnvironmentalIncome;
        amountOfEnvi = 0;
        carbonProduced = 0;
        indexTime = 0;
        SpawnNPCShips();
    }

    public static void AddShip(GameObject newShip)
    {
        Instance.ship.Add(newShip);
    }

    public static void SetMoney(float cost)
    {
        Instance.money -= cost;
    }

    public static float Income()
    {
        return FirstDataGive.PassiveIncome + Instance.moneyPerShip * Ship.Count + Instance.enviIncome * Instance.amountOfEnvi;
    }

    public static void RiseShipCost()
    {
        Instance.netSpawner.ActivateNet();
        Instance.shipCost += Mathf.RoundToInt(ShipCost * FirstDataGive.InduShipMulti);
    }

    private void RiseEnviCost()
    {
        Instance.enviCost += Mathf.RoundToInt(enviCost * FirstDataGive.EcoShipMulti);
    }

    public static int CarbonIncreasePerSecond()
    {
        return Ship.Count * FirstDataGive.InduShipCo2;
    }

    public static int TotalCarbonProduced()
    {
        Instance.indexTime += Time.deltaTime;
        if(Instance.indexTime >= 1)
        {
            Instance.carbonProduced += CarbonIncreasePerSecond();
        }

        return Instance.carbonProduced;
    }

    private void SpawnNPCShips()
    {
        for (int i = 0; i < amountOfNPCShips; i++)
        {
            WayPointPlacingSystem wayPoint = WayPointHandler.WayPoints[Random.Range(0, WayPointHandler.WayPoints.Count)];

            GameObject ship = PhotonNetwork.Instantiate("MachineGame/" + prefab[Random.Range(0, prefab.Count)].name, wayPoint.GetStartPoint().position, Quaternion.identity);
            ship.GetComponent<ShipMovement>().GetWayPoint(wayPoint);
            AddShip(ship);
        }
    }

    public static void EnviOption()
    {
        Instance.netSpawner.DeactivateNet();
        Destroy(Ship[0]);
        Instance.ship.Remove(Ship[0]);
        Instance.amountOfEnvi++;
        SetMoney(EnviCost);
        Instance.RiseEnviCost();
    }

    void Update()
    {
        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Machine)
        {
            timer.Tick();
            if (timer.CurrentTime <= 0)
            {
                money += Income();
                PlayIncomeSound();

                timer.ResetTimer();
            }
        }
    }

    private void PlayIncomeSound()
    {
        audioSource.pitch = Random.Range(0.8f , 1);
        audioSource.Play();
    }

    public static GameObject StartShip()
    {
        return Ship[Random.Range(0,Ship.Count)];
    }
}
