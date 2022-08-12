using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraMovement : MonoBehaviourPun
{
    [SerializeField] float scrollSpeed;
    private int horizontal, vertical;
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetAxis();
        Move();
    }

    private void SetAxis()
    {
        horizontal = 0;
        vertical = 0;

        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Machine)
        {
            if (Input.GetKey(KeyCode.W))
            {
                vertical = -1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                vertical = 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                horizontal = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                horizontal = 1;
            }
        }

    }

    private void Move()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + vertical * scrollSpeed * Time.deltaTime, gameObject.transform.position.y, gameObject.transform.position.z + horizontal * scrollSpeed * Time.deltaTime);
    }
}
