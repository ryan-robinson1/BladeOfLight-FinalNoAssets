using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLightScript : MonoBehaviour
{

    int oddsOfFlicker = 6;
    [SerializeField]
    private GameObject lightObject;
    private Light light;

    bool flicker;
    void Start()
    {
        light = lightObject.GetComponent<Light>();
        if (Random.Range(0, oddsOfFlicker) == 0)
        {
            flicker = true;
            flickerLight();
   
        }
    }



    void flickerLight()
    {
        //float randomTime = Random.Range(0.2f, 1.5f);

        turnOffLight();

        Invoke("flickerLight", 0.08f);

    }
    void turnOffLight()
    {

        light.enabled = false;
        Invoke("turnOnLight", 0.04f);
    }
    void turnOnLight()
    {
  
        light.enabled = true;
    }
}


