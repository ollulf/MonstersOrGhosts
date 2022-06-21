using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float currentTime, startTime;

    public float CurrentTime { get => currentTime;}

    public void SetStartTime(float time)
    {
        startTime = time;
        currentTime = time;
    }

    public void RunningTimer()
    {
        currentTime -= Time.deltaTime;
    }

    public void ResetTimer()
    {
        currentTime = startTime;
    }
}
