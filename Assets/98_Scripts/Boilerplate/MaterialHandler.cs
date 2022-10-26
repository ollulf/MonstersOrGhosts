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
        for(int i = 0;i<Instance.materialEvents.Count;i++)
        {
            Debug.Log("Start event");

            Instance.materialEvents[i].ChangeMaterial();
            Debug.Log("End Event");

        }
    }

    public static void StartDefaultEvent()
    {
        Debug.Log("Start default event");
        for (int i = 0; i < Instance.materialEvents.Count; i++)
        {
            Instance.materialEvents[i].ChangeMaterialToDefault();
        }
        Debug.Log("End Event");
    }
}
