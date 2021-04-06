using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shaderFix : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Image>().enabled = false;
        this.GetComponent<Image>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
