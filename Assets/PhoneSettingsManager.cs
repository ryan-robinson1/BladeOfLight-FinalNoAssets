
using UnityEngine;

public class PhoneSettingsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("saveBattery", "false") == "false")
        {
            Application.targetFrameRate = 60;
        }
        else
        {
            Application.targetFrameRate = 30;
        }
        
    }

    /**
     * Sets the settings to battery saving mode.
     */
    public void OnPerformancePress()
    {
        if (PlayerPrefs.GetString("saveBattery", "false") == "false")
        {
            PlayerPrefs.SetString("saveBattery", "true");
            Application.targetFrameRate = 30;
        }
        else
        {
            PlayerPrefs.SetString("saveBattery", "false");
            Application.targetFrameRate = 60;
        }
    }
}
