using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FoodSpotHandler : MonoBehaviourPun
{
    private List<Transform> foodSpots;


    void Start()
    {
        foodSpots = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            foodSpots.Add(transform.GetChild(i));
        }
    }

    void Update()
    {
        if (PlayerBaseDataHandler.GetBirdFood() > 0)
        {
            FillFoodSpawn();
        }
        if (PlayerBaseDataHandler.GetBirdFood() < 0)
        {
            ReduceFoodSpawn();
        }

    }

    private void FillFoodSpawn()
    {
        for (int i = 0; i < foodSpots.Count; i++)
        {
            if (foodSpots[i].childCount == 0 && PlayerBaseDataHandler.GetBirdFood() > 0)
            {
                GameObject food = PhotonNetwork.Instantiate("BirdGame/BirdFood", foodSpots[i].position, Quaternion.identity);
                food.transform.parent = foodSpots[i];
                if (PlayerBaseDataHandler.GetBirdFood() >= 15)
                {
                    food.GetComponent<BirdFood>().SetFoodAmount(15);
                    PlayerBaseDataHandler.ReduceBirdFood(15);
                }
                else
                {
                    food.GetComponent<BirdFood>().SetFoodAmount(PlayerBaseDataHandler.GetBirdFood());
                    PlayerBaseDataHandler.ReduceBirdFood(PlayerBaseDataHandler.GetBirdFood());
                    return;
                }
            }

            else if (foodSpots[i].childCount == 1 && PlayerBaseDataHandler.GetBirdFood() > 0 && foodSpots[i].GetChild(0).GetComponent<BirdFood>().FoodAmount < 15)
            {
                int index = 15 - foodSpots[i].GetChild(0).GetComponent<BirdFood>().FoodAmount;
                if (index < PlayerBaseDataHandler.GetBirdFood())
                {
                    foodSpots[i].GetChild(0).GetComponent<BirdFood>().SetFoodAmount(index);
                    PlayerBaseDataHandler.ReduceBirdFood(index);
                }
                else
                {
                    foodSpots[i].GetChild(0).GetComponent<BirdFood>().SetFoodAmount(PlayerBaseDataHandler.GetBirdFood());
                    PlayerBaseDataHandler.ReduceBirdFood(PlayerBaseDataHandler.GetBirdFood());
                    return;
                }
            }
            
            if(PlayerBaseDataHandler.GetBirdFood() == 0)
            {
                return;
            }
        }
    }

    private void ReduceFoodSpawn()
    {
        for (int i = 0; i < foodSpots.Count; i++)
        {
            if (foodSpots[i].childCount == 1 && PlayerBaseDataHandler.GetBirdFood() < 0)
            {
                int index = PlayerBaseDataHandler.GetBirdFood() * -1;


                if (foodSpots[i].GetChild(0).GetComponent<BirdFood>().FoodAmount > index)
                {
                    foodSpots[i].GetChild(0).GetComponent<BirdFood>().SetFoodAmount(PlayerBaseDataHandler.GetBirdFood());
                    PlayerBaseDataHandler.RaiseBirdFood(index);
                    return;
                }
                else
                {
                    if (foodSpots[i].GetChild(0).GetComponent<BirdFood>().FoodAmount == index)
                    {
                        foodSpots[i].GetChild(0).GetComponent<BirdFood>().SetFoodAmount(PlayerBaseDataHandler.GetBirdFood());
                        PlayerBaseDataHandler.RaiseBirdFood(index);
                        PhotonNetwork.Destroy(foodSpots[i].GetChild(0).gameObject);
                        return;
                    }
                    else
                    {
                        int negativ = foodSpots[i].GetChild(0).GetComponent<BirdFood>().FoodAmount * -1;
                        foodSpots[i].GetChild(0).GetComponent<BirdFood>().SetFoodAmount(negativ);
                        index = negativ * -1;
                        PlayerBaseDataHandler.RaiseBirdFood(index);
                        PhotonNetwork.Destroy(foodSpots[i].GetChild(0).gameObject);
                    }
                }

            }
        }
    }
}
