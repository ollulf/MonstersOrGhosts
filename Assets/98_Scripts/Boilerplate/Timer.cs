using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float currentTime, startTime;

    private bool backwards;

    public float CurrentTime { get => currentTime;}

    public void SetStartTime(float time, bool backward)
    {
        if (backward)
        {
            startTime = time;
            currentTime = time;
            backwards = backward;
        }
        else
        {
            startTime = 0;
            currentTime = 0;
            backwards = backward;
        }
    }

    public void Tick()
    {
        if (backwards)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }



    public void ResetTimer()
    {
        if (backwards)
        {
            currentTime = startTime;
        }
        else
        {
            currentTime = 0;
        }
    }
}
