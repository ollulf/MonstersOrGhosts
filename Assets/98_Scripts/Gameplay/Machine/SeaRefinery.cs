using UnityEngine;

public class SeaRefinery : MonoBehaviour
{
    void Start() => ShipHandler.AddShip(gameObject);
}