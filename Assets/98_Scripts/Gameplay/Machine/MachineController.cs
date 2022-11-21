using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MachineController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject canvas, buttons;
    [SerializeField] private List<GameObject> prefab;

    private void Start()
    {
        if(!photonView.Owner.IsLocal)
        {
            Destroy(canvas);
        }
    }

    public void ChangeButtonActive(bool newBool)
    {
        buttons.SetActive(newBool);
    }

    public void BuyOnClick()
    {
        if (ShipHandler.Money > ShipHandler.ShipCost)
        {
            WayPointPlacingSystem wayPoint = WayPointHandler.WayPoints[Random.Range(0, WayPointHandler.WayPoints.Count)];

            GameObject ship = PhotonNetwork.Instantiate("MachineGame/" + prefab[Random.Range(0,prefab.Count)].name, wayPoint.GetStartPoint().position, Quaternion.identity);
            ship.GetComponent<ShipMovement>().GetWayPoint(wayPoint);
            ShipHandler.AddShip(ship);
            ShipHandler.SetMoney(ShipHandler.ShipCost);
            ShipHandler.RiseShipCost();
        }
    }

    public void EnviOnClick()
    {
        if (ShipHandler.Money > ShipHandler.EnviCost)
        {
            ShipHandler.EnviOption();
        }
    }
}
