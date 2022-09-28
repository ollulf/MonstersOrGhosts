using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MachineCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText, incomeText, shipIndustrieCost, induShips ,carbonIncrease, enviCost,carbonProduce, tempreture;

    // Start is called before the first frame update
    void Start()
    {        
        moneyText.text = ShipHandler.Money.ToString() + "€";        
        incomeText.text = ShipHandler.Income().ToString() + "€";
        shipIndustrieCost.text = ShipHandler.ShipCost.ToString() + "€";
        induShips.text = ShipHandler.Ship.Count.ToString();
        enviCost.text = ShipHandler.EnviCost.ToString() + "€";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moneyText.text = ShipHandler.Money.ToString() + "€";
        incomeText.text = ShipHandler.Income().ToString() + "€";
        shipIndustrieCost.text = ShipHandler.ShipCost.ToString() + "€";
        induShips.text = ShipHandler.Ship.Count.ToString();
        enviCost.text = ShipHandler.EnviCost.ToString() + "€";
        carbonIncrease.text = ShipHandler.CarbonIncreasePerSecond().ToString();
        carbonProduce.text = ShipHandler.TotalCarbonProduced().ToString();
    }
}
