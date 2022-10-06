using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using TMPro;
using Photon.Pun;

public class BirdController : MonoBehaviourPun
{
    public int amounOfFoodNeededForNest = 10;
    public int birdPopulationIncreaseAmount;

    private List<GameObject> birdRoute;

    [ShowNonSerializedField]private int birdFood= 0, birdPopulation;
    [SerializeField] public TextMeshProUGUI foodAmount, population;
    [SerializeField] private float movementSpeed, turnSpeed, maxDistance;

    public int BirdPopulation { get => birdPopulation;}


    void Start()
    {
        birdPopulationIncreaseAmount = FirstDataGive.BirdPopulation;
        birdPopulation = FirstDataGive.BirdStartPopulation;
        PlayerBaseDataHandler.SetBird(this);
    }

    void Update()
    {
        if(birdRoute == null)
        {
            birdRoute = new List<GameObject>(BirdRouteHandler.BirdRoute.GetWayPoints());
        }
        CheckforClick();
        UpdateUI();
    }

    private void FixedUpdate()
    {
        if (birdRoute.Count != 0)
        {
            Move();
            if (CheckDistance())
            {
                RemoveFromList();
            }
        }
        else
        {
            birdRoute = new List<GameObject>(BirdRouteHandler.BirdRoute.GetWayPoints());
            transform.position = birdRoute[0].transform.position;
        }
    }

    private void UpdateUI()
    {
        foodAmount.SetText("" + birdFood);
        population.SetText("" + birdPopulation);

    }

    private void CheckforClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
           
            //Debug.DrawRay(ray.origin, ray.direction * 1000f , Color.yellow, Mathf.Infinity);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) 
            {
                Debug.Log(hit.collider.gameObject);

                if (hit.collider.gameObject.layer == 6) //6 = BirdFood Layer
                {
                    hit.collider.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
                    if (TryAddToBirdFood(hit.collider.gameObject.GetComponent<BirdFood>().FoodAmount))
                        hit.collider.gameObject.GetComponentInChildren<BirdFood>().DestroySelf();

                }
                else if (hit.collider.gameObject.layer == 7) //7 = BirdNest Layer
                {
                    if (TryBuildNest())
                        hit.collider.gameObject.GetComponentInChildren<BreedingSpot>().DestroySelf();
                }
            }
        }
    }
    
    public bool TryAddToBirdFood(int amount)
    {
        birdFood += amount;
        return true;
    }

    public bool TryBuildNest()
    {
        if (birdFood < amounOfFoodNeededForNest)
            return false;

        else
        {
            birdFood -= amounOfFoodNeededForNest;
            birdPopulation += birdPopulationIncreaseAmount;
            return true;
        }
    }

    private void Move()
    {
        Vector3 direction = ((birdRoute[0].transform.position) - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = lookRotation.eulerAngles;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), Time.fixedDeltaTime * turnSpeed);

        if (transform.rotation == Quaternion.Euler(new Vector3(rotation.x, rotation.y + 1, rotation.z)) || transform.rotation == Quaternion.Euler(new Vector3(rotation.x, rotation.y - 1, rotation.z))) ;
        {
            transform.position += direction * movementSpeed * Time.deltaTime;
        }
    }

    public void RemoveFromList()
    {
        birdRoute.Remove(birdRoute[0]);
    }

    private bool CheckDistance()
    {
        return Vector3.Distance(transform.position, birdRoute[0].transform.position) <= maxDistance;
    }

}
