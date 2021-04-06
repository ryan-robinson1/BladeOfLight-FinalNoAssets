using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class ButtonResizer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Button button;
    private Vector3 startScale;
    private bool buttonPressed = false;
    private Vector3 targetScale;

    private void Start()
    {
        button = this.GetComponent<Button>();
        startScale = button.transform.localScale;
        targetScale = startScale / 1.075f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
    }

    void Update()
    {
        if (buttonPressed)
        {
            button.transform.localScale = targetScale;
        }

        else
        {
            if (button.transform.localScale != startScale)
            {
                button.transform.localScale = Vector3.Lerp(button.transform.localScale, startScale, Time.deltaTime * 4);
            }
        }
    }
}
