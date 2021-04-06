using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterAdPlayer : MonoBehaviour
{
    public int numberOfRunsToGetToAd = 3;
    [HideInInspector]
    public static InterAdPlayer instance;
    int runsSinceLastAd = 0;
    private void Awake()
    {
        if (PlayerPrefs.GetString("noads", "false") == "false")
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
            DontDestroyOnLoad(this.gameObject);
            runsSinceLastAd = PlayerPrefs.GetInt("runsSinceLastAd", 0);
        }
        

    }
 
    public bool runCompleted()
    {
        runsSinceLastAd++;
        if (runsSinceLastAd >= numberOfRunsToGetToAd)
        {
            runsSinceLastAd = 0;
            PlayerPrefs.SetInt("runsSinceLastAd", runsSinceLastAd);
            return true;
        }
        Debug.Log(runsSinceLastAd);
        PlayerPrefs.SetInt("runsSinceLastAd", runsSinceLastAd);
        return false;
    }

}
