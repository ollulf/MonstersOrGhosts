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
    private float motionFloat, dayFloat, monthFloat, indexFloat;
    private Animator anim;

    private int dayInt = 1, monthInt = 1;

    void Start()
    {
        anim = GetComponent<Animator>();
        timer = new Timer();
        motionFloat = 1f / maxYears;
        timer.SetStartTime(0, false);
        monthFloat = oneYearInSeconds / 12f;
        dayFloat = monthFloat / 30f;
        Debug.LogError(dayFloat);
        indexFloat = 0;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timer.Tick();

            YearMonthDayCalculater();
            showTimer.text = years.ToString();

            photonView.RPC("UpdateUI", RpcTarget.All, years, monthInt, dayInt);
            if (years >= maxYears)
            {
                photonView.RPC("LevelLoad", RpcTarget.All);
            }
        }
    }

    private void LateUpdate()
    {
        anim.SetFloat("Time", motionFloat * years);
    }

    private void YearMonthDayCalculater()
    {
        years = Mathf.RoundToInt(timer.CurrentTime / oneYearInSeconds);
    }

    [PunRPC]
    private void UpdateUI(int newYear, int newMonth, int newDay)
    {
        showTimer.text = newDay + "_" + newMonth + "_" + (1950 + newYear);
    }

    [PunRPC]

    private void LevelLoad()
    {
        PhotonNetwork.LoadLevel(1);
    }

}
