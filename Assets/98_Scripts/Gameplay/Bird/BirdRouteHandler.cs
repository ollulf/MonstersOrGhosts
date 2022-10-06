using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdRouteHandler : Singleton<BirdRouteHandler>
{
    private WayPointPlacingSystem birdRoute;

    public static WayPointPlacingSystem BirdRoute { get => Instance.birdRoute;}

    void Start()
    {
        birdRoute = transform.GetChild(2).GetComponent<WayPointPlacingSystem>();
    }

}
