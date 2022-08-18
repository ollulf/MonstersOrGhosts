using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using Photon.Pun;

public class WorldTime : MonoBehaviourPun
{
    [SerializeField] private int maxYears, oneYearInSeconds;
    [SerializeField] private TextMeshProUGUI showTimer;

    private int years;
    private Timer timer;

    void Start()
    {
        timer = new Timer();

        timer.SetStartTime(0, false);
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timer.Tick();

            years = Mathf.RoundToInt(timer.CurrentTime / oneYearInSeconds);

            showTimer.text = years.ToString();
            photonView.RPC("UpdateUI", RpcTarget.All, years);
            if (years >= maxYears)
            {
                photonView.RPC("LevelLoad", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void UpdateUI(int newYear)
    {
        showTimer.text = newYear.ToString();
    }

    [PunRPC]

    private void LevelLoad()
    {
        PhotonNetwork.LoadLevel(1);
    }

}
