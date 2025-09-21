using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private float _timeInCredits = 0;
    private Timer _timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timer = new Timer(_timeInCredits, true);
    }

    // Update is called once per frame
    void Update()
    {
        _timer.Tick();
        if(_timer.CurrentTime <= 0 || Input.anyKeyDown)
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
}
