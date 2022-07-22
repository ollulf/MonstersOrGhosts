using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using TMPro;

public class BirdController : MonoBehaviour
{
    public int amounOfFoodNeededForNest = 10;
    public int birdPopulationIncreaseAmount = 10;
    
    [ShowNonSerializedField]private int birdFood= 0, birdPopulation = 100;
    [SerializeField] public TextMeshProUGUI foodAmount, population;

    public int BirdPopulation { get => birdPopulation;}


    void Start()
    {
        PlayerBaseDataHandler.SetBird(this);
    }

    void Update()
    {
        CheckforClick();
        UpdateUI();

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
                    if (TryAddToBirdFood(hit.collider.gameObject.GetComponent<BirdFood>().foodAmount))
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
}
