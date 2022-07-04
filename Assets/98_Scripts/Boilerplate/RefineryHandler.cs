using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RefineryHandler : Singleton<RefineryHandler>
{
    [SerializeField] private List<GameObject> refinery;

    [SerializeField] private float chanceOfOilSpawning, timerforSpawn;
    private Timer timer;
    private GameObject oilField;
    void Start()
    {
        timer = new Timer();
        timer.SetStartTime(timerforSpawn);
    }

    void Update()
    {
        timer.RunningTimer();
        if(timer.CurrentTime <= 0)
        {
            Destroy(oilField);
            oilField = PhotonNetwork.Instantiate("FishGame/Oil", new Vector3(refinery[0].transform.position.x + Random.Range(-5f, 5f), Random.Range(0f, 8f), refinery[0].transform.position.z + Random.Range(-5f, 5f)), Quaternion.identity);
            timer.ResetTimer();
        }
    }
}
