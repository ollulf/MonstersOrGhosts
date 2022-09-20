using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed, maxDistance;
    private List<GameObject> wayPoint;

    public float MovementSpeed { get => movementSpeed;}

    private void Start()
    {
        ShipHandler.AddShip(gameObject);
    }

    private void FixedUpdate()
    {
        Move();
        if(CheckDistance())
        {
            RemoveFromList();
        }
    }

    private void Move()
    {
        Vector3 direction = (wayPoint[0].transform.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = lookRotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rotation);

        transform.position += direction * movementSpeed * Time.deltaTime;
    }

    private bool CheckDistance()
    {
        return Vector3.Distance(transform.position, wayPoint[0].transform.position) <= maxDistance;
    }

    public void GetWayPoint(WayPointPlacingSystem newWayPointPlacingSystem)
    {
        wayPoint = new List<GameObject>(newWayPointPlacingSystem.GetWayPoints());
    }

    public void RemoveFromList()
    {
        wayPoint.Remove(wayPoint[0]);
    }

}
