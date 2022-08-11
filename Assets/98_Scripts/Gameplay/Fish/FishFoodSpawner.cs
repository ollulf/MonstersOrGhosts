using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FishFoodSpawner : MonoBehaviourPun
{
    [SerializeField] float spawningTime;

    private List<Transform> fishFoodSpawner;
    private Timer timer;

    void Start()
    {
        timer = new Timer();
        timer.SetStartTime(spawningTime, true);
        fishFoodSpawner = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            fishFoodSpawner.Add(transform.GetChild(i));
        }
    }

    void Update()
    {
        timer.Tick();
        if (timer.CurrentTime <= 0)
        {
            SpawningFood();
            timer.ResetTimer();
        }
    }

    private void SpawningFood()
    {
        foreach (Transform child in fishFoodSpawner)
        {
            if (child.childCount == 0)
            {
                GameObject food = PhotonNetwork.Instantiate("FishGame/FishFood", child.transform.position, Quaternion.identity);
                food.transform.parent = child.transform;
                return;
            }
        }
    }
}
