using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OilCraterGenerator : MonoBehaviour
{
    [SerializeField] private float spawningTime;
    private Timer timer;

    [SerializeField] private float minX, maxX, minZ, maxZ;

    void Start()
    {
        timer = new Timer();
        timer.SetStartTime(spawningTime);
    }

    void Update()
    {
        timer.Tick();
        if(timer.CurrentTime <= 0)
        {
            PhotonNetwork.Instantiate("MachineGame/OilCrater", new Vector3(Random.Range(minX,maxX), 5.1f, Random.Range(minZ,maxZ)), Quaternion.identity);
            timer.ResetTimer();
        }
    }
}
