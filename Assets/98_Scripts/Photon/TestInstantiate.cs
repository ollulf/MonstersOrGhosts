using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestInstantiate : MonoBehaviour
{
    private void Awake()
    {
        Vector2 offset = Random.insideUnitCircle * 3f;
        Vector3 position = new Vector3(transform.position.x + offset.x, 0.76f, transform.position.z + offset.y);

        PhotonNetwork.Instantiate("TestMultiplayerPlayer", position, Quaternion.identity);
    }
}
