using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreedingSpot : MonoBehaviour
{
    [SerializeField] GameObject collectEffect, occupiedSpot;

    internal void DestroySelf()
    {
        Instantiate(collectEffect, transform.position, transform.rotation);
        Instantiate(occupiedSpot, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
