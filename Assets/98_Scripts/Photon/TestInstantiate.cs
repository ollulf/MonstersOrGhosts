using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestInstantiate : MonoBehaviour
{
    [SerializeField] private Transform birdPosition, fishPosition, bacteriaPosition, deerPosition, machinePosition;

    private void Awake()
    {



        Charakter newCharakter = (Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"];

        switch (newCharakter)
        {
            case Charakter.Bird:
                {
                    PhotonNetwork.Instantiate("Bird", birdPosition.position, transform.rotation);
                    break;
                }
            case Charakter.Fish:
                {
                    PhotonNetwork.Instantiate("Fish", fishPosition.position, Quaternion.Euler(90, 0, 0));
                    break;
                }
            case Charakter.Bacteria:
                {
                    PhotonNetwork.Instantiate("Bacteria", bacteriaPosition.position, transform.rotation);
                    break;
                }
            case Charakter.Deer:
                {
                    PhotonNetwork.Instantiate("Deer", deerPosition.position, Quaternion.Euler(90, 0, 0));
                    break;
                }
            case Charakter.Machine:
                {
                    PhotonNetwork.Instantiate("Machine", machinePosition.position, transform.rotation);
                    break;
                }
        }

    }
}
