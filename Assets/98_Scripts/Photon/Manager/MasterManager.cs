using UnityEngine;

public class MasterManager : Singleton<MasterManager>
{
    [SerializeField] private GameSettings gameSetting;
    public static GameSettings GameSetting => Instance.gameSetting;
}
