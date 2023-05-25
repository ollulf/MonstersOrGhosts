using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CreateRoom :MonoBehaviourPunCallbacks
{
    [SerializeField] private Text roomName;
    [SerializeField] private LobbyCanvas lobbyCanvas;
    [SerializeField] private TextMeshProUGUI playerButton;

    private byte maxPlayerNumber = 7;

    public void OnClickChangePlayerNumber()
    {
        maxPlayerNumber++;
        if(maxPlayerNumber > 7)
        {
            maxPlayerNumber = 1;
        }

        playerButton.text = maxPlayerNumber.ToString();
    }

    public void OnClickCreateRoom()
    {
        if(!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Not Connected");
            return;
        }
        RoomOptions options = new RoomOptions();
        options.BroadcastPropsChangeToAll = true;
        options.MaxPlayers = maxPlayerNumber;

        PhotonNetwork.JoinOrCreateRoom(roomName.text, options, TypedLobby.Default);

    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Created Room {this}");
        lobbyCanvas.CreateOrJoinRoom();
    }
    public override void OnCreateRoomFailed(short returnCode, string message) => Debug.LogError($"Room creation failed {message} {this}");
}
