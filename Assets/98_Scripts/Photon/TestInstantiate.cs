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

        Charakter newCharakter = (Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"];

        switch (newCharakter)
        {
            case Charakter.Bird:
                {
                    PhotonNetwork.Instantiate("Bird", position, transform.rotation);
                    break;
                }
            case Charakter.Fish:
                {
                    PhotonNetwork.Instantiate("Fish", position, Quaternion.Euler(90,0,0));
                    break;
                }
        }

    }
}
