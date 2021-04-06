using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeButtonNext : MonoBehaviour
{
    private DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap scrollSnap;
    void Start()
    {
        scrollSnap = this.GetComponent<DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap>();
    }
    private void Update()
    {
        if (this.GetComponentInParent<Canvas>().enabled == true)
        {
            if (SwipeInput.Instance.SwipeLeft)
            {
                scrollSnap.GoToNextPanel();
            }
            else if (SwipeInput.Instance.SwipeRight)
            {
                scrollSnap.GoToPreviousPanel();
            }
        }
        
    }


}
