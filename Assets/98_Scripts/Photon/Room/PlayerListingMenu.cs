using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject playerListing;
    [SerializeField] private GameObject button;

    private List<GameObject> listings = new List<GameObject>();



    public override void OnEnable()
    {
        base.OnEnable();
        SetInActive();
        GetAllPlayer();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        for(int i=0;i < listings.Count; i++)
        {
            Destroy(listings[i].gameObject);
        }
        listings.Clear();
    }

    private void GetAllPlayer()
    {
        if(!PhotonNetwork.IsConnected)
        {
            return;
        }

        if(PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
        {
            return;
        }
        foreach(KeyValuePair<int,Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playerInfo.Value);
        }
    }

    private void AddPlayerListing(Player newPlayer)
    {
        int index = listings.FindIndex(x => x.GetComponent<PlayerListing>().playerInfo == newPlayer);
        if (index != -1)
        {
            listings[index].GetComponent<PlayerListing>().SetPlayerInfo(newPlayer);
        }
        else
        {
            GameObject listing = Instantiate(playerListing, content);
            if (listing != null)
            {
                listing.GetComponent<PlayerListing>().SetPlayerInfo(newPlayer);
                listings.Add(listing);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = listings.FindIndex(x => x.GetComponent<PlayerListing>().playerInfo == otherPlayer);
        if (index != -1)
        {
            Destroy(listings[index]);
            listings.RemoveAt(index);
        }
    }

    public void OnClickStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(1);
        }
    }

    private void SetInActive()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            button.SetActive(false);
        }
    }
}