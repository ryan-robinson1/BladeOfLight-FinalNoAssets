using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**
 * Handles the scoring system in the game. Gives players points for deflecting,
 * attacking, as well as distance ran. Will store as a high score if it is 
 * higher than the lowest high score.
 * 
 * @author Maxfield Barden
 */

public class ScoreCounter : MonoBehaviour
{
    public GameObject player;
    private PlayerAnimationController _animController;
    private GameOverState gmOverState;
    private int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI displayScore;
    public TextMeshProUGUI scoreMult;
    private Canvas canvas;
    private int deflectScore = 5;
    private int attackScore = 50;
    private int highestStreak;
    private int totalElims;
    private bool healed;
    private int achievementBounty = 0;
    private int displayBoxSize = 0;
    public GameObject[] displayBoxs;
    private bool[] shifted = { false, false, false, false, false, false };
    

    /**
     * Called before the first frame update.
     */
    void Start()
    {
        score = 0;
        highestStreak = 0;
        totalElims = 0;
        healed = false;
        _animController = player.GetComponent<PlayerAnimationController>();
        gmOverState = player.GetComponent<GameOverState>();
        canvas = this.GetComponentInChildren<Canvas>();
        
    }

    /**
     * Displays the score on screen given the score provided.
     */
    private void DisplayScore(int score)
    {
        try
        {
            TextMeshProUGUI tempText = Instantiate(displayScore, canvas.transform) as TextMeshProUGUI;
            tempText.transform.SetParent(canvas.transform, false);
            tempText.fontSize = 36;
            tempText.text = "+" + score;
            tempText.GetComponent<Animator>().SetTrigger("displayScore");

        }
        catch
        {

        }
       
    }

    /**
     * Displays score multiplier.
     */
    public void DisplayScoreMult(int mult)
    {
        TextMeshProUGUI tempText = Instantiate(scoreMult, canvas.transform) as TextMeshProUGUI;
        tempText.transform.SetParent(canvas.transform, false);
        tempText.text = "x" + mult + " Elim Multiplier";
        tempText.GetComponent<Animator>().SetTrigger("displayMult");
    }

    /**
     * Displays streak multiplier.
     */
    public void DisplayStreak(int streak)
    {
        TextMeshProUGUI tempText = Instantiate(scoreMult, canvas.transform) as TextMeshProUGUI;
        tempText.transform.SetParent(canvas.transform, false);
        tempText.text = streak + " Elim Streak";
        tempText.GetComponent<Animator>().SetTrigger("displayStreak");
    }

    /**
     * Sends the game over screen the score from this round when the player dies.
     */
    private void OnDisable()
    {
        try
        {
            _animController.resetAttackMultiplier();
            this.SendRoundInfo();
        }
        catch
        {

        }
      
    }

    /**
     * Sends the game over state the information from the current round.
     */
    private void SendRoundInfo()
    {
        gmOverState.GetScoreFromRound(score);
        gmOverState.GetAchievementBounty(achievementBounty);
        gmOverState.GetHighestStreak(highestStreak);
        
    }

    /**
     * Adds points to the total score when we deflect a bullet.
     */
    public void AddDeflectScore()
    {
        score += deflectScore;
        this.DisplayScore(deflectScore);
    }

    /**
     * Tells us that we have healed.
     */
    public void JustHealed()
    {
        healed = true;
    }

    /**
     * Compares the current streak that just broke to the current
     * highest streak on record. If the streak being passed in
     * is greater than the current highest streak, it becomes
     * the new highest streak. Only used for calculating high
     * streaks during the current round.
     * 
     * @param streak The streak we are comparing the current
     * highest streak to.
     */
    public void SetHighestStreak(int streak)
    {
        totalElims += streak;
        if (streak > highestStreak)
        {
            highestStreak = streak;
        }
    }

    /**
     * Adds points to the total score when we kill an enemy.
     * Every 10 kill streaks the multiplier for the score increases.
     */
    public void AddAttackScore(int streakMultiplier)
    {
        var ascore = attackScore * streakMultiplier;
        score += (ascore);
        this.DisplayScore(ascore);
    }

    /**
     * Adds to our achievementBounty score.
     */
    public void AddAchievementBounty(int bounty)
    {
        achievementBounty += bounty;
    }

    /**
     * Update is called once per frame.
     */
    void FixedUpdate()
    {
        score++;
 
        if (score>=10000 && !shifted[0])
        {
            displayBoxs[0].SetActive(false);
            displayBoxs[1].SetActive(true);
            RectTransform myRectTransform = scoreText.rectTransform;
            myRectTransform.localPosition -= new Vector3(32f, 0, 0);
            shifted[0] = true;
           
        }
        else if(score >=100000 && !shifted[1])
        {
            displayBoxs[1].SetActive(false);
            displayBoxs[2].SetActive(true);
            RectTransform myRectTransform = scoreText.rectTransform;
            myRectTransform.localPosition -= new Vector3(32f, 0, 0);
            shifted[1] = true;

        }
        else if (score >= 1000000 && !shifted[2])
        {
            displayBoxs[2].SetActive(false);
            displayBoxs[3].SetActive(true);
            RectTransform myRectTransform = scoreText.rectTransform;
            myRectTransform.localPosition -= new Vector3(32f, 0, 0);
            shifted[2] = true;
        }
        else if (score >= 10000000 && !shifted[3])
        {
            displayBoxs[3].SetActive(false);
            displayBoxs[4].SetActive(true);
            RectTransform myRectTransform = scoreText.rectTransform;
            myRectTransform.localPosition -= new Vector3(32f, 0, 0);
            shifted[3] = true;
        }
        else if (score >= 100000000 && !shifted[4])
        {
            displayBoxs[4].SetActive(false);
            displayBoxs[5].SetActive(true);
            RectTransform myRectTransform = scoreText.rectTransform;
            myRectTransform.localPosition -= new Vector3(32f, 0, 0);
            shifted[4] = true;
        }
        scoreText.text = 
            $"Score: {score} \nStreak: {_animController.GetStreak()}\nMultiplier: {_animController.GetStreakMultiplier()}";
        HandleElimAchievements();
    }

    /**
     * Handles unlocking achievements regarding eliminations.
     */
    private void HandleElimAchievements()
    {

        if (_animController.GetAttacks() == 0 && score >= 5000 && PlayerPrefs.GetString("passive5k", "locked") == "locked")
        {
            AchievementEvents.aEvents.UnlockAchievementTrigger("passive5k");
        }

        if (_animController.GetAttacks() == 0 && score >= 20000 && PlayerPrefs.GetString("passive20k", "locked") == "locked")
        {
            AchievementEvents.aEvents.UnlockAchievementTrigger("passive20k");
        }
    }
}
