using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Photon.Pun;

public class TerrainSwitcher : MonoBehaviour
{
    [ShowNonSerializedField] private Charakter localChar;
    [SerializeField] private Material bacteriaMat, birdMat, centralProjectionMat, deerMat, fishMat, machineMat, iceMat;

    public Terrain terrain;
    // Start is called before the first frame update
    void Start()
    {
        localChar = (Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"];

        switch (localChar)
        {
            case Charakter.Methanosarcina:
                terrain.materialTemplate = bacteriaMat;
                break;
            case Charakter.ArcticTern:
                terrain.materialTemplate = birdMat;
                break;
            case Charakter.Observer:
                terrain.materialTemplate = centralProjectionMat;
                break;
            case Charakter.Caribou:
                terrain.materialTemplate = deerMat;
                break;
            case Charakter.ArcticCod:
                terrain.materialTemplate = fishMat;
                break;
            case Charakter.Machine:
                terrain.materialTemplate = machineMat;
                break;
            case Charakter.Ice:
                terrain.materialTemplate = iceMat;
                break;
            default:
                Debug.LogWarning("No new Material assigned to Terrain");
                break;
        }
        //Debug.Log("Currently used Material on Terrain: " + terrain.materialTemplate);
    }
}
