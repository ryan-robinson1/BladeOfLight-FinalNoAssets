using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
    // Start is called before the first frame update
    Image i;
    float rgbValue =0.33f;
    bool increasingRGB = true;
    void Start()
    {
        i=this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rgbValue >= 0.45f&&increasingRGB)
        {
            increasingRGB = false;
        }
        else if(rgbValue<33f && !increasingRGB){
            increasingRGB = true;
        }
        
        i.color = new Color(rgbValue, rgbValue, rgbValue);
        if (increasingRGB)
        {
            rgbValue += 0.005f;
        }
        else
        {
            rgbValue -= 0.005f;
        }

        Debug.Log(rgbValue);
    }
}
