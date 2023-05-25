using UnityEngine;

public class TernFloatAndFly : MonoBehaviour
{
    Animator animator;

    public float floatTimeMin = 1, floatTimeMax = 5;

    private Timer timer;

    void Start()
    {
        animator = GetComponent<Animator>();
        timer = new Timer(Random.Range(floatTimeMin, floatTimeMax), true);
    }

    void Update()
    {
        timer.Tick();

        if(timer.CurrentTime <= 0)
        {
            timer.SetNewTime(Random.Range(floatTimeMin, floatTimeMax), true);
            timer.ResetTimer();
            ToggleFlyAndFloat();
        }
    }

    private void ToggleFlyAndFloat() => animator.SetBool(id: Animator.StringToHash("IsFloating"), !animator.GetBool("IsFloating"));
}