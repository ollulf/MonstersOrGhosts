using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class AcetateSpawner : MonoBehaviour
{
    [MinMaxSlider(0f, 20f)]
    [SerializeField] private Vector2 minMaxSpawnTime;
    [SerializeField] private GameObject acetate;

    private List<Transform> spawner;

    private Timer timer;


    private float randomTime;

    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();
        timer.SetStartTime(0, true);
        spawner = new List<Transform>();
        foreach (Transform child in transform)
        {
            spawner.Add(child);
        }

        randomTime = Random.Range(minMaxSpawnTime.x, minMaxSpawnTime.y);
    }

    // Update is called once per frame
    void Update()
    {
        timer.Tick();
        if (timer.CurrentTime >= randomTime)
        {
            //int spawn = Random.Range(1,GetChildren());

            //for (int i = 0; i < spawn; i++)
            //{
            Instantiate(acetate, spawner[Random.Range(0, spawner.Count)]);
            //}
            randomTime = Random.Range(minMaxSpawnTime.x, minMaxSpawnTime.y);
        }
    }

    public int GetChildren()
    {
        return transform.childCount;
    }
}
