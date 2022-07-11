using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestInstantiate : MonoBehaviour
{
    [SerializeField] private Transform birdPosition, fishPosition, bacteriaPosition, deerPosition, machinePosition, overViewPosition;

    private void Awake()
    {

        Charakter newCharakter = (Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"];

        switch (newCharakter)
        {
            case Charakter.Bird:
                {
                    GameObject bird = PhotonNetwork.Instantiate("Bird", birdPosition.position, transform.rotation);
                    bird.transform.GetChild(0).GetComponent<CameraScroller>().SetFarPosition(overViewPosition);
                    break;
                }
            case Charakter.Fish:
                {
                    GameObject fish = PhotonNetwork.Instantiate("Fish", fishPosition.position, Quaternion.Euler(90, 0, 0));
                    fish.transform.GetChild(0).GetComponent<CameraScroller>().SetFarPosition(overViewPosition);
                    break;
                }
            case Charakter.Bacteria:
                {
                    GameObject bacteria = PhotonNetwork.Instantiate("Bacteria", bacteriaPosition.position, transform.rotation);
                    bacteria.transform.GetChild(0).GetComponent<CameraScroller>().SetFarPosition(overViewPosition);
                    break;
                }
            case Charakter.Deer:
                {
                    GameObject deer = PhotonNetwork.Instantiate("Deer", deerPosition.position, Quaternion.Euler(90, 0, 0));
                    deer.transform.GetChild(0).GetComponent<CameraScroller>().SetFarPosition(overViewPosition);
                    break;
                }
            case Charakter.Machine:
                {
                    GameObject machine = PhotonNetwork.Instantiate("MachineGame/Machine", machinePosition.position, transform.rotation);
                    machine.transform.GetChild(0).GetComponent<CameraScroller>().SetFarPosition(overViewPosition);
                    break;
                }
        }

    }
}
