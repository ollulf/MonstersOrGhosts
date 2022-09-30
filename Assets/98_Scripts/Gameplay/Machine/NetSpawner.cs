using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSpawner : MonoBehaviour
{
    [SerializeField] private Transform enviTrans, induTrans;
    private List<GameObject> enviNet, induNet;

    // Start is called before the first frame update
    void Start()
    {
        enviNet = new List<GameObject>();
        induNet = new List<GameObject>();

        for(int i = 0;i<enviTrans.childCount;i++)
        {
            enviNet.Add(enviTrans.GetChild(i).gameObject);
        }

        for (int i = 0; i < induTrans.childCount; i++)
        {
            induNet.Add(induTrans.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
