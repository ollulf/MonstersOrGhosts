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
            case Charakter.Bacteria:
                terrain.materialTemplate = bacteriaMat;
                break;
            case Charakter.Bird:
                terrain.materialTemplate = birdMat;
                break;
            case Charakter.MiddleProjection:
                terrain.materialTemplate = centralProjectionMat;
                break;
            case Charakter.Deer:
                terrain.materialTemplate = deerMat;
                break;
            case Charakter.Fish:
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
