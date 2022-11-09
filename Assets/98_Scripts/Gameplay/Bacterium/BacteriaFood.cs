using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaFood : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BacteriaFoodHandler.AddToList(gameObject);
    }

    public void RemoveFromList()
    {
        BacteriaFoodHandler.RemoveFromList(gameObject);
    }
}
