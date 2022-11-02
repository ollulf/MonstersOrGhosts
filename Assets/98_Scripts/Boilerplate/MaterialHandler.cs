using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MaterialHandler : Singleton<MaterialHandler>
{
    private List<MaterialSwitcher> materialEvents;

    protected override void Awake()
    {
        base.Awake();
        materialEvents = new List<MaterialSwitcher>();
    }

    public static void AddListener(MaterialSwitcher newMatFunc)
    {
        Debug.Log("KOMM SCHON");
        Instance.materialEvents.Add(newMatFunc);
    }

    public static void StartEvent()
    {
        foreach (MaterialSwitcher materialSwitcher in Instance.materialEvents)
        {
            Debug.Log("Start event");
            materialSwitcher.ChangeMaterial();
            Debug.Log("End Event");
        }
    }

    public static void StartDefaultEvent()
    {
        Debug.Log("Start default event");
        foreach (MaterialSwitcher materialSwitcher in Instance.materialEvents)
        {
            Debug.Log("Start event");
            materialSwitcher.ChangeMaterialToDefault();
            Debug.Log("End Event");
        }
    }
}
