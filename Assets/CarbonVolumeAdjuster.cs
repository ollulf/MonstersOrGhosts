using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CarbonVolumeAdjuster : MonoBehaviour
{
    [SerializeField] public VolumeProfile volumeProfile;
    private ColorAdjustments colorAdjustments;

    public float tempIncreaseUntilStart = 0, tempIncreaseMax = 5;
    public float startContrast = 0, endContrast = -20;

    public float curentTempIncrease;

    // Start is called before the first frame update
    void Awake()
    {
        ColorAdjustments temp;
        if (volumeProfile.TryGet<ColorAdjustments>(out temp))
        {
            colorAdjustments = temp;
            colorAdjustments.contrast.overrideState = true;
        }
    }
    void Update()
    {
        GetCurrentTemp();
        UpdateVolumeConstrast();
    }

    private void GetCurrentTemp()
    {
        curentTempIncrease = TempretureHandler.Tempreture;
    }

    private void UpdateVolumeConstrast()
    {
        Mathf.Clamp(colorAdjustments.contrast.value = startContrast - (curentTempIncrease / (tempIncreaseMax - tempIncreaseUntilStart) * (Mathf.Abs(startContrast - endContrast))), startContrast, endContrast);
    }
}
