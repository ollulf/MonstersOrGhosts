using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DeerFood : MonoBehaviour
{
    public float minFoodCapacity = 500, maxFoodCapacity = 1000;
    
    [ShowNonSerializedField]private float foodCapacity;



    // Start is called before the first frame update
    void Start()
    {
        foodCapacity = Random.Range(minFoodCapacity, maxFoodCapacity);
    }

    public void ReduceFoodCapacity(float amount)
    {
        foodCapacity -= amount;

        if(foodCapacity <= 0)
            Destroy(gameObject);
    }
}
