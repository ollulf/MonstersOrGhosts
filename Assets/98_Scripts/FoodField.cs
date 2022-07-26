using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FoodField : MonoBehaviourPun
{
    [SerializeField] private int foodValue;
    [SerializeField] private int feeding;
    [SerializeField] private float setTimer;
    private Timer timer;

    public int FoodValue { get => foodValue; }

    private void Start()
    {
        timer = new Timer();
        timer.SetStartTime(setTimer);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<FishMovment>())
        {
            Debug.LogError(timer.CurrentTime);
            timer.Tick();
            if (timer.CurrentTime <= 0)
            {
                foodValue -= feeding;
                other.GetComponent<FishMovment>().FeedFish(feeding);
                if (foodValue <= 0)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
                timer.ResetTimer();
            }
        }
    }
}
