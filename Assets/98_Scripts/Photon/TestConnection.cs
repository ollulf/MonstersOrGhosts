using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestConnection : MonoBehaviourPunCallbacks
{
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
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(DisconnectCause cause) => Debug.LogWarning($"Disconnected from server. Reason: {cause}");
}
