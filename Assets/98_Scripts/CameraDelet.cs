using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraDelet : MonoBehaviourPun
{
    void Start()
    {
        if(!photonView.IsMine)
        {
            Destroy(gameObject);
        }
    }
}