using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaFoodHandler : Singleton<BacteriaFoodHandler>
{
    private List<GameObject> food;

    public static List<GameObject> Food { get => Instance.food;}

    protected override void Awake()
    {
        base.Awake();
        food = new List<GameObject>();
    }

    public static void AddToList(GameObject newFood) => Instance.food.Add(newFood);

    public static void RemoveFromList(GameObject newFood) => Instance.food.Remove(newFood);

    public static GameObject GetFood() => Instance.food[Random.Range(0, Instance.food.Count)];
}
