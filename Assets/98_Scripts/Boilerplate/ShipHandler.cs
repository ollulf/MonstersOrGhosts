using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ShipHandler : Singleton<ShipHandler>
{
    private List<GameObject> ship;

    private float money, passiveIncome, moneyPerShip, shipCost, enviCost, enviIncome;

    [SerializeField] private List<GameObject> prefab;
    [SerializeField] private NetSpawner netSpawner;
    [SerializeField] private FishTrawlerSpawn fishTrawlerSpawn;
    [SerializeField] private OilrigSpawner oilrigSpawner;
    [SerializeField] private float timerForMoney, indexTime;
    [SerializeField] private int amountOfNPCShips, amountOfEnvi;
    private Timer timer;
    private int carbonProduced;
    private AudioSource audioSource;

    private PhotonView photonView;

    [SerializeField] private TextMeshProUGUI carbonIncreaseText, carbonProduceText;

    public static float Money { get => Instance.money; }
    public static float ShipCost { get => Instance.shipCost; }
    public static List<GameObject> Ship { get => Instance.ship; }
    public static float EnviCost { get => Instance.enviCost; }
    public static int CarbonProduced { get => Instance.carbonProduced;}

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ship = new List<GameObject>();
        timer = new Timer(timerForMoney, true);
        money = FirstDataGive.StartMoney;
        moneyPerShip = FirstDataGive.Income;
        shipCost = FirstDataGive.ShipCost;
        enviCost = FirstDataGive.EnviCost;
        passiveIncome = FirstDataGive.PassiveIncome;
        enviIncome = FirstDataGive.EnvironmentalIncome;
        amountOfEnvi = 0;
        carbonProduced = 0;
        indexTime = 0;
        photonView = GetComponent<PhotonView>();
        SpawnNPCShips();
    }

    public static void AddShip(GameObject newShip)
    {
        if (!Instance.ship.Contains(newShip))
        {
            Instance.ship.Add(newShip);
        }
    }

    public static void SetMoney(float cost) => Instance.money -= cost;

    public static float Income() => FirstDataGive.PassiveIncome + Instance.moneyPerShip * Ship.Count + Instance.enviIncome * Instance.amountOfEnvi;

    public static void RiseShipCost()
    {
        Instance.netSpawner.ActivateNet();
        Instance.fishTrawlerSpawn.ActivateNet();
        Instance.oilrigSpawner.ActivateNet();
        Instance.shipCost += Mathf.RoundToInt(ShipCost * FirstDataGive.InduShipMulti);
    }

    private void RiseEnviCost() => Instance.enviCost += Mathf.RoundToInt(enviCost * FirstDataGive.EcoShipMulti);

    private void SpawnNPCShips()
    {
        for (int i = 0; i < amountOfNPCShips; i++)
        {
            WayPointPlacingSystem wayPoint = WayPointHandler.WayPoints[Random.Range(0, WayPointHandler.WayPoints.Count)];

            GameObject ship = PhotonNetwork.Instantiate("MachineGame/" + prefab[Random.Range(0, prefab.Count)].name, wayPoint.GetStartPoint().position, Quaternion.identity);
            ship.TryGetComponent<ShipMovement>(out ShipMovement shipMovement); 
            shipMovement.GetWayPoint(wayPoint);
            shipMovement.RemoveFromList();
            AddShip(ship);
        }
    }

    public static void EnviOption()
    {
        Instance.netSpawner.DeactivateNet();
        Instance.fishTrawlerSpawn.DeactivateNet();
        Instance.oilrigSpawner.DeactivateNet();
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
            photonView.RPC("ShowAllData", RpcTarget.All, CarbonIncreasePerSecond(), TotalCarbonProduced());
        }
        else if (photonView.Owner.IsMasterClient)
        {
            photonView.RPC("ShowAllData", RpcTarget.All, CarbonIncreasePerSecond(), TotalCarbonProduced());
        }
    }

    [PunRPC]
    private void ShowAllData(int carbonIncreasePerSec, int totalCarbonProd)
    {
        carbonIncreaseText.text = carbonIncreasePerSec.ToString();
        carbonProduceText.text = totalCarbonProd.ToString();
    }

    [PunRPC]
    private void AllAddShip(GameObject newShip)
    {
        if (!ship.Contains(newShip))
        {
            ship.Add(newShip);
        }
    }

    public static int CarbonIncreasePerSecond() => Ship.Count * FirstDataGive.InduShipCo2 + FirstDataGive.PassiveCo2;

    public static int TotalCarbonProduced()
    {
        Instance.indexTime += Time.deltaTime;
        if (Instance.indexTime >= 1)
        {
            Instance.carbonProduced += CarbonIncreasePerSecond();
            TempretureHandler.AddCO2(CarbonIncreasePerSecond());
            Instance.indexTime = 0;
        }

        return Instance.carbonProduced;
    }

    private void PlayIncomeSound()
    {
        audioSource.pitch = Random.Range(0.8f, 1);
        audioSource.Play();
    }

    public static GameObject StartShip() => Ship[Random.Range(0, Ship.Count)];

}