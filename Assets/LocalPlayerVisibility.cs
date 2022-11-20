using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LocalPlayerVisibility : MonoBehaviour
{
    [SerializeField] private Charakter charakter;
    [SerializeField] private bool included;

    // Start is called before the first frame update
    void Start()
    {
        if(included)
        {
            Debug.Log("Try Destroy ");
            if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] != charakter)
            {
                Debug.Log("I am not Fish");
                Destroy(gameObject);
            }

        }
        if(!included)
        {
            if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == charakter)
            {
                Debug.Log("I am Fish");
                Destroy(gameObject);
            }
        }
    }

}
