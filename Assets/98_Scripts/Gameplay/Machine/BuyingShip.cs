using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BuyingShip : MonoBehaviourPun
{
    public void OnClick()
    {
        WayPointPlacingSystem wayPoint = WayPointHandler.WayPoints[Random.Range(0, WayPointHandler.WayPoints.Count)];

        GameObject ship = PhotonNetwork.Instantiate("MachineGame/Ship", wayPoint.GetStartPoint().position, Quaternion.identity);
        ship.GetComponent<ShipMovement>().GetWayPoint(wayPoint);
    }
}
