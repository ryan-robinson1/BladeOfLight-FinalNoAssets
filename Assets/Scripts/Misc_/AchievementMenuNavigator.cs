using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementMenuNavigator : MonoBehaviour
{
    public Canvas achMenu;
    public Canvas mainMenu;
    public void startAchMenu()
    {
        achMenu.enabled = true;
        mainMenu.enabled = false;
        
    }
    public void exitAchMenu()
    {
        achMenu.enabled = false;
        mainMenu.enabled = true;
    }

}
