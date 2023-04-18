using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : Singleton<GlobalFlock>
{
    private List<Transform> fishFlock;

    public static List<Transform> FishFlock => Instance.fishFlock;

    void Start() => fishFlock = new List<Transform>();

    private void Update()
    {
        if (PlayerBaseDataHandler.FishPopulation != null)
        {
            CheckFishFlock();
        }
    }

    private void CheckFishFlock()
    {
        if(PlayerBaseDataHandler.FishPopulation.Population < fishFlock.Count)
        {
            fishFlock[fishFlock.Count - 1].GetComponent<Flocking>().DestroyFlock();
            fishFlock.RemoveAt(fishFlock.Count - 1);
            foreach(Transform fish in fishFlock)
            {
                fish.GetComponent<Flocking>().SetFlock(FishFlock);
            }
        }
    }

    public static void AddFishFlock(Transform newFish)
    {
        if(!FishFlock.Contains(newFish))
        {
            Instance.fishFlock.Add(newFish);
        }            
    }
}