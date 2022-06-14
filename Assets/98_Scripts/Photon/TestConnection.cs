using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestConnection : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Server");
        PhotonNetwork.NickName = MasterManager.GameSetting.NickName;
        PhotonNetwork.GameVersion = MasterManager.GameSetting.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Disconnected from server. Reason: " + cause);
    }
}
