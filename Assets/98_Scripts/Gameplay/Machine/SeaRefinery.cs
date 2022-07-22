using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaRefinery : MonoBehaviour
{

    void Start()
    {
        RefineryHandler.AddRefineries(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
