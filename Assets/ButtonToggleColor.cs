using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggleColor : MonoBehaviour
{
    [SerializeField] public ColorBlock colors;

    private Button button;
    
    void Start()
    {
        if (!(button = GetComponent<Button>()))
            Destroy(this);
        else
            button = GetComponent<Button>();
    }

    public void ToggleColor()
    {
        ColorBlock tempColorBlock = button.colors;

        button.colors = colors;

        colors = tempColorBlock;

    }

}
