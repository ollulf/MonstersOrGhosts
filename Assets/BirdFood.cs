using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class BirdFood : MonoBehaviourPun
{
    [ShowNonSerializedField]
    private int foodAmount;
    private SphereCollider clickCollider;
    [SerializeField ]private Animator anim;

    public int FoodAmount { get => foodAmount;}

    void Start()
    {
        clickCollider = GetComponent<SphereCollider>();
        anim.Play("FishJump",-1,UnityEngine.Random.Range(0f,1f));
    }

    internal void DestroySelf()
    {
        PlayerBaseDataHandler.FishPopulation.ReducePopulation(FoodAmount);
        PhotonNetwork.Instantiate("BirdGame/CollectEffect", transform.position, transform.rotation);
        PhotonNetwork.Destroy(gameObject);
    }

    public void SetFoodAmount(int newAmount)
    {
        foodAmount += newAmount;
    }
}
