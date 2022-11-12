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
        if (GetComponent<MeshRenderer>() != null)
        {
            defaultMaterial = GetComponent<MeshRenderer>().material;
        }
        else if (GetComponent<SkinnedMeshRenderer>() != null)
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
        Debug.LogWarning("CHANGE MATERIAL");
        if (GetComponent<MeshRenderer>() != null)
        {
            Debug.Log("MeshRenderer active");
            switch ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"])
            {
                case Charakter.Bacteria:
                    {
                        Debug.LogWarning(gameObject.name + "Bacteria MeshRenderer");
                        ChangeMeshRendererMaterial(bacteriaMaterial);
                        break;
                    }
                case Charakter.Bird:
                    {
                        Debug.LogWarning(gameObject.name + "Bird MeshRenderer");
                        ChangeMeshRendererMaterial(birdMaterial);
                        break;
                    }
                case Charakter.Deer:
                    {
                        Debug.LogWarning(gameObject.name + "Deer MeshRenderer");
                        ChangeMeshRendererMaterial(deerMaterial);
                        break;
                    }
                case Charakter.Fish:
                    {
                        Debug.LogWarning(gameObject.name + "Fish MeshRenderer");
                        ChangeMeshRendererMaterial(fishMaterial);
                        break;
                    }
                case Charakter.Ice:
                    {
                        Debug.LogWarning(gameObject.name + "Ice MeshRenderer");
                        ChangeMeshRendererMaterial(iceMaterial);
                        break;
                    }
                case Charakter.Machine:
                    {
                        Debug.LogWarning(gameObject.name + "Machine MeshRenderer");
                        ChangeMeshRendererMaterial(machineMaterial);
                        break;
                    }
            }
        }
        else if (GetComponent<SkinnedMeshRenderer>() != null)
        {
            Debug.Log("Skinned MeshRenderer active");
            switch ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"])
            {
                case Charakter.Bacteria:
                    {
                        Debug.LogWarning(gameObject.name + "Bacteria Skinned MeshRenderer");
                        ChangeSkinned(bacteriaMaterial);
                        break;
                    }
                case Charakter.Bird:
                    {
                        Debug.LogWarning(gameObject.name + "Bird Skinned MeshRenderer");
                        ChangeSkinned(birdMaterial);
                        break;
                    }
                case Charakter.Deer:
                    {
                        Debug.LogWarning(gameObject.name + "Deer Skinned MeshRenderer");
                        ChangeSkinned(deerMaterial);
                        break;
                    }
                case Charakter.Fish:
                    {
                        Debug.LogWarning(gameObject.name + "Fish Skinned MeshRenderer");
                        ChangeSkinned(fishMaterial);
                        break;
                    }
                case Charakter.Ice:
                    {
                        Debug.LogWarning(gameObject.name + "Ice Skinned MeshRenderer");
                        ChangeSkinned(iceMaterial);
                        break;
                    }
                case Charakter.Machine:
                    {
                        Debug.LogWarning(gameObject.name + "Machine Skinned MeshRenderer");
                        ChangeSkinned(machineMaterial);
                        break;
                    }
            }
        }
        else
        {
            Debug.LogError("Something went wrong. No renderer found!");
        }
    }

    public void ChangeMaterialToDefault()
    {
        Debug.LogWarning(gameObject.name + "Back to Normal");
        if (GetComponent<MeshRenderer>() != null)
        {
            Debug.LogWarning(gameObject.name + "MeshRenderer normal");
            ChangeMeshRendererMaterial(defaultMaterial);
        }
        else if (GetComponent<SkinnedMeshRenderer>() != null)
        {
            Debug.LogWarning(gameObject.name + "Skinned normal");
            ChangeSkinned(defaultMaterial);
        }
        else
        {
            Debug.LogError("Something went wrong. No renderer found!");
        }
    }

    private void ChangeMeshRendererMaterial(Material newMaterial)
    {
        GetComponent<MeshRenderer>().material = newMaterial;
    }

    private void ChangeSkinned(Material newMaterial)
    {
        GetComponent<SkinnedMeshRenderer>().material = newMaterial;
    }
}
