using System.Collections;
using System.Text;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

/**
 * This script is in charge of navigating the settings UI.
 * 
 * @author Maxfield Barden
 */
public class SettingsNavigator : MonoBehaviour
{

    public GameObject displayPlane;
    public AudioManager _am;


    /**
     * Enables the settings ui.
     */
    public void EnableSettingsMenu()
    {
        displayPlane.SetActive(true);
    
 
    }


    /**
     * Returns to the main menu upon the back button being clicked.
     */
    public void OnExitButtonClick()
    {
 
        displayPlane.SetActive(false);
    }

    /**
     * Handles the muting / unmuting functionality.
     */
    public void OnMuteButtonPress()
    {
        if (PlayerPrefs.GetString("muteMusic", "false") == "false")
        {
            _am.MuteAllSounds();
       
        }

        // otherwise, unmute
        else
        {
            _am.UnMuteAllSounds();
         
        }
    }

    /**
     * Handles enabling / disabling screen shake.
     */
    public void OnScreenShakePress()
    {
        if (PlayerPrefs.GetString("sshake", "true") == "true")
        {
            PlayerPrefs.SetString("sshake", "false");
       
        }
        else
        {
            PlayerPrefs.SetString("sshake", "true");
         
        }
    }

    /**
     * Handles enabling / disabling vibration.
     */
    public void OnVibrationPress()
    {
        if (PlayerPrefs.GetString("vibrate", "true") == "true")
        {
            PlayerPrefs.SetString("vibrate", "false");
           
        }
        else
        {
            PlayerPrefs.SetString("vibrate", "true");
           
        }
    }
}
