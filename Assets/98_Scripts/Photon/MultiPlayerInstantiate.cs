using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiPlayerInstantiate : MonoBehaviour
{
    [SerializeField] private Transform birdPosition, fishPosition, bacteriaPosition, deerPosition, machinePosition, icePosition, middleProjectionPosition, overViewPosition, scientificOverViewPosition;

    private void Awake()
    {
        Charakter newCharakter = (Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"];

        switch (newCharakter)
        {
            case Charakter.ArcticTern:
                {
                    GameObject bird = PhotonNetwork.Instantiate("BirdGame/Bird", birdPosition.position, transform.rotation);
                    bird.transform.GetChild(0).GetComponent<CameraScroller>().SetFarPosition(overViewPosition, scientificOverViewPosition);
                    break;
                }
            case Charakter.ArcticCod:
                {
                    GameObject fish = PhotonNetwork.Instantiate("FishGame/Fish", fishPosition.position, Quaternion.Euler(90, 0, 0));
                    fish.transform.GetChild(0).GetComponent<CameraScroller>().SetFarPosition(overViewPosition, scientificOverViewPosition);
                    break;
                }
            case Charakter.Methanosarcina:
                {
                    GameObject bacteria = PhotonNetwork.Instantiate("BacteriaGame/Bacteria", bacteriaPosition.position, transform.rotation);
                    bacteria.transform.GetChild(0).GetComponent<CameraScroller>().SetFarPosition(overViewPosition, scientificOverViewPosition);
                    break;
                }
            case Charakter.Caribou:
                {
                    GameObject deer = PhotonNetwork.Instantiate("DeerGame/Deer", deerPosition.position, transform.rotation);
                    deer.transform.GetChild(0).GetComponent<CameraScroller>().SetFarPosition(overViewPosition, scientificOverViewPosition);
                    break;
                }
            case Charakter.Machine:
                {
                    GameObject machine = PhotonNetwork.Instantiate("MachineGame/Machine", machinePosition.position, transform.rotation);
                    machine.transform.GetChild(0).GetComponent<CameraScroller>().SetFarPosition(overViewPosition, scientificOverViewPosition);
                    break;
                }
            case Charakter.Ice:
                {
                    GameObject ice = PhotonNetwork.Instantiate("IceGame/Ice", machinePosition.position, transform.rotation);
                    ice.transform.GetChild(0).GetComponent<CameraScroller>().SetFarPosition(overViewPosition, scientificOverViewPosition);
                    break;
                }
            case Charakter.Observer:
                {
                    GameObject middle = PhotonNetwork.Instantiate("CentralProjection/CentralProjection", machinePosition.position, transform.rotation);
                    middle.transform.GetChild(0).GetComponent<CameraScroller>().SetFarPosition(overViewPosition, scientificOverViewPosition);
                    break;
                }
        }
    }
}