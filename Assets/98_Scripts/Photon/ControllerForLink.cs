using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ControllerForLink : MonoBehaviourPunCallbacks
{
    [SerializeField] private float timerTime;
    [SerializeField] private int test;

    private Timer timer;

    private bool isLoading;

    // Start is called before the first frame update
    void Start()
    {
        isLoading = false;
        timer = new Timer();
        timer.SetStartTime(timerTime, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount <= test)
        {
            timer.Tick();
            Debug.LogError(timer.CurrentTime);
            if (timer.CurrentTime <= 0 && !isLoading)
            {
                isLoading = true;
                PhotonNetwork.LoadLevel(2);
            }
        }
    }
}
