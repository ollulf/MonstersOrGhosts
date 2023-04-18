using System.Collections.Generic;
using UnityEngine;

public class FishTrawlerSpawn : MonoBehaviour
{
    [SerializeField] private Transform enviTrans, induTrans;
    private List<GameObject> enviNet, induNet;

    void Start()
    {
        enviNet = new List<GameObject>();
        induNet = new List<GameObject>();

        for (int i = 0; i < enviTrans.childCount; i++)
        {
            enviNet.Add(enviTrans.GetChild(i).gameObject);
        }

        for (int i = 0; i < induTrans.childCount; i++)
        {
            induNet.Add(induTrans.GetChild(i).gameObject);
        }
    }

    public void ActivateNet()
    {
        if (ShipHandler.Ship.Count > 15)
        {
            for (int i = 0; i < induNet.Count; i++)
            {
                if (induNet[i].activeSelf)
                {

                }
                else
                {
                    induNet[i].SetActive(true);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < enviNet.Count; i++)
            {
                if (enviNet[i].activeSelf)
                {

                }
                else
                {
                    enviNet[i].SetActive(true);
                    break;
                }
            }
        }
    }

    public void DeactivateNet()
    {
        if (ShipHandler.Ship.Count > 15)
        {
            for (int i = 0; i < induNet.Count; i++)
            {
                if (!induNet[i].activeSelf)
                {

                }
                else
                {
                    induNet[i].SetActive(false);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < enviNet.Count; i++)
            {
                if (!enviNet[i].activeSelf)
                {

                }
                else
                {
                    enviNet[i].SetActive(false);
                    break;
                }
            }
        }
    }
}