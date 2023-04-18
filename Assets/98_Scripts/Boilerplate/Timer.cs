using UnityEngine;

public class Timer
{
    private float currentTime, startTime;

    private bool backwards;

    public float CurrentTime { get => currentTime;}

    public Timer(float time, bool backward)
    {
        if (backward)
        {
            startTime = time;
            currentTime = time;
            backwards = backward;
        }
        else
        {
            startTime = time;
            currentTime = time;
            backwards = backward;
        }
    }

    public void SetNewTime(float newTime, bool backward)
    {
        if (backward)
        {
            startTime = newTime;
            currentTime = newTime;
            backwards = backward;
        }
        else
        {
            startTime = newTime;
            currentTime = newTime;
            backwards = backward;
        }
    }

    public void Tick()
    {
        if (backwards)
        {
            currentTime -= Time.fixedDeltaTime;
        }
        else
        {
            currentTime += Time.fixedDeltaTime;
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