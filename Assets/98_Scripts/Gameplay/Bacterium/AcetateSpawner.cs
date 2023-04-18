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

    void Start()
    {
        timer = new Timer(0, false);
        spawner = new List<Transform>();
        foreach (Transform child in transform)
        {
            spawner.Add(child);
        }

        randomTime = Random.Range(minMaxSpawnTime.x, minMaxSpawnTime.y);
    }

    void FixedUpdate()
    {
        timer.Tick();
        if (timer.CurrentTime >= randomTime)
        {
            Instantiate(acetate, spawner[Random.Range(0, spawner.Count)]);
            randomTime = Random.Range(minMaxSpawnTime.x, minMaxSpawnTime.y);
            timer.ResetTimer();
        }
    }

    public int GetChildren() => transform.childCount;
}