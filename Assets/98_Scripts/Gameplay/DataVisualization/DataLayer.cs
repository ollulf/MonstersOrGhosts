using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLayer : MonoBehaviour
{
    [SerializeField] Texture2D texture;
    public Texture2D Texture => texture;

    DataVisualisationController controller;
    private void OnValidate() => controller = GetComponentInParent<DataVisualisationController>();

    public void UIToggle()
    {
        controller.Toggle(this);
    }

}
