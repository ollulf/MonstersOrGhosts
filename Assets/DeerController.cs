using UnityEngine;
using NaughtyAttributes;
using TMPro;
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
    private int popoluationLoss;

    [SerializeField] private TextMeshProUGUI populationTF, co2TF, co2SecTF, hungerTF;

    [ShowNonSerializedField] private DeerFood currentFoodSource;
    private Timer timer;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float turnspeed, fishPrefTurnSpeed;
    [SerializeField] private Transform deerPref;
    [SerializeField] private Animator anim;
    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        population = FirstDataGive.DeerStartPopulation;
        co2Compressed = FirstDataGive.DeerCompress;
        populationGrowth = FirstDataGive.DeerPopulation;
        popoluationLoss = FirstDataGive.DeerLoss;
        timer = new Timer(timeUntilTick, true);
        CallInEndValues.SetDeer(this);
        DeerHandler.SetTarget(gameObject);
    }
    private void FixedUpdate()
    {
        if (base.photonView.IsMine)
        {
            if (GetVertical() < 0 || GetVertical() > 0)
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


        if (isMoving)
        {
            co2CompressedTotal += co2Compressed;
            TempretureHandler.AddCompressedCO2(co2Compressed);
        }
    }

    private void MovePlayer() => rigidbody.AddRelativeForce(Vector3.forward * GetVertical() * movementSpeed);

    private void TurnDeer() => transform.Rotate(0, GetHorizontal() * turnspeed, 0);

    private void TurnDeerPref()
    {
        Quaternion tempEulerAngles = Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);

        Quaternion rotation = Quaternion.Lerp(Quaternion.Euler(tempEulerAngles.eulerAngles), Quaternion.Euler(tempEulerAngles.eulerAngles.x, tempEulerAngles.eulerAngles.y - GetHorizontal() * 20, tempEulerAngles.eulerAngles.z), Time.deltaTime * fishPrefTurnSpeed);
        deerPref.transform.eulerAngles = rotation.eulerAngles;
    }

    void Update()
    {
        timer.Tick();
        if (hunger < hungerValueUntilStarving && timer.CurrentTime <= 0)
        {
            population += populationGrowth;
            population -= popoluationLoss;
        }

        CheckTimer();
    }

    private float GetHorizontal() => Input.GetAxisRaw("Horizontal");

    private float GetVertical() => Input.GetAxisRaw("Vertical");

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

            if (hunger > hungerValueUntilStarving)
                hunger = hungerValueUntilStarving;
        }
        UpdateUI();
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