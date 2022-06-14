using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterManager : Singleton<MasterManager>
{
    [SerializeField] private GameSettings gameSetting;
    public static GameSettings GameSetting { get => Instance.gameSetting; }
}
