using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffektOnDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject objectEffekt;
    
    void OnDestroy()
    {
        Instantiate(objectEffekt, gameObject.transform);
    }
}
