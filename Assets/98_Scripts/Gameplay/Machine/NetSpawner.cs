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

    public void ActivateNet()
    {
        int maxActive = 0;

        if(ShipHandler.Ship.Count > 15)
        {
            for(int i = 0;i<induNet.Count;i++)
            {
                if (maxActive < 3)
                {
                    if (induNet[i].activeSelf)
                    {

                    }
                    else
                    {
                        induNet[i].SetActive(true);
                        maxActive++;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < enviNet.Count; i++)
            {
                if (maxActive < 3)
                {
                    if (enviNet[i].activeSelf)
                    {

                    }
                    else
                    {
                        enviNet[i].SetActive(true);
                        maxActive++;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void DeactivateNet()
    {
        int maxDeactive = 0;

        Debug.LogError("Deactivate net: " + maxDeactive);

        if (ShipHandler.Ship.Count > 15)
        {
            for (int i = 0; i < induNet.Count; i++)
            {
                if (maxDeactive < 3)
                {
                    if (!induNet[i].activeSelf)
                    {

                    }
                    else
                    {
                        induNet[i].SetActive(false);
                        maxDeactive++;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < enviNet.Count; i++)
            {
                Debug.LogError("Envi");
                if (maxDeactive < 3)
                {
                    if (!enviNet[i].activeSelf)
                    {
                        Debug.LogError("Envi deactive");
                    }
                    else
                    {
                        Debug.LogError("envi active");
                        enviNet[i].SetActive(false);
                        maxDeactive++;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
