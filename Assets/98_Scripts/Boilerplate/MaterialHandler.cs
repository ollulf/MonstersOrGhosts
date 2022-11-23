using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MaterialHandler : Singleton<MaterialHandler>
{
    private List<MaterialSwitcher> materialEvents = new List<MaterialSwitcher>();
    [SerializeField] private WorldTime world;

    public static WorldTime World { get => Instance.world;}

    public static void AddListener(MaterialSwitcher newMatFunc)
    {
        Instance.materialEvents.Add(newMatFunc);
    }

    public static void RemoveListener(MaterialSwitcher newMatFunc)
    {
        Instance.materialEvents.Remove(newMatFunc);
    }

    public static void StartEvent()
    {
        foreach (MaterialSwitcher materialSwitcher in Instance.materialEvents)
        {
            //Debug.Log("Start event");
            materialSwitcher.ChangeMaterial();
            //Debug.Log("End Event");
        }
    }

    public static void StartDefaultEvent()
    {
        //Debug.Log("Start default event");
        foreach (MaterialSwitcher materialSwitcher in Instance.materialEvents)
        {
            //Debug.Log("Start event");
            materialSwitcher.ChangeMaterialToDefault();
            //Debug.Log("End Event");
        }
    }
}
