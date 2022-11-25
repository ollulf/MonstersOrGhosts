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

    private WorldTime world;

    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (base.photonView.IsMine)
        {
            MovePlayer();
            TurnIce();
            IceView(world.IceYear);
        }

    }

    private void MovePlayer() => rigidbody.AddRelativeForce(Vector3.forward * Vertical() * movementSpeed);

    private void TurnIce() => transform.Rotate(0, Horizontal() * turnspeed, 0);

    private float Horizontal() => Input.GetAxisRaw("Horizontal");

    private float Vertical() => Input.GetAxisRaw("Vertical");

    private void IceView(int newYear)
    {
        if(newYear >= 1980 && newYear < 2000)
        {
            volume.profile = volume1980;
        }
        if(newYear >= 2000 && newYear < 2015)
        {
            volume.profile = volume2000;
        }
        if(newYear >= 2015 && newYear < 2050)
        {
            volume.profile = volume2015;
        }
        if(newYear >= 2050 && newYear <2100)
        {
            volume.profile = volume2050;
        }
        if(newYear >= 2100)
        {
            volume.profile = volume2100;
        }
    }

    public void GetWorldTime(WorldTime newWorld)
    {
        world = newWorld;
    }
}
