using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaManager : MonoBehaviour
{
    [SerializeField] GameObject bacteriaObject;
    [SerializeField] Transform spawnPoint;

    [SerializeField] private KeyCode[] keyCodes;
    private KeyCode currendKey;

    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        SetNewKey();
        GameObject obj = Instantiate(bacteriaObject,spawnPoint);
        obj.GetComponent<Bacteria>().SetCenterTransform(spawnPoint);

        Debug.Log("Press " + currendKey);
    }

  
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(currendKey))
        {
            score++;
            SetNewKey();
            Debug.Log("Pressed Correct Key!");
            Debug.Log("Press " + currendKey);
            GameObject obj = Instantiate(bacteriaObject, spawnPoint);
            obj.GetComponent<Bacteria>().SetCenterTransform(spawnPoint);

        } 
    }

    private void SetNewKey()
    {
        currendKey = keyCodes[UnityEngine.Random.Range(0, keyCodes.Length - 1 )];
    }
}
