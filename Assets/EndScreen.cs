using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EndScreen : MonoBehaviour
{
    public float realTotalTemperatureIncrease;
    [SerializeField] TextMeshProUGUI realTemperatureIncreaseTMP, gameTotalTemperatureIncreaseTMP;

    [SerializeField] TextMeshProUGUI CarbonDioxideTMP;

    [SerializeField] TextMeshProUGUI MoneyGeneratedTotalTMP;

    [SerializeField] PopulationValues arcticTern, arcticCod, deer;

    private int iceCoverage;
    public float maxTempIncrease = 5;
    [SerializeField] TextMeshProUGUI iceCoverageChangeTMP;

    enum MethaneProduction {low, medium, high};
    public int mediumInAmount = 15, mediumOutAmount = 30;
    [SerializeField] TextMeshProUGUI methaneProductionTMP;

    /// <summary>
    /// Updates Game Values and UI
    /// </summary>
    public void UpdateGameValues(EndGameValues endGameValues)
    {
        arcticTern.UpdateValues(endGameValues.Tern);
        arcticCod.UpdateValues(endGameValues.Cod);
        deer.UpdateValues(endGameValues.Deer);

        realTemperatureIncreaseTMP.text = realTotalTemperatureIncrease.ToString();
        
        gameTotalTemperatureIncreaseTMP.color = realTotalTemperatureIncrease < endGameValues.GameTotalTemperatureIncrease 
            ? new Color(255, 129, 129, 255) : new Color(222, 255, 129, 255);
        gameTotalTemperatureIncreaseTMP.text = endGameValues.GameTotalTemperatureIncrease.ToString();

        CarbonDioxideTMP.text = endGameValues.TotalCarbonDioxide.ToString();
        MoneyGeneratedTotalTMP.text = endGameValues.MoneyGeneratedTotal.ToString();

        iceCoverage = (int) (endGameValues.GameTotalTemperatureIncrease / maxTempIncrease * 1000);
        iceCoverageChangeTMP.color = realTotalTemperatureIncrease < endGameValues.GameTotalTemperatureIncrease 
            ? new Color(255, 129, 129, 255) : new Color(222, 255, 129, 255);
        iceCoverageChangeTMP.text = iceCoverage.ToString() + " %";

        methaneProductionTMP.color = CalculateMethaneEnum(
            endGameValues.TotalAmountOfCollectedAcetate, mediumInAmount, mediumOutAmount) == MethaneProduction.medium
            ? new Color(255,200,129, 255) : new Color(255, 129, 129, 255);
        methaneProductionTMP.text = CalculateMethaneEnum(
            endGameValues.TotalAmountOfCollectedAcetate, mediumInAmount, mediumOutAmount).ToString();
    }

    private MethaneProduction CalculateMethaneEnum(int totalAmountOfCollectedAcetate, int mediumInAmount, int mediumOutAmount)
    {
        if (totalAmountOfCollectedAcetate < mediumInAmount) return MethaneProduction.low;
        return (totalAmountOfCollectedAcetate < mediumOutAmount) ? MethaneProduction.medium : MethaneProduction.high;
    }

    // Start is called before the first frame update
    void Start()
    {
        EndGameValues endGameValues = new EndGameValues(0.1f ,1000, 10000000, 20,
            new PopulationPair(99,100), new PopulationPair(99, 80), new PopulationPair(99, 100));

        UpdateGameValues(endGameValues);
    }

}

[System.Serializable]
 public class PopulationValues
{
    public TextMeshProUGUI start, end, difference;

    internal void UpdateValues(PopulationPair pair)
    {
        int dif = pair.endPop - pair.startPop;

        difference.color = dif < 0 ? new Color(255, 129, 129, 255) : new Color(222, 255, 129);
        difference.color = dif < 0 ? Color.red : Color.green;
        start.text = pair.startPop.ToString();
        end.text = pair.endPop.ToString();
        difference.text = dif.ToString();

    }
}

[System.Serializable]
public class ValueTMPPair
{
    private float value;
    public TextMeshProUGUI tmp;
}

public class PopulationPair
{
    public int startPop;
    public int endPop;

    public PopulationPair(int startPop, int endPop)
    {
        this.startPop = startPop;
        this.endPop = endPop;
    }
}
public class EndGameValues
{
    private float gameTotalTemperatureIncrease;
    private float totalCarbonDioxide;
    private float moneyGeneratedTotal;
    private int totalAmountOfCollectedAcetate;
    private PopulationPair tern, cod, deer;

    public EndGameValues(float gameTotalTemperatureIncrease, float totalCarbonDioxide, float moneyGeneratedTotal, int totalAmountOfCollectedAcetate,
        PopulationPair tern, PopulationPair cod, PopulationPair deer)
    {
        this.gameTotalTemperatureIncrease = gameTotalTemperatureIncrease;
        this.totalCarbonDioxide = totalCarbonDioxide;
        this.moneyGeneratedTotal = moneyGeneratedTotal;
        this.totalAmountOfCollectedAcetate = totalAmountOfCollectedAcetate;
        this.tern = tern;
        this.cod = cod;
        this.deer = deer;
    }

    public float GameTotalTemperatureIncrease { get => gameTotalTemperatureIncrease;}
    public float TotalCarbonDioxide { get => totalCarbonDioxide;}
    public float MoneyGeneratedTotal { get => moneyGeneratedTotal;}
    public int TotalAmountOfCollectedAcetate { get => totalAmountOfCollectedAcetate;}
    public PopulationPair Tern { get => tern;}
    public PopulationPair Cod { get => cod;}
    public PopulationPair Deer { get => deer;}
}
