using System.Collections;
using System.Text;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

/**
 * This script is in charge of navigating the achievments menu UI
 *  and handling the achievement unlock for the shop.
 * 
 * @author Maxfield Barden
 */
public class AchievementTracker : MonoBehaviour
{
    private AchievementHandler _acHandler;
    private Achievement ach;
    public string id;
    [HideInInspector]
    public TextMeshProUGUI displayNameText;
    [HideInInspector]
    public TextMeshProUGUI descriptionText;
    [HideInInspector]
    public TextMeshProUGUI moneyText;
    [HideInInspector]
    public Image unearned;
    [HideInInspector]
    public Image earned;

    // Start is called before the first frame update
    void Start()
    {
        _acHandler = this.GetComponentInParent<AchievementHandler>();
        ach = _acHandler.GetAchievements()[id];
        InitVariables();
    }

    /**
     * Sets the variables, condenses code.
     */
    public void InitVariables()
    {
        displayNameText.text = ach.GetDisplayName();
        descriptionText.text = ach.GetDescription();

        if (ach.IsUnlocked())
        {
            unearned.enabled = false;
            earned.enabled = true;
            moneyText.text = "";
        }
        else
        {
            unearned.enabled = true;
            earned.enabled = false;
            moneyText.text = "$" + ach.GetPayout();
        }

    }

}
