using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SinglePlayerRoom : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Not Connected");
            return;
        }
        RoomOptions options = new RoomOptions();
        options.BroadcastPropsChangeToAll = true;
        options.MaxPlayers = 1;

        PhotonNetwork.JoinOrCreateRoom("SinglePlayer", options, TypedLobby.Default);

    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Created Room {this}");
    }
}
