using UnityEngine;

[CreateAssetMenu(menuName = "Manager/GameSettings")]
public class GameSettings : ScriptableObject
{

    [SerializeField] private string gameVersion = "0.0.0";
    public string GameVersion { get => gameVersion; }

    [SerializeField] private string nickName = "Meltdragon";
    public string NickName
    {
        get
        {
            int value = Random.Range(0, 9999);
            return nickName + value.ToString();
        }
    }
}
