using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class PlayerListing : MonoBehaviourPunCallbacks
{
    [SerializeField] Text text;

    public Player playerInfo { get; private set; }


    public void SetPlayerInfo(Player player)
    {
        playerInfo = player;
        text.text = (Charakter)player.CustomProperties["PlayerCharakter"] + "";
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (targetPlayer != null && targetPlayer == playerInfo)
        {
            if (changedProps.ContainsKey("PlayerCharakter"))
            {
                SetPlayerInfo(targetPlayer);
            }
        }
    }
}
