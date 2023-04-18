using UnityEngine;
using Photon.Pun;

public class LeaveRoomMenu : MonoBehaviour
{
    [SerializeField] private LobbyCanvas lobbyCanvas;

    public void OnClick()
    {
        PhotonNetwork.LeaveRoom(true);
        lobbyCanvas.LeaveRoom();
    }
}
