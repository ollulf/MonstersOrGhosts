using UnityEngine;

public class UnparentAndKeep : MonoBehaviour
{
    [ContextMenu("Unparent and Keep Transforms")]
    void Unparent()
    {
        Transform t = transform;
        t.SetParent(null, true); // the ⁠ true ⁠ keeps world position
    }
}