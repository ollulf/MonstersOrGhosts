using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using System;

public class DeerController : MonoBehaviour
{
    public float moveAmount = 0.1f;

    [ShowNonSerializedField] private bool isMoving = false, isInFoodArea = false;

    public float co2Compressed;
    public float population;
    public float hunger = 0;
    public float hungerIncrease = 5;
    public float hungerDecrease = 15;
    public float hungerValueUntilStarving = 100;
    public float eatAmount = 50;
    public float populationGrowth;
    public float timeUntilTick = 1f;

    [SerializeField] private TextMeshProUGUI populationTF, co2TF,co2SecTF, hungerTF;

    [ShowNonSerializedField] private DeerFood currentFoodSource;
    private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        population = FirstDataGive.DeerStartPopulation;
        co2Compressed = FirstDataGive.DeerCompress;
        populationGrowth = FirstDataGive.DeerPopulation;
        timer = new Timer();
        timer.SetStartTime(timeUntilTick, true);
        DeerHandler.SetTarget(gameObject);
    }
    private void FixedUpdate()
    {
        population--;

        if (hunger < hungerValueUntilStarving)
            population += populationGrowth;

        if (isMoving)
            co2Compressed++;
    }
    void Update()
    {
        timer.Tick();
        CheckTimer();

        if (Input.GetKey(KeyCode.D))
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
        if (!Input.anyKey)
        {
            isMoving = false;
        }
    }

    private void TickUpdate()
    {
        UpdateUI();

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

            if (hunger > hungerValueUntilStarving)
                hunger = hungerValueUntilStarving;
        }
    }

    private void UpdateUI()
    {
        populationTF.text = population.ToString();
        hungerTF.text = HungerToState(hunger).ToString();
        co2TF.text = co2Compressed.ToString();
        if (isMoving) co2SecTF.text = UnityEngine.Random.Range(45,55).ToString();


    }

    private HungerState HungerToState(float hunger)
    {
        if (hunger >= hungerValueUntilStarving)
            return HungerState.starving;
        else if (hunger > hungerValueUntilStarving / 4 * 3)
            return HungerState.hungry;
        else if (hunger > hungerValueUntilStarving / 2)
            return HungerState.ok;
        else if (hunger > hungerValueUntilStarving / 4)
            return HungerState.satisfied;
        else if (hunger > 0)
            return HungerState.full;
        else
        {
            Debug.LogWarning("Hunger value out of bounds");
            return HungerState.error;
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

public enum HungerState
{
    starving,
    hungry,
    ok,
    satisfied,
    full,
    error
}
