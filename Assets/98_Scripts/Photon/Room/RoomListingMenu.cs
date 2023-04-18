using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject roomListing;

    private List<GameObject> listings = new List<GameObject>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Let's update the List");
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = listings.FindIndex(x => x.GetComponent<RoomListing>().RoomInfo.Name == info.Name);
                if (index != -1)
                {
                    Destroy(listings[index]);
                    listings.RemoveAt(index);
                }
            }
            else
            {
                int index = listings.FindIndex(x => x.GetComponent<RoomListing>().RoomInfo.Name == info.Name);
                if (index == -1)
                {
                    GameObject listing = Instantiate(roomListing, content);
                    if (listing != null)
                    {
                        listing.GetComponent<RoomListing>().SetRoomName(info);
                        listings.Add(listing);
                    }
                }
                else
                {

                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        transform.root.GetComponent<LobbyCanvas>().CreateOrJoinRoom();
        content.DestroyChildren();
        listings.Clear();
    }
}
