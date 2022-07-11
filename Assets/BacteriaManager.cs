using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BacteriaManager : MonoBehaviour
{
    [SerializeField] GameObject bacteriaObject;
    [SerializeField] Transform spawnPoint;

    [SerializeField] TextMeshProUGUI timerUI, scoreUI, buttonUI, highscoreUI;

    [SerializeField] private KeyCode[] keyCodes;
    private KeyCode currendKey;

    private int score = 0;
    private int highscore = 0;
    private int time = 30;
    private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        SetNewKey();
        GameObject obj = Instantiate(bacteriaObject,spawnPoint);
        obj.GetComponent<Bacteria>().SetCenterTransform(spawnPoint);

        //Debug.Log("Press " + currendKey);

        timer = new Timer();
        timer.SetStartTime(time);
    }

  
    // Update is called once per frame
    void Update()
    {
        timer.Tick();

        if(Input.GetKeyDown(currendKey))
        {
            score++;
            SetNewKey();
            GameObject obj = Instantiate(bacteriaObject,spawnPoint);
            obj.GetComponent<Bacteria>().SetCenterTransform(spawnPoint);
            obj.GetComponent<Rigidbody>().AddForce(UnityEngine.Random.Range(0, 5), UnityEngine.Random.Range(0,5), UnityEngine.Random.Range(0, 5));

        }
        UpdateUI();
        CheckTimer();
        
    }

    private void CheckTimer()
    {
        if(timer.CurrentTime < 0)
        {
            timer.ResetTimer();
            highscore = score;
            score = 0;
            SetNewKey();
        }
    }

    private void UpdateUI()
    {
        timerUI.SetText(""+timer.CurrentTime.ToString("0"));
        scoreUI.SetText("" + score);
        buttonUI.SetText("" + currendKey);
        highscoreUI.SetText("" + highscore);
    }

    private void SetNewKey()
    {
        currendKey = keyCodes[UnityEngine.Random.Range(0, keyCodes.Length - 1 )];
    }
}
