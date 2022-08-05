using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BreedingSpotsSpawner : MonoBehaviourPun
{
    [SerializeField] float spawningTime;

    private List<Transform> breedingSpots;
    private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();
        timer.SetStartTime(spawningTime, true);
        breedingSpots = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            breedingSpots.Add(transform.GetChild(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer.Tick();
        if (timer.CurrentTime <= 0)
        {
            SpawningBreedingSpot();
            timer.ResetTimer();
        }
    }

    private void SpawningBreedingSpot()
    {
        foreach (Transform child in breedingSpots)
        {
            if (child.childCount == 0)
            {
                GameObject breedingSpot = PhotonNetwork.Instantiate("BirdGame/BirdBreedingSpot", child.transform.position, Quaternion.identity);
                breedingSpot.transform.parent = child.transform;
                return;
            }
        }
    }

}
