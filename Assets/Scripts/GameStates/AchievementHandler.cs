using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Analytics;

/**
 * Responsible for handling the unlocking of achievements and updating our player
 * preferences as follows. Contains a dictionary database of achievements that can
 * be accessed by other scripts.
 * 
 * @author Maxfield Barden
 */

public class AchievementHandler : MonoBehaviour
{
    private Dictionary<string, Achievement> achievements;
    [HideInInspector]
    public TextMeshProUGUI highScoreText;

    public ScoreCounter scoreCounter;

    public Shop shop;

        

    /**
     * Called before the first frame update.
     */
    void Start()
    {
        AchievementEvents.aEvents.onAchievementUnlocked += UnlockAchievement;
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore1", 0).ToString();
    }

    /**
     * Called when the script instance is being loaded.
     */
    private void Awake()
    {
        InitializeAchievements();
    }

    /**
     * Sets the player prefs for the given achievement to become unlocked.
     * 
     * @param id The id of the achievement.
     */
    private void UnlockAchievement(string id)
    {
        // no need to execute this method if the achievement is already unlocked.
        if (achievements[id].IsUnlocked())
        {
            return;
        }

        // notify our player prefs that the achievement is unlocked and send analytics info
        Analytics.CustomEvent("Achievement Unlocked: ", new Dictionary<string, object>
        {
            {"Achievement", achievements[id].GetDisplayName() },
        });
        PlayerPrefs.SetString(id, "unlocked");
        achievements[id].SetUnlocked();
        if (id.Contains("shop"))
        {
            shop.depositMoney(achievements[id].GetPayout());
        }
        else
        {
            scoreCounter.AddAchievementBounty(achievements[id].GetPayout());
        }
        
    }

    /**
     * Returns our dictionary of achievements so other scripts can access it.
     * 
     * @return The achievements dictionary.
     */
    public Dictionary<string, Achievement> GetAchievements()
    {
        return achievements;
    }

    /**
     * Called on the destruction of this MonoBehaviour script.
     */
    void OnDestroy()
    {
        AchievementEvents.aEvents.onAchievementUnlocked -= UnlockAchievement;
    }

    /**
     * Initializes the player achievement dictionary. 
     * IF ADDING AN ACHIEVEMENT ORGANIZE BY TYPE (i.e. Score-based, attack-based, etc.)
     */
    private void InitializeAchievements()
    {
        achievements = new Dictionary<string, Achievement>(){

            // SCORE BASED ACHIEVEMENTS

            {"score1500", new Achievement(
                "score1500", PlayerPrefs.GetString("score1500", "locked"), "Noob Assassin",
                "Acquire a score of 1,500 or higher", 5)
            },

            {"score15k", new Achievement(
                "score15k", PlayerPrefs.GetString("score15k", "locked"), "Skilled Assassin",
                "Acquire a score of 15,000 or higher", 50)
            },

            {"score50k", new Achievement(
                "score50k", PlayerPrefs.GetString("score50k", "locked"), "Elite Assassin",
                "Acquire a score of 50,000 or higher", 150) 
            },

            {"score200k", new Achievement(
                "score200k", PlayerPrefs.GetString("score200k", "locked"), "Legendary Assassin",
                "Acquire a score of 200,000 or higher", 500) 
            },

            // STREAK BASED ACHIEVEMENTS

            {"streak25", new Achievement(
                "streak25", PlayerPrefs.GetString("streak25", "locked"), "Chain 'em Together",
                "Acquire an elim streak of 25 or higher", 15) 
            },

            {"streak75", new Achievement(
                "streak75", PlayerPrefs.GetString("streak75", "locked"), "Don't Miss",
                "Acquire an elim streak of 75 or higher", 50) 
            },

            {"bigguy", new Achievement(
                "bigguy", PlayerPrefs.GetString("bigguy", "locked"), "Big Guy",
                "Find Big Guy", 750) 
            },

            // THESE GO TOGETHER FOR THE MEME

            {"firsthealth", new Achievement(
                "firsthealth", PlayerPrefs.GetString("firsthealth", "locked"), "Call an Ambulance!",
                "Use a health pack for the first time", 5) 
            },

            {"kill200noheal", new Achievement(
                "kill200noheal", PlayerPrefs.GetString("kill200noheal", "locked"), "But not for me",
                "Eliminate 200 enemies in a single run without using a health pack", 500) 
            },

            // PACIFIST (IDK HOW TO SPELL) ACHIEVEMENTS

            {"passive5k", new Achievement(
                "passive5k", PlayerPrefs.GetString("passive5k", "locked"), "Love thy enemy",
                "Score 5,000 without eliminating an enemy", 15) 
            },

            {"passive20k", new Achievement(
                "passive20k", PlayerPrefs.GetString("passive20k", "locked"), "All life is sacred",
                "Score 20,000 without eliminating an enemy", 150) 
            },

            // SHOP RELATED ACHIEVEMENTS

            {"shopfirstherocolor", new Achievement(
                "shopfirstherocolor", PlayerPrefs.GetString("shopfirstherocolor", "locked"), "This color suits me",
                "Purchase a hero core", 5) 
            },

            {"shopfirstenemycolor", new Achievement(
                "shopfirstenemycolor", PlayerPrefs.GetString("shopfirstenemycolor", "locked"), "This color suits them",
                "Purchase an enemy core", 5)
            },

            {"shopfirstsword", new Achievement(
                "shopfirstsword", PlayerPrefs.GetString("shopfirstsword", "locked"), "Building an Arsenal",
                "Purchase a sword", 50) 
            },

        };
    }
}
