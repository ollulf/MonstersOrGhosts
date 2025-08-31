using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization; // Für FormerlySerializedAs

public class ShipMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float maxDistance = 0.35f;
    [SerializeField] private float routeOffSet = 0f;

    [FormerlySerializedAs("turnspeed")]
    [SerializeField] private float turnSpeedDegPerSec = 120f; // Fallback, falls nötig

    [Header("Soft Rotation")]
    [Tooltip("Zeit (Sek.), über die die Drehung weich auf das Ziel einschwingt.")]
    [SerializeField, Range(0.01f, 1.0f)] private float rotationDampingTime = 0.25f;

    [Tooltip("Ab diesem Winkel (Grad) darf mit voller Geschwindigkeit gefahren werden.")]
    [SerializeField, Range(0f, 45f)] private float moveWhenAngleBelowDeg = 10f;

    [Tooltip("Ab diesem Winkel (Grad) steht das Schiff (Geschwindigkeit ~0).")]
    [SerializeField, Range(30f, 180f)] private float maxAngleForNoMove = 120f;

    [Header("Selection")]
    [SerializeField] private GameObject selectCircle;

    private List<GameObject> wayPoint;
    private bool isSelected;
    private float _yawVel; // für SmoothDampAngle

    public float MovementSpeed => movementSpeed;
    public bool IsSelected => isSelected;

    private void Awake() => isSelected = false;

    private void Update()
    {
        if (wayPoint != null && wayPoint.Count > 0)
        {
            Move();
            if (CheckDistance())
                RemoveFromList();
        }
        else
        {
            // Neue Route wählen
            if (WayPointHandler.WayPoints != null && WayPointHandler.WayPoints.Count > 0)
            {
                GetWayPoint(WayPointHandler.WayPoints[Random.Range(0, WayPointHandler.WayPoints.Count)]);

                // Auf den Startpunkt (inkl. Offset) setzen, dann ersten Wegpunkt konsumieren
                Vector3 startPos = ComputeTargetPosition(wayPoint[0].transform.position, transform.position);
                transform.position = startPos;
                RemoveFromList();
            }
        }
    }

    private void Move()
    {
        // Zielposition inkl. stabilem Seitenversatz (nicht abhängig von eigener Rotation)
        Vector3 targetPos = ComputeTargetPosition(wayPoint[0].transform.position, transform.position);
        Vector3 to = targetPos - transform.position;
        float sqr = to.sqrMagnitude;
        if (sqr < 1e-6f) return;

        Vector3 dir = to.normalized;

        // Ziel-Yaw aus Richtung bestimmen (Schiff bleibt horizontal)
        float targetYaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        // Aktuellen Yaw (Euler Y) weich auf targetYaw ziehen
        float currentYaw = transform.eulerAngles.y;
        float yaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref _yawVel, rotationDampingTime);

        // Optional: Begrenze maximale Drehgeschwindigkeit zusätzlich hart
        float maxStep = turnSpeedDegPerSec * Time.deltaTime;
        float clampedYaw = Mathf.MoveTowardsAngle(currentYaw, yaw, maxStep);

        transform.rotation = Quaternion.Euler(0f, clampedYaw, 0f);

        // Bewegungs-Blend abhängig vom Ausrichtungswinkel -> verhindert „Seitwärtsrutschen“
        float angleToTarget = Mathf.DeltaAngle(clampedYaw, targetYaw);
        float moveBlend = Mathf.Clamp01(Mathf.InverseLerp(maxAngleForNoMove, moveWhenAngleBelowDeg, Mathf.Abs(angleToTarget)));

        float step = movementSpeed * moveBlend * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }

    // Versatz berechnen: statt transform.right nehmen wir eine stabile Rechtsachse aus der Weg-Richtung
    // so bleibt der Offset konstant relativ zur Route, nicht relativ zur momentanen Schiffsrotation
    private Vector3 ComputeTargetPosition(Vector3 waypointPos, Vector3 fromPos)
    {
        Vector3 pathDir = (waypointPos - fromPos).normalized;
        if (pathDir.sqrMagnitude < 1e-6f) pathDir = transform.forward;

        Vector3 rightFromPath = Vector3.Cross(Vector3.up, pathDir).normalized;
        return waypointPos + rightFromPath * routeOffSet;
    }

    private bool CheckDistance()
    {
        if (wayPoint == null || wayPoint.Count == 0 || wayPoint[0] == null) return true;
        Vector3 target = ComputeTargetPosition(wayPoint[0].transform.position, transform.position);
        return Vector3.Distance(transform.position, target) <= maxDistance;
    }

    public void GetWayPoint(WayPointPlacingSystem newWayPointPlacingSystem)
        => wayPoint = new List<GameObject>(newWayPointPlacingSystem.GetWayPoints());

    public void RemoveFromList()
    {
        if (wayPoint != null && wayPoint.Count > 0)
            wayPoint.RemoveAt(0);
    }

    public void SetIsSelected()
    {
        isSelected = !isSelected;
        if (selectCircle) selectCircle.SetActive(isSelected);
    }
}
