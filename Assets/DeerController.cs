using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DeerController : MonoBehaviour
{
    public float moveAmount = 0.1f;

    [ShowNonSerializedField]private bool isMoving = false, isInFoodArea = false;

    public float co2Compressed = 0;
    public float population = 10000;
    public float hunger = 0;
    public float hungerIncrease = 5;
    public float hungerDecrease = 15;
    public float hungerValueUntilStarving = 100;
    public float eatAmount = 50;
    public float populationGrowth = 50;
    public float timeUntilTick = 1f;

    [ShowNonSerializedField]private DeerFood currentFoodSource;
    private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();
        timer.SetStartTime(timeUntilTick);
    }
    private void FixedUpdate()
    {
        population--;

        if(hunger < hungerValueUntilStarving)
            population += populationGrowth;

        if (isMoving)
            co2Compressed++;
    }
    void Update()
    {
        timer.Tick();
        CheckTimer();
       
        if(Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(0, 0, moveAmount);
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(0, 0, -moveAmount);
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(-moveAmount, 0, 0);
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(moveAmount, 0, 0);
            isMoving = true;
        }
        if(!Input.anyKey)
        {
            isMoving = false;
        }
    }

    private void TickUpdate()
    {
        if (!currentFoodSource) 
        {
            currentFoodSource = null;
            isInFoodArea = false;
        }

        if (isInFoodArea)
        {
            currentFoodSource.ReduceFoodCapacity(eatAmount);

            population += populationGrowth;
            hunger -= hungerDecrease;
            if (hunger < 0)
                hunger = 0;
        }

        else
        {
            hunger += hungerIncrease;

            if(hunger > hungerValueUntilStarving)
                hunger = hungerValueUntilStarving;
        }
    }

    private void CheckTimer()
    {
        if (timer.CurrentTime <= 0)
        {
            timer.ResetTimer();
            TickUpdate();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "DeerFood")
        {
            currentFoodSource = collision.gameObject.GetComponent<DeerFood>();
        }
    }
    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "DeerFood" && currentFoodSource)
            isInFoodArea = true;
    }

    private void OnTriggerExit(Collider collision)
    {
            currentFoodSource = null;
            isInFoodArea = false;
    }
}
