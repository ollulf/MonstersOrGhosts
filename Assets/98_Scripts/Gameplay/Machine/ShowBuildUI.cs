using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShowBuildUI : MonoBehaviourPun
{
    [SerializeField] GameObject canvas;
    bool isClicked = false;


    public void CanvasShow()
    {
        if (!isClicked)
        {
            Debug.LogError(isClicked);

            isClicked = true;

            canvas.SetActive(isClicked);
            Debug.LogError(isClicked);
        }
        else
        {
            Debug.LogError("Clicking");
            isClicked = false;
            canvas.SetActive(isClicked);
        }
    }
}