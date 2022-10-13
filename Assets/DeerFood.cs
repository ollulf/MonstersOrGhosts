using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DeerFood : MonoBehaviour
{
    public float minFoodCapacity = 500, maxFoodCapacity = 1000;
    public float moveDownIntensity = 1;

    [SerializeField] GameObject foodVisuals;

    [ShowNonSerializedField]private float foodCapacity;

    private Vector3 startPosition;
    private float tempfoodCapacity;


    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        foodCapacity = Random.Range(minFoodCapacity, maxFoodCapacity);
        tempfoodCapacity = foodCapacity;
    }

    public void ReduceFoodCapacity(float amount)
    {
        foodCapacity -= amount;

        if (foodCapacity <= 0)
            Destroy(gameObject);

    }

    void Update()
    {
        if(tempfoodCapacity != foodCapacity)
        {
            foodVisuals.transform.position = new Vector3 (startPosition.x, startPosition.y - (foodCapacity+1)/maxFoodCapacity*moveDownIntensity, startPosition.z);
        }
    } 
}
