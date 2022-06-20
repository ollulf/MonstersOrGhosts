using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class PlayerListing : MonoBehaviour
{
    [SerializeField] Text text;

    public Player playerInfo { get; private set; }

    public void SetPlayerInfo(Player player)
    {
        playerInfo = player;
        text.text = playerInfo.NickName;
    }
}
