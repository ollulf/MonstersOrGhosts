using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraMovement : MonoBehaviourPun
{
    [SerializeField] private float dragSpeed = 15;
    [SerializeField] private GameObject scinceSpot, sciencesButton;
    private Vector3 dragOrigin;

    public GameObject SciencesButton => sciencesButton;


    void Update()
    {
        DragMove();
    }

    private void DragMove()
    {


        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(2)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.y * dragSpeed, 0, -pos.x * dragSpeed);

        transform.Translate(move, Space.World);
        scinceSpot.transform.Translate(move, Space.World);
    }
}
