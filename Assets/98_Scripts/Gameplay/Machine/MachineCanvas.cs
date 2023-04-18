using UnityEngine;
using TMPro;
public class MachineCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText, incomeText, shipIndustrieCost, induShips ,enviCost;

    void Start()
    {        
        moneyText.text = $"{ShipHandler.Money}€";        
        incomeText.text = $"{ShipHandler.Income()}€";
        shipIndustrieCost.text = $"{ShipHandler.ShipCost}€";
        induShips.text = $"{ShipHandler.Ship.Count}";
        enviCost.text = $"{ShipHandler.EnviCost}€";
    }

    void FixedUpdate()
    {
        moneyText.text = $"{ShipHandler.Money}€";
        incomeText.text = $"{ShipHandler.Income()}€";
        shipIndustrieCost.text = $"{ShipHandler.ShipCost}€";
        induShips.text = $"{ShipHandler.Ship.Count}";
        enviCost.text = $"{ShipHandler.EnviCost}€";
    }
}