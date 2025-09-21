using Unity.VisualScripting;
using UnityEngine;

public class NarrationSkipper : MonoBehaviour
{
    [SerializeField] private Transform _narration;
    private int index = 0, _maxIndex = 0, _currentIndex = 0;


    private void Start()
    {
        foreach (Transform child in _narration)
        {
            child.gameObject.SetActive(false);
            _maxIndex++;
        }

        _narration.GetChild(index).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) index--;
        if (Input.GetKeyDown(KeyCode.RightArrow)) index++;

        if (index < 0) index = 0;
        if (index == _maxIndex) index = _maxIndex - 1;

        if (index != _currentIndex) ChangePage();
    }

    private void ChangePage()
    {
        _narration.GetChild(_currentIndex).gameObject.SetActive(false);
        _narration.GetChild(index).gameObject.SetActive(true);
        _currentIndex = index;
    }
}
