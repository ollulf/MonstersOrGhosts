using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomListing : MonoBehaviour
{
    [SerializeField] Text text;

    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomName(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        text.text = roomInfo.Name;
    }

    public void OnClick() => PhotonNetwork.JoinRoom(RoomInfo.Name);
}
