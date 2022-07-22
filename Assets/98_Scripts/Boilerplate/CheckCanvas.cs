using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Photon.Pun;

public class CheckCanvas : MonoBehaviourPun
{

    private void Start()
    {
        if(!photonView.IsMine)
        {
            Destroy(gameObject);
        }
    }

}
