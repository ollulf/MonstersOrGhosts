using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using Photon.Pun;

public class BirdController : MonoBehaviourPun
{
    public int amounOfFoodNeededForNest = 10;
    public int birdPopulationIncreaseAmount;

    public List<GameObject> birdRoute;

    [ShowNonSerializedField] private int birdFood = 0, birdPopulation;
    [SerializeField] public TextMeshProUGUI foodAmount, population;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float maxDistance = 0.25f;

    [Header("Soft Rotation")]
    [Tooltip("Zeit (Sek.) bis die Rotation weich auf Ziel einschwingt.")]
    [SerializeField, Range(0.01f, 1.0f)] private float rotationDampingTime = 0.25f;

    [Tooltip("Voller Speed, wenn Winkel <= dieser Schwelle (Grad).")]
    [SerializeField, Range(0f, 45f)] private float moveWhenAngleBelowDeg = 10f;

    [Tooltip("Bei großem Winkel wird die Laufgeschwindigkeit runtergeblendet. Ab diesem Winkel steht der Vogel.")]
    [SerializeField, Range(30f, 180f)] private float maxAngleForNoMove = 120f;

    [Header("Optionales Banking (Neigung beim Kurvenflug)")]
    [SerializeField] private bool enableBanking = true;
    [SerializeField, Range(0f, 45f)] private float maxBankDeg = 15f;
    [SerializeField, Range(0.01f, 0.6f)] private float bankDampingTime = 0.2f;
    [Tooltip("Faktor wie stark Yaw-Drehgeschwindigkeit in Roll übersetzt wird.")]
    [SerializeField] private float bankFromYawVelFactor = 0.06f;

    [Header("Input")]
    [SerializeField] private LayerMask layerMask;

    private Timer timer;

    // SmoothDamp-Werte (Grad/Sek.)
    private float _yawVel, _pitchVel, _rollVel;

    public int BirdPopulation { get => birdPopulation; }

    void Start()
    {
        timer = new Timer(0, false);
        birdRoute = new List<GameObject>(BirdRouteHandler.BirdRoute.GetWayPoints());

        birdPopulationIncreaseAmount = FirstDataGive.BirdPopulation;
        birdPopulation = FirstDataGive.BirdStartPopulation;
        PlayerBaseDataHandler.SetBird(this);
        CallInEndValues.SetBird(this);
        UpdateUI();

        // Falls exakt auf erstem Waypoint stehen, nimm direkt den nächsten
        if (birdRoute.Count > 0 && (transform.position - birdRoute[0].transform.position).sqrMagnitude < 0.0001f)
            RemoveFromList();
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected && photonView && !photonView.IsMine) return;
        CheckforClick();
    }

    private void FixedUpdate()
    {
        if (PhotonNetwork.IsConnected && photonView && !photonView.IsMine) return;

        timer.Tick();
        if (timer.CurrentTime >= 1f)
        {
            birdPopulation -= FirstDataGive.BirdLoss;
            if (birdPopulation < 0) birdPopulation = 0; // TODO: Death State
            UpdateUI();
            timer.ResetTimer();
        }

        if (birdRoute.Count > 0)
        {
            Move();
            if (CheckDistance())
                RemoveFromList();
        }
        else
        {
            var points = BirdRouteHandler.BirdRoute.GetWayPoints();
            if (points != null && points.Count > 0)
                birdRoute = new List<GameObject>(points);
        }
    }

    private void UpdateUI()
    {
        if (foodAmount) foodAmount.SetText(birdFood.ToString());
        if (population) population.SetText(birdPopulation.ToString());
    }

    private void CheckforClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (!Camera.main) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
        {
            // 6 = BirdFood
            if (hit.collider.gameObject.layer == 6)
            {
                var pv = hit.collider.GetComponent<PhotonView>();
                if (pv) pv.TransferOwnership(PhotonNetwork.LocalPlayer);

                var bf = hit.collider.GetComponent<BirdFood>();
                if (bf && TryAddToBirdFood(bf.FoodAmount))
                {
                    var child = hit.collider.GetComponentInChildren<BirdFood>();
                    if (child) child.DestroySelf();
                }
            }
            // 7 = BirdNest
            else if (hit.collider.gameObject.layer == 7)
            {
                if (TryBuildNest())
                {
                    var spot = hit.collider.GetComponentInChildren<BreedingSpot>();
                    if (spot) spot.DestroySelf();
                }
            }
        }
    }

    public bool TryAddToBirdFood(int amount)
    {
        birdFood += amount;
        UpdateUI();
        return true;
    }

    public bool TryBuildNest()
    {
        if (birdFood < amounOfFoodNeededForNest)
            return false;

        birdFood -= amounOfFoodNeededForNest;
        birdPopulation += birdPopulationIncreaseAmount;
        UpdateUI();
        return true;
    }

    private void Move()
    {
        var targetGo = birdRoute[0];
        if (!targetGo) { RemoveFromList(); return; }

        Vector3 targetPos = targetGo.transform.position;
        Vector3 to = targetPos - transform.position;
        float distSqr = to.sqrMagnitude;
        if (distSqr < 0.000001f) return;

        Vector3 dir = to.normalized;

        // Ziel-Yaw/Pitch aus Richtung bestimmen (robust ohne Gimbal-Probleme für typische Vogelbewegung)
        float targetYaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        float horizLen = Mathf.Max(0.0001f, new Vector2(dir.x, dir.z).magnitude);
        float targetPitch = -Mathf.Atan2(dir.y, horizLen) * Mathf.Rad2Deg;

        // Aktuelle Euler
        Vector3 e = transform.rotation.eulerAngles;

        // Weiche Winkel-Annäherung
        float yaw = Mathf.SmoothDampAngle(e.y, targetYaw, ref _yawVel, rotationDampingTime);
        float pitch = Mathf.SmoothDampAngle(e.x, targetPitch, ref _pitchVel, rotationDampingTime);

        // Optional: Banking aus Yaw-Drehgeschwindigkeit ableiten
        float roll = e.z;
        if (enableBanking)
        {
            float targetRoll = Mathf.Clamp(-_yawVel * bankFromYawVelFactor, -maxBankDeg, maxBankDeg);
            roll = Mathf.SmoothDampAngle(e.z, targetRoll, ref _rollVel, bankDampingTime);
        }
        else
        {
            // Zur Sicherheit Roll zurück zu 0 fahren, wenn Banking aus ist
            roll = Mathf.SmoothDampAngle(e.z, 0f, ref _rollVel, bankDampingTime);
        }

        transform.rotation = Quaternion.Euler(pitch, yaw, roll);

        // Bewegungs-Blend abhängig vom Ausrichtungswinkel (0..1)
        float angleToTarget = Quaternion.Angle(transform.rotation, Quaternion.Euler(targetPitch, targetYaw, roll));
        float moveBlend = Mathf.Clamp01(Mathf.InverseLerp(maxAngleForNoMove, moveWhenAngleBelowDeg, angleToTarget));

        float step = movementSpeed * moveBlend * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }

    public void RemoveFromList()
    {
        if (birdRoute.Count > 0) birdRoute.RemoveAt(0);
    }

    private bool CheckDistance()
    {
        var target = birdRoute[0];
        if (!target) return true;
        return Vector3.Distance(transform.position, target.transform.position) <= maxDistance;
    }
}
