using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MachineCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText, incomeText, shipIndustrieCost;

    // Start is called before the first frame update
    void Start()
    {        
        moneyText.text = ShipHandler.Money.ToString();        
        incomeText.text = ShipHandler.Income().ToString();
        shipIndustrieCost.text = ShipHandler.ShipCost.ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moneyText.text = ShipHandler.Money.ToString() + "€";
        incomeText.text = ShipHandler.Income().ToString() + "€";
    }
}
