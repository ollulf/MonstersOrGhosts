using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCanvas : MonoBehaviour
{
    [SerializeField] private GameObject createJoinRoom;
    [SerializeField] private GameObject room;

    public void CreateOrJoinRoom()
    {
        createJoinRoom.SetActive(false);
        room.SetActive(true);
    }

    public void LeaveRoom()
    {
        createJoinRoom.SetActive(true);
        room.SetActive(false);
    }
}
