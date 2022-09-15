using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class ShipHandler : Singleton<ShipHandler>
{
    private List<GameObject> ship;

    private int moneyPerShip, shipCost;

    private float money, passiveIncome;

    [SerializeField] private float chanceOfOilSpawning, timerforSpawn;
    private Timer timer;

    public static float Money { get => Instance.money; }
    public static int ShipCost { get => Instance.shipCost; }
    public static List<GameObject> Ship { get => Instance.ship; }


    public void Start()
    {
        ship = new List<GameObject>();
        timer = new Timer();
        timer.SetStartTime(timerforSpawn, true);
        Debug.LogError(FirstDataGive.StartMoney);
        Debug.LogError(FirstDataGive.Income);
        Debug.LogError(FirstDataGive.ShipCost);
        Debug.LogError(FirstDataGive.PassiveIncome);
        money = FirstDataGive.StartMoney;
        moneyPerShip = FirstDataGive.Income;
        shipCost = FirstDataGive.ShipCost;
        passiveIncome = FirstDataGive.PassiveIncome;
    }

    public static void AddShip(GameObject newShip)
    {
        Instance.ship.Add(newShip);
    }

    public static void SetMoney()
    {
        Instance.money -= Instance.shipCost;
    }

    public static float Income()
    {
        return Instance.passiveIncome + Instance.moneyPerShip * Ship.Count;
    }

    void Update()
    {
        timer.Tick();
        if (timer.CurrentTime <= 0)
        {
            money += passiveIncome + moneyPerShip * ship.Count;


            timer.ResetTimer();
        }
    }
}
