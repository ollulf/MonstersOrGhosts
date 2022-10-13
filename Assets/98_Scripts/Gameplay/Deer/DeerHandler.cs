using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerHandler :Singleton<DeerHandler>
{
    [SerializeField] private List<GameObject> deerFollower;

    public static void SetTarget(GameObject newTarget)
    {
        foreach(GameObject deer in Instance.deerFollower)
        {
            deer.GetComponent<Deer>().SetTarget(newTarget);
        }
    }
}
