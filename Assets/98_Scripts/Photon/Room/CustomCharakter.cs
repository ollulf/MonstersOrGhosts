using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CustomCharakter : MonoBehaviourPunCallbacks
{
    private ExitGames.Client.Photon.Hashtable myCharakter = new ExitGames.Client.Photon.Hashtable();

    [SerializeField] private Text text;

    private Charakter playerCharakter;
    private int index = 0;

    private void Start()
    {
        SetProperties();
    }

    public void SetProperties()
    {
        if (PhotonNetwork.PlayerListOthers.Length != 0)
        {
            for (int i = 0; i < System.Enum.GetValues(typeof(Charakter)).Length; i++)
            {
                bool newCharakter = true;

                for (int j = 0; j < PhotonNetwork.PlayerListOthers.Length; j++)
                {
                    if (PhotonNetwork.PlayerListOthers[j].CustomProperties.ContainsValue(index))
                    {
                        newCharakter = false;
                        index++;
                        if (index >= System.Enum.GetValues(typeof(Charakter)).Length)
                        {
                            index = 0;
                        }
                        break;
                    }
                }
                if (newCharakter)
                {
                    break;
                }
            }
        }

        playerCharakter = (Charakter)index;
        myCharakter["PlayerCharakter"] = playerCharakter;
        PhotonNetwork.SetPlayerCustomProperties(myCharakter);
        text.text = playerCharakter.ToString();
    }

    public void OnClick()
    {
        index++;
        if (index >= System.Enum.GetValues(typeof(Charakter)).Length)
        {
            index = 0;
        }
        SetProperties();
    }
}

public enum Charakter
{
    Fish,
    Bird,
    Deer,
    Machine,
    Bacteria,
    Ice
}