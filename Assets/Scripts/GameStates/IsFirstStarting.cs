using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsFirstStarting : MonoBehaviour
{


    [HideInInspector]
    public static IsFirstStarting instance;
    [HideInInspector]
    public bool startUp = true;
    public bool audioManagerStarted = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Debug.Log("initialized");
        Invoke("setStartUpFalse", 1f);
    

        
    }
    private void setStartUpFalse()
    {
        startUp = false;

    }
  
}
