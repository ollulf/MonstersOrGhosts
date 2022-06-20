using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyVisibleInEditor : MonoBehaviour
{
    public void Awake()
    {
        Destroy(gameObject);
    }
}
