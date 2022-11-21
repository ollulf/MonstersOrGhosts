using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class MaterialSwitcher : MonoBehaviour
{
    [SerializeField] private Material deerMaterial, birdMaterial, fishMaterial, bacteriaMaterial, machineMaterial, iceMaterial;
    [ShowNonSerializedField] private Material defaultMaterial;

    // Start is called before the first frame update
    void Start()
    {
        if (CheckMeshRenderer())
        {
            defaultMaterial = GetComponent<MeshRenderer>().material;
        }
        else if (CheckSkinnedRenderer())
        {
            defaultMaterial = GetComponent<SkinnedMeshRenderer>().material;
        }
        else
        {
            Debug.LogError("Something went wrong. No renderer found!");
        }

        MaterialHandler.AddListener(this);
    }

    public void ChangeMaterial()
    {
        //Debug.LogWarning("CHANGE MATERIAL");
        switch ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"])
        {
            case Charakter.Methanosarcina:
                {
                    if (bacteriaMaterial != null)
                    {
                        //Debug.LogWarning(gameObject.name + "Methanosarcina MeshRenderer");
                        CheckRenderer(bacteriaMaterial);
                    }
                    break;
                }
            case Charakter.ArcticTern:
                {
                    if (birdMaterial != null)
                    {
                        //Debug.LogWarning(gameObject.name + "ArcticTern MeshRenderer");
                        CheckRenderer(birdMaterial);
                    }
                    break;
                }
            case Charakter.Caribou:
                {
                    if (deerMaterial != null)
                    {
                        //Debug.LogWarning(gameObject.name + "Caribou MeshRenderer");
                        CheckRenderer(deerMaterial);
                    }
                    break;
                }
            case Charakter.ArcticCod:
                {
                    if (fishMaterial != null)
                    {
                        //Debug.LogWarning(gameObject.name + "ArcticCod MeshRenderer");
                        CheckRenderer(fishMaterial);
                    }
                    break;
                }
            case Charakter.Ice:
                {
                    if (iceMaterial != null)
                    {
                        //Debug.LogWarning(gameObject.name + "Ice MeshRenderer");
                        CheckRenderer(iceMaterial);
                    }
                    break;
                }
            case Charakter.Machine:
                {
                    if (machineMaterial != null)
                    {
                        //Debug.LogWarning(gameObject.name + "Machine MeshRenderer");
                        CheckRenderer(machineMaterial);
                    }
                    break;
                }
        }
    }

    public void ChangeMaterialToDefault()
    {
        //Debug.LogWarning(gameObject.name + "Back to Normal");
        CheckRenderer(defaultMaterial);
    }

    private void CheckRenderer(Material newMaterial)
    {
        if (CheckMeshRenderer())
        {
            ChangeMeshRendererMaterial(newMaterial);
        }
        else if (CheckSkinnedRenderer())
        {
            ChangeSkinned(newMaterial);
        }
        else
        {
            Debug.LogError("No renderer found on " + gameObject.name);
        }
    }

    private bool CheckMeshRenderer() => GetComponent<MeshRenderer>() != null;

    private bool CheckSkinnedRenderer() => GetComponent<SkinnedMeshRenderer>() != null;

    private void ChangeMeshRendererMaterial(Material newMaterial) => GetComponent<MeshRenderer>().material = newMaterial;

    private void ChangeSkinned(Material newMaterial) => GetComponent<SkinnedMeshRenderer>().material = newMaterial;

    private void OnDestroy()
    {
        ChangeMaterialToDefault();
        MaterialHandler.RemoveListener(this);
    }
}