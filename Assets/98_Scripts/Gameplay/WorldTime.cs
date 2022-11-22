using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using Photon.Pun;

public class WorldTime : MonoBehaviourPun
{
    [SerializeField] private int maxYears;
    [SerializeField] private float oneYearInSeconds;
    [SerializeField] private TextMeshProUGUI showYear, showDay;
    [SerializeField] private Animator animYear, animMonth;

    private int years, month = 1, day = 1, lastYear;
    private Timer timer, timer2;
    private float motionFloat, dayFloat, monthFloat, motionFloat2, indexFloat;

    public int Years { get => years; }

    void Start()
    {
        timer = new Timer();
        timer2 = new Timer();
        motionFloat = 1f / maxYears;
        motionFloat2 = 1f / 12f;
        timer.SetStartTime(0, false);
        timer2.SetStartTime(0, false);
        monthFloat = oneYearInSeconds / 12f;
        dayFloat = monthFloat / 30f;
        indexFloat = 0;
        lastYear = years;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timer.Tick();
            timer2.Tick();

            if (years != lastYear)
            {
                timer2.ResetTimer();
                month = 1;
                lastYear = years;
            }

            YearMonthDayCalculater();

            photonView.RPC("UpdateUI", RpcTarget.All, years, day);
            if (years >= maxYears)
            {
                CallInEndValues.SetValues();
            }
        }
    }

    private void LateUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateAnimator", RpcTarget.All, years, month);
        }
    }

    private void YearMonthDayCalculater()
    {
        years = Mathf.RoundToInt(timer.CurrentTime / oneYearInSeconds);
        month = Mathf.RoundToInt(timer2.CurrentTime / monthFloat);

        indexFloat += Time.deltaTime;
        if (indexFloat >= dayFloat)
        {
            day++;
            indexFloat = 0;
            if (day >= 30)
            {
                day = 1;
            }
        }
    }

    [PunRPC]
    private void UpdateUI(int newYear, int newDay)
    {
        showYear.text = (1990 + newYear).ToString();
        if (day < 10)
        {
            showDay.text = "0" + newDay.ToString();

        }
        else
        {
            showDay.text = newDay.ToString();
        }
    }

    [PunRPC]
    private void UpdateAnimator(int newYear, int newMonth)
    {
        animYear.SetFloat("Time", motionFloat * newYear);
        animMonth.SetFloat("Time", motionFloat2 * newMonth);
    }

    [PunRPC]
    private void LevelLoad()
    {
        PhotonNetwork.LoadLevel(1);
    }

}
