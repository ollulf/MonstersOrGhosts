using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TernFloatAndFly : MonoBehaviour
{
    Animator animator;

    public float floatTimeMin = 1, floatTimeMax = 5;

    private Timer timer;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        timer = new Timer();
        timer.SetStartTime(Random.Range(floatTimeMin, floatTimeMax), true);
    }

    // Update is called once per frame
    void Update()
    {
        timer.Tick();

        if(timer.CurrentTime <= 0)
        {
            timer.SetStartTime(Random.Range(floatTimeMin, floatTimeMax), true);
            timer.ResetTimer();
            ToggleFlyAndFloat();
        }
    }

    private void ToggleFlyAndFloat()
    {
        animator.SetBool("IsFloating", !animator.GetBool("IsFloating"));
    }
}
