using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;
using System;
using TMPro;

public class FishMovment : MonoBehaviourPun
{
    private Transform motherFlock;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float turnspeed, fishPrefTurnSpeed;
    [SerializeField] private Transform fishPref;
    private Vector2 movement;

    [ShowNonSerializedField] private int food;
    [ShowNonSerializedField] private int poisen;
    [ShowNonSerializedField] private int population;

    [SerializeField] private TextMeshProUGUI populationTextField;

    public int Population { get => population; }


    private void Start()
    {
        population = FirstDataGive.FishStartPopulation;
        PlayerBaseDataHandler.SetFish(this);
        PlayerBaseDataHandler.RaiseBirdFood(population);


        for (int i = 0; i < 20; i++)
        {
            AddFlock();
        }
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
        UpdateUI();

        if (base.photonView.IsMine)
        {
            MovePlayer();
            TurnFish();
            TurnFishPref();
        }
    }

    private void UpdateUI()
    {
        populationTextField.text = "" + population;
    }

    private void GetAxis()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.W))
        {
            movement.y = Mathf.Clamp(Input.GetAxisRaw("Vertical"), 0, 1);
        }
        else
        {
            if (movement.y > 0)
            {
                movement.y -= Time.deltaTime;
            }
            else
            {
                movement.y = 0;
            }
        }

    }
    private void MovePlayer()
    {
        transform.position += transform.up * movement.y * movementSpeed;
    }

    private void TurnFish()
    {
        transform.Rotate(0, 0, -movement.x * turnspeed);
    }

    private void TurnFishPref()
    {
        Quaternion tempEulerAngles = Quaternion.Euler(gameObject.transform.eulerAngles.x - 90, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);


        Quaternion rotation = Quaternion.Lerp(Quaternion.Euler(tempEulerAngles.eulerAngles), Quaternion.Euler(tempEulerAngles.eulerAngles.x, tempEulerAngles.eulerAngles.y + movement.x * 20, tempEulerAngles.eulerAngles.z), Time.deltaTime * fishPrefTurnSpeed);
        fishPref.transform.eulerAngles = rotation.eulerAngles;
    }

    public void FeedFish(int newFood)
    {
        food += newFood;
        if (food >= 100)
        {
            food = 0;
            population += 1;
            PlayerBaseDataHandler.RaiseBirdFood(1);
            AddFlock();
        }
    }

    public void PoisenFish(int newPoisen)
    {
        poisen += newPoisen;
        if (poisen >= 100)
        {
            poisen = 0;
            if (population > 0)
            {
                ReducePopulation(1);
                PlayerBaseDataHandler.ReduceBirdFood(1);
            }
        }
    }

    public void ReducePopulation(int newPop)
    {
        population -= newPop;
    }

    private void AddFlock()
    {
        if (GlobalFlock.FishFlock.Count <= 20)
        {
            GameObject flocki = PhotonNetwork.Instantiate("FishGame/flock", this.transform.position, Quaternion.identity);
            flocki.GetComponent<Flocking>().SetTarget(this.transform);
            flocki.GetComponent<Flocking>().SetMotherFlock(GlobalFlock.Instance.transform);
            flocki.transform.parent = GlobalFlock.Instance.transform;
        }
    }

    public bool CheckMovement()
    {
        return movement.y > 0;
    }
}
