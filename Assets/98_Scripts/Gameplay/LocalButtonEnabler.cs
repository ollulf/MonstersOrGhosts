using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class LocalButtonEnabler : MonoBehaviourPun
{
    [SerializeField] private List<GameObject> fish, bird, machine, deer, bacteria, ice;

    void Start()
    {
        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.ArcticCod)
        {
            ActivateButtons(fish);
        }

        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.ArcticTern)
        {
            ActivateButtons(bird);
        }

        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Machine)
        {
            ActivateButtons(machine);
        }

        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Caribou)
        {
            ActivateButtons(deer);
        }

        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Methanosarcina)
        {
            ActivateButtons(bacteria);
        }

        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Ice)
        {
            ActivateButtons(ice);
        }
    }

    private void ActivateButtons(List<GameObject> newList)
    {
        for (int i = 0; i < newList.Count; i++)
        {
            newList[i].SetActive(true);
        }
    }
}
