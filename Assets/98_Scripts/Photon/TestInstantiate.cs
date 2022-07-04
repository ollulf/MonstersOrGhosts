using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestInstantiate : MonoBehaviour
{
    [SerializeField] private Transform birdPosition, fishPosition, bacteriaPosition, deerPosition, machinePosition;

    private void Awake()
    {
        Camera mainCamera = Camera.main;

        Charakter newCharakter = (Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"];

        switch (newCharakter)
        {
            case Charakter.Bird:
                {
                    GameObject bird = PhotonNetwork.Instantiate("Bird", birdPosition.position, transform.rotation);
                    mainCamera.GetComponent<CameraScroller>().SetClosePosition(bird.transform);
                    break;
                }
            case Charakter.Fish:
                {
                    GameObject fish = PhotonNetwork.Instantiate("Fish", fishPosition.position, Quaternion.Euler(90, 0, 0));
                    mainCamera.GetComponent<CameraScroller>().SetClosePosition(fish.transform);
                    break;
                }
            case Charakter.Bacteria:
                {
                    GameObject bacteria = PhotonNetwork.Instantiate("Bacteria", bacteriaPosition.position, transform.rotation);
                    mainCamera.GetComponent<CameraScroller>().SetClosePosition(bacteria.transform);
                    break;
                }
            case Charakter.Deer:
                {
                    GameObject deer = PhotonNetwork.Instantiate("Deer", deerPosition.position, Quaternion.Euler(90, 0, 0));
                    mainCamera.GetComponent<CameraScroller>().SetClosePosition(deer.transform);
                    break;
                }
            case Charakter.Machine:
                {
                    GameObject machine = PhotonNetwork.Instantiate("Machine", machinePosition.position, transform.rotation);
                    mainCamera.GetComponent<CameraScroller>().SetClosePosition(machine.transform);
                    break;
                }
        }

    }
}
