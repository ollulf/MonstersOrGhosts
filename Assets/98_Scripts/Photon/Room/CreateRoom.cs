using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoom :MonoBehaviourPunCallbacks
{
    [SerializeField] private Text roomName;
    [SerializeField] private LobbyCanvas lobbyCanvas;

    public void OnClickCreateRoom()
    {
        if(!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Not Connected");
            return;
        }
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 6;

        PhotonNetwork.JoinOrCreateRoom(roomName.text, options, TypedLobby.Default);

    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room" + this);
        lobbyCanvas.CreateOrJoinRoom();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed" + message + this);
    }
}
