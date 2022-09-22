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
    private float motionFloat;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        timer = new Timer();
        motionFloat = 1f / maxYears;
        timer.SetStartTime(0, false);
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timer.Tick();

            years = Mathf.RoundToInt(timer.CurrentTime / oneYearInSeconds);

            anim.SetFloat("Time", motionFloat * years);

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
