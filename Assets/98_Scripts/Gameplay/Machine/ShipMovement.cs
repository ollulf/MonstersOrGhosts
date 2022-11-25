using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed, maxDistance, routeOffSet, turnspeed;
    [SerializeField] GameObject selectCircle;
    private List<GameObject> wayPoint;

    private bool isSelected;

    public float MovementSpeed { get => movementSpeed; }
    public bool IsSelected { get => isSelected;}

    private void Awake()
    {
        isSelected = false;
    }

    private void LateUpdate()
    {
        if (wayPoint != null && wayPoint.Count != 0)
        {
            Move();
            if (CheckDistance())
            {
                RemoveFromList();
            }
        }
        else
        {
            GetWayPoint(WayPointHandler.WayPoints[Random.Range(0, WayPointHandler.WayPoints.Count)]);
            transform.position = wayPoint[0].transform.position + transform.right * routeOffSet;
        }
    }

    private void Move()
    {
        Vector3 direction = ((wayPoint[0].transform.position + transform.right * routeOffSet) - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = lookRotation.eulerAngles;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), Time.fixedDeltaTime * turnspeed);

        if (transform.rotation == Quaternion.Euler(new Vector3(rotation.x, rotation.y + 1, rotation.z)) || transform.rotation == Quaternion.Euler(new Vector3(rotation.x, rotation.y - 1, rotation.z))) ;
        {
            transform.position += direction * movementSpeed * Time.deltaTime;
        }
    }

    private bool CheckDistance()
    {
        return Vector3.Distance(transform.position, wayPoint[0].transform.position + transform.right * routeOffSet) <= maxDistance;
    }

    public void GetWayPoint(WayPointPlacingSystem newWayPointPlacingSystem)
    {
        wayPoint = new List<GameObject>(newWayPointPlacingSystem.GetWayPoints());
    }

    public void RemoveFromList()
    {
        wayPoint.Remove(wayPoint[0]);
    }

    public void SetIsSelected()
    {
        isSelected = !isSelected;
        selectCircle.SetActive(isSelected);
    }
}
