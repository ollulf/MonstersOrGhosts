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
    private Rigidbody rigidbody;

    [ShowNonSerializedField] private int food;
    [ShowNonSerializedField] private int poisen;
    [ShowNonSerializedField] private int population;

    [SerializeField] private TextMeshProUGUI populationTextField;

    private Timer timer;

    [SerializeField] private float yMax = -1, yMin = -5, hoverSpeed = 2f;
    private bool goesUp = true;
    private float volumeDif;
    [SerializeField] private GameObject fogVolume1, fogVolume2, fogVolume3, fogVolume4, fogVolume5, fogVolume6, fogVolume7;
    [ShowNonSerializedField] private GameObject activeFog;

    public int Population { get => population; }


    private void Start()
    {
        timer = new Timer();
        timer.SetStartTime(0, false);
        rigidbody = GetComponent<Rigidbody>();
        population = FirstDataGive.FishStartPopulation;
        PlayerBaseDataHandler.SetFish(this);
        PlayerBaseDataHandler.RaiseBirdFood(population);

        volumeDif = (yMax - yMin) / 7;

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

        timer.Tick();
        if(timer.CurrentTime >= 1)
        {
            ReducePopulation(FirstDataGive.FishLoss);
            timer.ResetTimer();
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
            MoveUpDown();
        }
    }

    private void MoveUpDown()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            Vector3 t  = transform.position;
            goesUp = (t.y < yMin && !goesUp || t.y < yMax && goesUp) ? true : false;
            transform.position = goesUp ? new Vector3(t.x, transform.position.y + 0.1f * hoverSpeed, t.z) 
                : new Vector3(t.x, transform.position.y - 0.1f * hoverSpeed, t.z);
        }

        //UpdateVolumes();

    }

    private void UpdateVolumes()
    {
        fogVolume1.SetActive(false);
        fogVolume2.SetActive(false);
        fogVolume3.SetActive(false);
        fogVolume4.SetActive(false);
        fogVolume5.SetActive(false);
        fogVolume6.SetActive(false);
        fogVolume7.SetActive(false);

        GetActiveVolume().SetActive(true);
        activeFog = GetActiveVolume();
    }

    private GameObject GetActiveVolume()
    {
        float t = transform.position.y;
        if (t < yMax - 6 * volumeDif) return fogVolume7;
        else if (t < yMax - 5 * volumeDif) return fogVolume6;
        else if (t < yMax - 4 * volumeDif) return fogVolume5;
        else if (t < yMax - 3 * volumeDif) return fogVolume4;
        else if (t < yMax - 2 * volumeDif) return fogVolume3;
        else if (t < yMax - 1 * volumeDif) return fogVolume2;
        else return fogVolume1;
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
            rigidbody.AddRelativeForce(transform.forward * -movement.y * movementSpeed);
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

    public void FeedFish(int newPopulation)
    {
        population += newPopulation;
        PlayerBaseDataHandler.RaiseBirdFood(1);
        AddFlock();
    }

    public void ReducePopulation(int newPop)
    {
        population -= newPop;
        PlayerBaseDataHandler.ReduceBirdFood(newPop);
    }

    private void AddFlock()
    {
        if (GlobalFlock.FishFlock.Count <= 20)
        {
            GameObject flocki = PhotonNetwork.Instantiate("FishGame/flock", this.transform.position, Quaternion.identity);
            flocki.GetComponent<FalseFlock>().SetTarget(this.gameObject);
        }
    }

    public bool CheckMovement()
    {
        return movement.y > 0;
    }

}

public enum Danger
{
    VeryLow,
    low,
    normal,
    high,
    ExtremHigh
}