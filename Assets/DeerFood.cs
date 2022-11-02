using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DeerFood : MonoBehaviour
{
    public float minFoodCapacity = 500, maxFoodCapacity = 1000;
    public float moveDownIntensity = 1;

    public float defaultRespawnTime = 60;

    [SerializeField] GameObject foodVisuals;

    [ShowNonSerializedField] private float foodCapacity;
    [ShowNonSerializedField] private float respawnTime;

    private Vector3 startPosition;
    private float tempfoodCapacity;
    private float currentTempIncrease;

    private Timer timer;


    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        foodCapacity = Random.Range(minFoodCapacity, maxFoodCapacity);
        tempfoodCapacity = foodCapacity;
        timer = new Timer();
    }

    public void ReduceFoodCapacity(float amount)
    {
        foodCapacity -= amount;

        if (foodCapacity <= 0)
        {
            timer.SetStartTime(60/ 1 + currentTempIncrease, true);
            foodCapacity = 0;
        }
    }

    void Update()
    {
        GetCurrentTemp();

        timer.Tick();

        if(timer.CurrentTime < 0)
        {
            foodCapacity = Random.Range(minFoodCapacity, maxFoodCapacity);
            timer.SetStartTime(60 / 1 + currentTempIncrease, true);
            timer.ResetTimer();
        }

        if(tempfoodCapacity != foodCapacity)
        {
            foodVisuals.transform.position = new Vector3 (startPosition.x, startPosition.y - ((1- (foodCapacity+1)/maxFoodCapacity)) * moveDownIntensity, startPosition.z);
        }
    }

    private void GetCurrentTemp()
    {
        currentTempIncrease = TempretureHandler.Tempreture;
    }
}
