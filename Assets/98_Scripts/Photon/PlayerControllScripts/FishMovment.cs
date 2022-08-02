using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class FishMovment : MonoBehaviourPun
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float turnspeed;
    private Vector2 movement;

    [ShowNonSerializedField] private int food;
    [ShowNonSerializedField] private int poisen;
    [ShowNonSerializedField] private int population = 30;

    public int Population { get => population;}

    private void Start()
    {
        PlayerBaseDataHandler.SetFish(this);
        PlayerBaseDataHandler.RaiseBirdFood(population);
    }

    void Update()
    {
        if (base.photonView.IsMine)
        {
            GetAxis();
        }
    }

    private void FixedUpdate()
    {
        if (base.photonView.IsMine)
        {
            MovePlayer();
            TurnFish();
        }
    }

    private void GetAxis()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Mathf.Clamp(Input.GetAxisRaw("Vertical"), 0, 1);

    }
    private void MovePlayer()
    {
        transform.position += transform.up * movement.y * movementSpeed;
    }

    private void TurnFish()
    {
        transform.Rotate(0, 0, -movement.x * turnspeed);
    }

    public void FeedFish(int newFood)
    {
        food += newFood;
        if (food >= 100)
        {
            food = 0;
            population += 1;
            PlayerBaseDataHandler.RaiseBirdFood(1);
        }
    }

    public void PoisenFish(int newPoisen)
    {
        poisen += newPoisen;
        if (poisen >= 100)
        {
            poisen = 0;
            ReducePopulation(1);
            PlayerBaseDataHandler.ReduceBirdFood(1);
        }
    }

    public void ReducePopulation(int newPop)
    {
        population -= newPop;
    }
}
