using UnityEngine;
using Photon.Pun;

public class ControllerForLink : MonoBehaviourPunCallbacks
{
    [SerializeField] private float timerTime;

    private Timer timer;

    private bool isLoading;

    void Start()
    {
        isLoading = false;
        timer = new Timer(timerTime, true);
    }

    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            timer.Tick();
            if (timer.CurrentTime <= 0 && !isLoading)
            {
                isLoading = true;
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.LoadLevel(2);
            }
        }
    }
}
