using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class FlockingTest : MonoBehaviour
{
    [SerializeField] GameObject flock;
    [SerializeField] Transform target, motherFlock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    private void CreateFlock()
    {
        GameObject flocki = Instantiate(flock);
        flocki.GetComponent<Flocking>().SetTarget(target);
        flocki.GetComponent<Flocking>().SetMotherFlock(motherFlock);
        flocki.transform.parent = motherFlock.transform;
    }
}
