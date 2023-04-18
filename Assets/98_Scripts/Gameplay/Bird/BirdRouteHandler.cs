public class BirdRouteHandler : Singleton<BirdRouteHandler>
{
    private WayPointPlacingSystem birdRoute;

    public static WayPointPlacingSystem BirdRoute => Instance.birdRoute;

    void Start() => birdRoute = transform.GetChild(2).GetComponent<WayPointPlacingSystem>();
}