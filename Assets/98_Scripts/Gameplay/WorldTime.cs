using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using Photon.Pun;

public class WorldTime : Singleton<WorldTime>
{
    [SerializeField] private int maxYears, oneYearInSeconds;
    [SerializeField] private TextMeshProUGUI showTimer;

    private int years;
    private Timer timer;

    public static int OneYearInSeconds { get => Instance.oneYearInSeconds;}


    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();

        timer.SetStartTime(0, false);
    }

    // Update is called once per frame
    void Update()
    {
        timer.Tick();

        years = Mathf.RoundToInt(timer.CurrentTime / oneYearInSeconds);

        showTimer.text = years.ToString();

        if(years >= maxYears)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(1);
            }
        }
    }
}
