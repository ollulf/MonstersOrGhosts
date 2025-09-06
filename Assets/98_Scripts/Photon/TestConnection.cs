using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestConnection : MonoBehaviourPunCallbacks
{
    [SerializeField] private SinglePlayerRoom _room;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Server");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 5;
        PhotonNetwork.NickName = MasterManager.GameSetting.NickName;
        PhotonNetwork.GameVersion = MasterManager.GameSetting.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        _room.Init();
    }

    public override void OnDisconnected(DisconnectCause cause) => Debug.LogWarning($"Disconnected from server. Reason: {cause}");
}
