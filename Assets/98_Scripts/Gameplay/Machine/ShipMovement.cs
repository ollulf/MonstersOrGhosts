using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    private List<GameObject> wayPoint;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 direction = (wayPoint[0].transform.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = lookRotation.eulerAngles;


        transform.position += direction * movementSpeed * Time.deltaTime;
    }

    public void GetWayPoint(WayPointPlacingSystem newWayPointPlacingSystem)
    {
        wayPoint = new List<GameObject>(newWayPointPlacingSystem.GetWayPoints());
    }

    public void RemoveFromList()
    {
        wayPoints.Remove(wayPoints[0]);
    }

}
