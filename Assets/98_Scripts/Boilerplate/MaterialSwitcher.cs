using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class MaterialSwitcher : MonoBehaviour
{
    [SerializeField] private Material deerMaterial, birdMaterial, fishMaterial, bacteriaMaterial, machineMaterial, iceMaterial;
    [ShowNonSerializedField] private Material defaultMaterial;

    void Start()
    {
        if (CheckMeshRenderer())
        {
            defaultMaterial = GetComponent<MeshRenderer>().material;
            MaterialHandler.AddListener(this);
        }
        else if (CheckSkinnedRenderer())
        {
            defaultMaterial = GetComponent<SkinnedMeshRenderer>().material;
            MaterialHandler.AddListener(this);
        }
        else
        {
            Debug.LogError("Something went wrong. No renderer found!");
        }
    }

    public void ChangeMaterial()
    {
        switch ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"])
        {
            case Charakter.Methanosarcina:
                {
                    if (bacteriaMaterial != null)
                    {
                        CheckRenderer(bacteriaMaterial);
                    }
                    break;
                }
            case Charakter.ArcticTern:
                {
                    if (birdMaterial != null)
                    {
                        CheckRenderer(birdMaterial);
                    }
                    break;
                }
            case Charakter.Caribou:
                {
                    if (deerMaterial != null)
                    {
                        CheckRenderer(deerMaterial);
                    }
                    break;
                }
            case Charakter.ArcticCod:
                {
                    if (fishMaterial != null)
                    {
                        CheckRenderer(fishMaterial);
                    }
                    break;
                }
            case Charakter.Ice:
                {
                    if (iceMaterial != null)
                    {
                        CheckRenderer(iceMaterial);
                    }
                    break;
                }
            case Charakter.Machine:
                {
                    if (machineMaterial != null)
                    {
                        CheckRenderer(machineMaterial);
                    }
                    break;
                }
        }
    }

    public void ChangeMaterialToDefault() => CheckRenderer(defaultMaterial);

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