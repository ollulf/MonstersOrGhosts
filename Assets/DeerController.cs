using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using System;
using Photon.Pun;

public class DeerController : MonoBehaviourPun
{
    public float moveAmount = 0.1f;

    [ShowNonSerializedField] private bool isMoving = false, isInFoodArea = false;

    private float co2CompressedTotal;

    public float co2Compressed;
    public int population;
    public float hunger = 0;
    public float hungerIncrease = 5;
    public float hungerDecrease = 15;
    public float hungerValueUntilStarving = 100;
    public float eatAmount = 50;
    public int populationGrowth;
    public float timeUntilTick = 1f;

    [SerializeField] private TextMeshProUGUI populationTF, co2TF, co2SecTF, hungerTF;

    [ShowNonSerializedField] private DeerFood currentFoodSource;
    private Timer timer;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float turnspeed, fishPrefTurnSpeed;
    [SerializeField] private Transform deerPref;
    [SerializeField] private Animator anim;
    private Vector2 movement;
    private Rigidbody rigidbody;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        population = FirstDataGive.DeerStartPopulation;
        co2Compressed = FirstDataGive.DeerCompress;
        populationGrowth = FirstDataGive.DeerPopulation;
        timer = new Timer();
        timer.SetStartTime(timeUntilTick, true);
        DeerHandler.SetTarget(gameObject);
    }
    private void FixedUpdate()
    {
        if (base.photonView.IsMine)
        {
            if (movement.y < 0 || movement.y > 0)
            {
                isMoving = true;
                anim.SetBool("IsWalking", true);
            }
            else
            {
                isMoving = false;
                anim.SetBool("IsWalking", false);
            }

            MovePlayer();
            TurnDeer();
            TurnDeerPref();
        }

        population--;

        if (hunger < hungerValueUntilStarving)
            population += populationGrowth;

        if (isMoving)
        {
            co2CompressedTotal += co2Compressed;
            TempretureHandler.AddCompressedCO2(co2Compressed);
        }
    }

    private void MovePlayer()
    {
        rigidbody.AddRelativeForce(Vector3.forward * movement.y * movementSpeed);
    }

    private void TurnDeer()
    {
        transform.Rotate(0, movement.x * turnspeed, 0);
    }

    private void TurnDeerPref()
    {
        Quaternion tempEulerAngles = Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);


        Quaternion rotation = Quaternion.Lerp(Quaternion.Euler(tempEulerAngles.eulerAngles), Quaternion.Euler(tempEulerAngles.eulerAngles.x, tempEulerAngles.eulerAngles.y - movement.x * 20, tempEulerAngles.eulerAngles.z), Time.deltaTime * fishPrefTurnSpeed);
        deerPref.transform.eulerAngles = rotation.eulerAngles;
    }


    void Update()
    {
        timer.Tick();
        CheckTimer();

        if (base.photonView.IsMine)
        {
            GetAxis();
        }
    }

    private void GetAxis()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
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
        co2TF.text = co2CompressedTotal.ToString();
        if (isMoving) co2SecTF.text = UnityEngine.Random.Range(45, 55).ToString();


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
        else if (hunger >= 0)
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
        {
            isInFoodArea = true;
            anim.SetBool("IsGrazing", true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        currentFoodSource = null;
        isInFoodArea = false;
        anim.SetBool("IsGrazing", false);
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
