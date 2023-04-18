using System.Collections.Generic;
using UnityEngine;

public class WayPointHandler : Singleton<WayPointHandler>
{
    [SerializeField] List<WayPointPlacingSystem> wayPoints;

    public static List<WayPointPlacingSystem> WayPoints { get => Instance.wayPoints; }

}
