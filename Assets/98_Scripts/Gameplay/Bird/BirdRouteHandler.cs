using UnityEngine;

public class BirdRouteHandler : Singleton<BirdRouteHandler>
{
    [SerializeField] private WayPointPlacingSystem birdRoute;

    public static WayPointPlacingSystem BirdRoute => Instance.birdRoute;
    }