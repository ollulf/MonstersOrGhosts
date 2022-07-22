using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OilCraterGenerator : MonoBehaviour
{
    [SerializeField] private float spawningTime;
    [SerializeField] private int amountOfOilFields;

    [SerializeField] private float minX, maxX, minZ, maxZ;

    private Timer timer;
    private int index;

    void Start()
    {
        index = 0;
        timer = new Timer();
        timer.SetStartTime(spawningTime);
    }

    void Update()
    {
        if (index < amountOfOilFields)
        {
            timer.Tick();
            if (timer.CurrentTime <= 0)
            {
                index++;
                PhotonNetwork.Instantiate("MachineGame/OilCrater", new Vector3(Random.Range(minX, maxX), 5.1f, Random.Range(minZ, maxZ)), Quaternion.identity);
                timer.ResetTimer();
            }
        }
    }
}
