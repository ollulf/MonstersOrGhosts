using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class TempretureHandler : Singleton<TempretureHandler>
{
    private float cO2, tempreture;
    [SerializeField] private TextMeshProUGUI tempretureText;

    public static float Tempreture { get => Instance.tempreture;}

    public static void AddCO2(float newCO2)
    {
        Instance.cO2 += newCO2;
        Instance.ShowTempreture();
    }

    public static void AddCompressedCO2(float compressCO2)
    {
        Instance.cO2 -= compressCO2;
        Instance.ShowTempreture();
    }

    private void ShowTempreture()
    {
        tempreture = (cO2 / FirstDataGive.CO2NeededFor1C);
        tempretureText.text = tempreture.ToString("F2") + "°C";
    }
}
