using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilField : MonoBehaviour
{
    [SerializeField] private int poisenPerTimer;
    [SerializeField] private float setTimer;
    private Timer timer;

    private void Start()
    {
        timer = new Timer();
        timer.SetStartTime(setTimer, true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<FishMovment>())
        {
            timer.Tick();
            if (timer.CurrentTime <= 0)
            {
                other.GetComponent<FishMovment>().PoisenFish(poisenPerTimer);

                timer.ResetTimer();
            }
        }
    }
}
