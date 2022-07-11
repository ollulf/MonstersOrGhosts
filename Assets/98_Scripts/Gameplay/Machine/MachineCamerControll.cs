using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MachineCamerControll : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI money, refineryCost;
    [SerializeField] private GameObject canvas;

    private void Start()
    {
        if(!photonView.IsMine)
        {
            Destroy(canvas);
        }
    }

    void Update()
    {
        money.text = "Money: " + RefineryHandler.Money.ToString();
        refineryCost.text = "Refinery Cost: " + RefineryHandler.RefineryCost.ToString();
        gameObject.transform.GetChild(0).GetComponent<CameraScroller>().SetIndex(2);
    }
}
