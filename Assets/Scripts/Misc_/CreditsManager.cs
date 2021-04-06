using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * CreditsManager is in charge of managing playing the credits and exiting back
 * to the options menu.
 * 
 * @author Maxfield Barden
 */
public class CreditsManager : MonoBehaviour
{
    public Canvas startMenu;
    public Canvas creditsText;
    /**
     * Enables credits menu.
     */
    public void OnCreditsPress()
    {
        this.GetComponent<Canvas>().enabled = true;
        startMenu.enabled = false;
        creditsText.enabled = true;
    }

    /**
     * Disables the credits menu.
     */
    public void OnCreditsExit()
    {
        this.GetComponent<Canvas>().enabled = false;
        startMenu.enabled = true;
        creditsText.enabled = false;
    }
}
