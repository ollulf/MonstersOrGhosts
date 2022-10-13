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
        SpawnAnimation();
        startPosition = transform.position;
        foodCapacity = Random.Range(minFoodCapacity, maxFoodCapacity);
        tempfoodCapacity = foodCapacity;
    }

    public void ReduceFoodCapacity(float amount)
    {
        foodCapacity -= amount;

        if (foodCapacity <= 0)

            DespawnAnimation();
    }
    private void SpawnAnimation() //moves out of the floor
    {
        throw new System.NotImplementedException();
    }
    private void DespawnAnimation() //move into the foor and destroys self
    {
        throw new System.NotImplementedException();
    }

    void Update()
    {
        if(tempfoodCapacity != foodCapacity)
        {
            foodVisuals.transform.position = new Vector3 (startPosition.x, startPosition.y - (foodCapacity+1)/maxFoodCapacity*moveDownIntensity, startPosition.z);
        }
    } 
}
