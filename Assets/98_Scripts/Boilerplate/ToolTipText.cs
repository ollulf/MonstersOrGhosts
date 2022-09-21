using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ToolTipText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [ResizableTextArea]
    [SerializeField] private string header, toolTipText;

    [SerializeField] GameObject toolTipBox;

    private Timer timer;
    private bool isShown, isOver;

    private void Start()
    {
        isShown = false;
        isOver = false;
        timer = new Timer();
        timer.SetStartTime(0, false);
    }



    private void Update()
    {
        if (isOver)
        {
            timer.Tick();
            Debug.LogError("Tick");
            if (timer.CurrentTime >= 1)
            {
                ShowToolTip();
            }

        }
    }

    private void ShowToolTip()
    {
        toolTipBox.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = header;
        toolTipBox.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = toolTipText;
        if (!isShown)
        {
            isShown = true;
            toolTipBox.SetActive(isShown);
            toolTipBox.transform.position = new Vector3(toolTipBox.transform.position.x, transform.position.y, toolTipBox.transform.position.z);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
        isShown = isOver;
        toolTipBox.SetActive(isShown);
        timer.ResetTimer();
    }
}
