using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering;

public class IceController : MonoBehaviourPun
{
    [SerializeField] private float movementSpeed, turnspeed;

    [SerializeField] private VolumeProfile volumeFirst, volume1980, volume2000, volume2015, volume2050, volume2100;

    [SerializeField]private Volume volume;
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (base.photonView.IsMine)
        {
            MovePlayer();
            TurnIce();
        }
    }

    private void MovePlayer()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * Vertical() * movementSpeed);
    }

    private void TurnIce()
    {
        transform.Rotate(0, Horizontal() * turnspeed, 0);
    }

    private float Horizontal()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    private float Vertical()
    {
        return Input.GetAxisRaw("Vertical");
    }
}
