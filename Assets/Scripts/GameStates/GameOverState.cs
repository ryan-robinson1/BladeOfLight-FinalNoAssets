using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using TMPro;
using System;
using System.Collections.Generic;

/**
 * Takes care of the game over screen when after the player runs out of health.
 * In charge of navigating the game over screen's ui and displaying unique
 * information regarding the previous game played.
 * 
 * @author Maxfield Barden
 */

public class GameOverState : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject inGameUI;
    public GameObject startMenu;
    public GameObject shop;
    public InterAdManager _interAdManager;
    public GameObject reviveButtons;
    public GameObject noReviveButtons;
    private StartMenuScript sMenuScript;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI runMoneyText;
    public TextMeshProUGUI achMoneyText;
    public TextMeshProUGUI totalPayoutText;
    public TextMeshProUGUI scoreText;
    public RawImage bigGuy;

    [HideInInspector]
    public float moneyMultiplier = 1f;
    private int score;
    private int streak;
    private int bounty;
    private int achievementMoney;
    bool played = false;
    bool playInterAd = false;

    private void OnEnable()
    {
        inGameUI.GetComponent<ScoreCounter>().enabled = false;
        inGameUI.GetComponent<Canvas>().enabled = false;
        sMenuScript = startMenu.GetComponent<StartMenuScript>();
        calculateBounty();
        DetermineHighScore(score);
        DisplayScore();
        gameOverUI.GetComponent<Canvas>().enabled = true;
        Analytics.CustomEvent("Score From Round: ", new Dictionary<string, object>
        {
            {"Score: ", GetScoreRange(score) },
        }
        );

        Analytics.CustomEvent("Highest Streak: ", new Dictionary<string, object>
        {
            {"Streak: ", GetStreakRange(streak) },
        }
        );


        this.GetComponent<PlayerController>().enabled = false;
        this.GetComponent<PlayerAnimationController>().enabled = false;

        if (played)
        {
            reviveButtons.SetActive(false);
            noReviveButtons.SetActive(true);
            DisplayScore();
        }
        else
        {
            played = true;
        }
      
    }

    /**
     * Displays the score from this current round.
     */
    private void DisplayScore()
    {
        if (IsHighScore())
        {
            highScoreText.enabled = true;
            
        }
        killsText.text = streak.ToString();
        runMoneyText.text = "$" + bounty + "";
        achMoneyText.text = "$" + achievementMoney + "";
        int payout = achievementMoney + bounty;
        totalPayoutText.text = "$" + payout+ "";
        scoreText.text = score + "";


    }

    /**
     * Restarts the game upon being clicked.
     */
    public void RestartGame()
    {
        PlayerPrefs.SetString("HeroColor", ColorDataBase.heroColorName);
        PlayerPrefs.SetString("EnemyColor", ColorDataBase.enemyColorName);
        PlayerPrefs.SetString("SwordColor", ColorDataBase.swordColorName);
        PlayerPrefs.SetString("restart", "true");
        if (bigGuy.gameObject.activeSelf)
        {
            bigGuy.gameObject.SetActive(false);
        }
        if (PlayerPrefs.GetString("noads", "false") == "false" && InterAdPlayer.instance.runCompleted())
        {
            _interAdManager.PlayInterstitialAd();
            Analytics.CustomEvent("Watched inter ad");
         
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }



    }

    /**
     * Goes to main menu upon being clicked.
     */
    public void GoToMenu()
    {
        PlayerPrefs.SetString("HeroColor", ColorDataBase.heroColorName);
        PlayerPrefs.SetString("EnemyColor", ColorDataBase.enemyColorName);
        PlayerPrefs.SetString("SwordColor", ColorDataBase.swordColorName);
        PlayerPrefs.SetString("restart", "false");
        if (bigGuy.gameObject.activeSelf)
        {
            bigGuy.gameObject.SetActive(false);
        }

        if (PlayerPrefs.GetString("noads", "false") == "false" && InterAdPlayer.instance.runCompleted())
        {
            _interAdManager.PlayInterstitialAd();
            Analytics.CustomEvent("Watched inter ad");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /**
     * Gets the achievement bounty.
     */
    public void GetAchievementBounty(int bounty)
    {
        achievementMoney = bounty;
    }

    /**
     * Revives the player upon being called
     */
    public void revivePlayer()
    {
        this.GetComponent<DamageManager>().resetHealth();
        inGameUI.GetComponent<ScoreCounter>().enabled = true;
        inGameUI.GetComponent<Canvas>().enabled = true;
        gameOverUI.GetComponent<Canvas>().enabled = false;
        this.GetComponent<PlayerController>().enabled = true;
        this.GetComponent<PlayerAnimationController>().enabled = true;
        this.enabled = false;
    }

    /**
     * Gets the score from the current round.
     * 
     * @param int The score from the round that was just completed.
     */
    public void GetScoreFromRound(int score)
    {
        this.score = score;
        HandleScoringAchievements();
    }

    /**
     * Determines if the score from this round is a new high score.
     * 
     * @return True if this round's score is a new high score.
     */
    private bool IsHighScore()
    {
        // based on script execution order, it's HighScore2, not HighScore1
        return score > PlayerPrefs.GetInt("HighScore1", 0);
    }

    /**
     * Tells the achievements to update the high score chart if this is
     * a new high score.
     */
    private void DetermineHighScore(int score)
    {
        // if its higher than the lowest high score, it's a new high score.
        if (IsHighScore())
        {
            PlayerPrefs.SetInt("HighScore1", score);
        }
    }

    /**
     * Gets the highest streak from the current round.
     * 
     * @param streak The highest streak from the current round.
     */
    public void GetHighestStreak(int streak) 
    {
        this.streak = streak;
        HandleStreakAchievements();
    }

    /**
     * Determines streak range. Used for analytics tracking.
     */
    private string GetStreakRange(int streak)
    {
        if (streak < 25)
        {
            return "25";
        }

        if (streak >= 25 && streak < 50)
        {
            return "25-50";
        }

        if (streak >= 50 && streak < 100)
        {
            return "50-100";
        }

        if (streak >= 100)
        {
            return "100";
        }

        return "streak NA";
    }

    /**
     * Gets the score range from the current round. Used for analytics tracking.
     */
    private string GetScoreRange(int score)
    {

        if (score < 5000)
        {
            return "<5000";
        }

        if (score >= 5000 && score < 10000)
        {
            return "5000 - 10,000";
        }

        if (score >=10000 && score < 20000)
        {
            return "10,000 - 20,000";
        }

        if (score >= 20000 && score < 50000)
        {
            return "20,000 - 50,000";
        }

        if (score >=50000 && score < 100000)
        {
            return "50,000 - 100,000";
        }

        if (score >= 100000)
        {
            return ">100,000";
        }
        return "score NA";
    }

    /**
    * Calculates the money earned from the round
    */
    public void calculateBounty()
    {
        // new payout system (every 1000 = 1 credit) rounds to next highest whole number
        // with a base of one per round
        double unRounded = score / 1000f;
        bounty = ((int)Math.Ceiling(unRounded*moneyMultiplier) + 1);

        shop.GetComponent<Shop>().depositMoney(bounty + achievementMoney);
    }

    /**
     * Handles unlocking achievements based on the score from this round.
     */
    private void HandleScoringAchievements()
    {
        // will need to restructure this code to execute better
        if (PlayerPrefs.GetString("score1500", "locked") == "locked" && score >= 1500)
        {
            AchievementEvents.aEvents.UnlockAchievementTrigger("score1500");
        }
        if (PlayerPrefs.GetString("score15k", "locked") == "locked" && score >= 15000)
        {
            AchievementEvents.aEvents.UnlockAchievementTrigger("score15k");
        }
        if (PlayerPrefs.GetString("score50k", "locked") == "locked" && score >= 50000)
        {
            AchievementEvents.aEvents.UnlockAchievementTrigger("score50k");
        }

        //very rare to hit 200K so flip the if statement
        if (score >= 200000 && PlayerPrefs.GetString("score200k", "locked") == "locked")
        {
            AchievementEvents.aEvents.UnlockAchievementTrigger("score200k");
        }

        if (ColorDataBase.heroColorName == "aqua" && ColorDataBase.enemyColorName == "aqua"
            && ColorDataBase.swordColorName == "aqua" && score >= 69000 && score < 70000 &&
            PlayerPrefs.GetString("bigguy", "locked") == "locked")
        {
            AchievementEvents.aEvents.UnlockAchievementTrigger("bigguy");
            bigGuy.gameObject.SetActive(true);
        }
    }

    /**
     * Handles unlocking achievements based on the highest streak from this round.
     */
    private void HandleStreakAchievements()
    {
        if (PlayerPrefs.GetString("streak25", "locked") == "locked" && streak >= 25)
        {
            AchievementEvents.aEvents.UnlockAchievementTrigger("streak25");
        }

        if (PlayerPrefs.GetString("streak75", "locked") == "locked" && streak >= 75)
        {
            AchievementEvents.aEvents.UnlockAchievementTrigger("streak75");
        }
    }
}
