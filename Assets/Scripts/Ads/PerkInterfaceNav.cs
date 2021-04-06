using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkInterfaceNav : MonoBehaviour
{
    public GameObject perkMenu;
    public void enablePerkMenu()
    {
        if (!perkMenu.activeSelf)
        {
            perkMenu.SetActive(true);
        }
        
    }
    public void disablePerkMenu()
    {
        perkMenu.SetActive(false);
    }
}
