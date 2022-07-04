using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdFood : MonoBehaviour
{
    public int foodAmount = 15;
    private SphereCollider clickCollider;

    [SerializeField] GameObject collectEffect;
    // Start is called before the first frame update
    void Start()
    {
        clickCollider = GetComponent<SphereCollider>();
    }


    internal void DestroySelf()
    {
        Instantiate(collectEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
