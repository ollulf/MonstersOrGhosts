using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MachineCamerControll : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject canvas;

    private void Start()
    {
        if(!photonView.Owner.IsLocal)
        {
            Destroy(canvas);
        }
    }

}
