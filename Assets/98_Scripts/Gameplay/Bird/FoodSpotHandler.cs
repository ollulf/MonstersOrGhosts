using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using NaughtyAttributes;

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

            if (PlayerBaseDataHandler.GetBirdFood() == 0)
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

#if UNITY_EDITOR

    [Button]
    private void CreatePlace()
    {
        GameObject place = new GameObject("Bird Food Spot");
        place.transform.parent = this.gameObject.transform;

        DrawIcon(place, 4);
        UnityEditorInternal.ComponentUtility.CopyComponent(this);
        UnityEditorInternal.ComponentUtility.PasteComponentValues(this);
    }


    private void DrawIcon(GameObject gameObject, int idx)
    {
        GUIContent[] icons = GetTextures("sv_label_", string.Empty, 0, 8);
        GUIContent icon = icons[idx];
        EditorGUIUtility.SetIconForObject(gameObject, (Texture2D)icon.image);
    }

    private GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
    {
        GUIContent[] Iconarray = new GUIContent[count];
        for (int i = 0; i < count; i++)
        {
            Iconarray[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
        }
        return Iconarray;
    }
#endif
}
