using System.Collections.Generic;


public class MaterialHandler : Singleton<MaterialHandler>
{
    private List<MaterialSwitcher> materialEvents = new List<MaterialSwitcher>();

    public static void AddListener(MaterialSwitcher newMatFunc) => Instance.materialEvents.Add(newMatFunc);

    public static void RemoveListener(MaterialSwitcher newMatFunc) => Instance.materialEvents.Remove(newMatFunc);

    public static void StartEvent()
    {
        foreach (MaterialSwitcher materialSwitcher in Instance.materialEvents)
        {
            materialSwitcher.ChangeMaterial();
        }
    }

    public static void StartDefaultEvent()
    {
        foreach (MaterialSwitcher materialSwitcher in Instance.materialEvents)
        {
            materialSwitcher.ChangeMaterialToDefault();
        }
    }
}
