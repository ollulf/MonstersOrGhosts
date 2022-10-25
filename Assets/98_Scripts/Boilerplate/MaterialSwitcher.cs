using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MaterialSwitcher : MonoBehaviour
{
    [SerializeField] private Material deerMaterial, birdMaterial, fishMaterial, bacteriaMaterial, machineMaterial, iceMaterial;
    private Material defaultMaterial;

    // Start is called before the first frame update
    void Start()
    {
        defaultMaterial = GetComponent<MeshRenderer>().material;
        MaterialHandler.AddListener(this);
    }

    public void ChangeMaterial()
    {
        Debug.LogWarning("CHANGE MATERIAL");
        switch((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"])
        {
            case Charakter.Bacteria:
                {
                    Debug.LogWarning(gameObject.name + "Bacteria");
                    break;
                }
            case Charakter.Bird:
                {
                    Debug.LogWarning(gameObject.name + "Bird");
                    break;
                }
            case Charakter.Deer:
                {
                    Debug.LogWarning(gameObject.name + "Deer");
                    break;
                }
            case Charakter.Fish:
                {
                    Debug.LogWarning(gameObject.name + "Fish");
                    break;
                }
            case Charakter.Ice:
                {
                    Debug.LogWarning(gameObject.name + "Ice");
                    break;
                }
            case Charakter.Machine:
                {
                    Debug.LogWarning(gameObject.name + "Machine");
                    break;
                }
        }
    }

    public void ChangeMaterialToDefault()
    {
        Debug.LogWarning(gameObject.name + "Back to Normal");
    }
}
