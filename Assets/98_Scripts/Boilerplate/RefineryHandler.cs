using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class RefineryHandler : Singleton<RefineryHandler>
{
    private List<GameObject> refinery;

    [SerializeField] private int moneyPerRefinery, refineryCost;

    private int money;

    [SerializeField] private float chanceOfOilSpawning, timerforSpawn;
    private Timer timer;

    public static int Money { get => Instance.money; }
    public static int RefineryCost { get => Instance.refineryCost; }
    public static List<GameObject> Refinery { get => Instance.refinery; }

    //private GameObject oilField;

    public override void Awake()
    {
        base.Awake();
        refinery = new List<GameObject>();
        timer = new Timer();
        timer.SetStartTime(timerforSpawn, true);

    }

    public static void AddRefineries(GameObject newRefinery)
    {
        Instance.refinery.Add(newRefinery);
    }

    public static void SetMoney()
    {
        Instance.money -= Instance.refineryCost;
    }

    void Update()
    {
        timer.Tick();
        if (timer.CurrentTime <= 0)
        {
            if (refinery.Count > 0)
            {
                for (int i = 0; i < refinery.Count; i++)
                {
                    //Destroy(oilField);
                    GameObject oilField = PhotonNetwork.Instantiate("FishGame/Oil", new Vector3(refinery[i].transform.position.x + Random.Range(-5f, 5f), Random.Range(0f, 8f), refinery[i].transform.position.z + Random.Range(-5f, 5f)), Quaternion.identity);
                }
                money += moneyPerRefinery * refinery.Count;

            }
            timer.ResetTimer();
        }
    }
}
