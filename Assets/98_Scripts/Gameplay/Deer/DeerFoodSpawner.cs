using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeerFoodSpawner : MonoBehaviourPun
{

    [SerializeField] float spawningTime;

    private List<Transform> foodSpot;
    private Timer timer;


    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();
        timer.SetStartTime(spawningTime);
        foodSpot = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            foodSpot.Add(transform.GetChild(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer.Tick();
        if (timer.CurrentTime <= 0)
        {
            SpawningFoodSpot();
            timer.ResetTimer();
        }

    }

    private void SpawningFoodSpot()
    {
        foreach (Transform child in foodSpot)
        {
            if (child.childCount == 0)
            {
                GameObject breedingSpot = PhotonNetwork.Instantiate("DeerGame/DeerFoodSource", child.transform.position, Quaternion.identity);
                breedingSpot.transform.parent = child.transform;
                return;
            }
        }
    }

}
