using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceScaler : MonoBehaviour
{
    [SerializeField] public Transform[] iceSheets;

    public float temperatureIncreaseToStart = 0, temperatureIncreaseWhenGone = 5;
    public float currentTemperatureIncrease = 0;
    public float scale = 1;

    void Update()
    {
        GetCurrentTemp();
        ScaleIceSheets();
    }

    private void GetCurrentTemp()
    {
        currentTemperatureIncrease = TempretureHandler.Tempreture;
    }

    private void ScaleIceSheets()
    {
        scale = 1 - Mathf.Clamp01((currentTemperatureIncrease + 0.01f) / (temperatureIncreaseWhenGone - temperatureIncreaseToStart));

        foreach (var iceSheet in iceSheets)
        {
            iceSheet.localScale = new Vector3(scale, scale, scale);
        }
    }
}
