using UnityEngine;

/**
 * Controls the transitions for the heroAnimatorController. Takes in player
 * input and changes animation state accordingly. This script is required by
 * the PlayerController script which uses the different states of animation
 * to adjust player movement accordingly.
 * 
 * @author Maxfield Barden
 */
public class PlayerAnimationController : MonoBehaviour
{

    Animator _anim;
    PlayerController player;
    SwordSlashSpawner slashSpawner;
    private CamShaker _cs;
    private int attacks = 0;
    private int totalAttacks = 0;
    private float attackTimer = 0f;
    private float attackReset = 4f;
    private int slashID;
    private int streak = 0;
    private int streakMultiplier = 1;
    private int elimSinceHeal = 0;
    private DamageManager dm;
    [HideInInspector]
    public ScoreCounter sCounter;

    /**
     * Called before first frame update and used to instantiate our variables.
     */
    void Start()
    {
        _anim = this.GetComponentInChildren<Animator>();
        player = this.GetComponent<PlayerController>();
        slashSpawner = this.GetComponent<SwordSlashSpawner>();
        dm = this.GetComponent<DamageManager>();
        _cs = player._cShaker;
    }

    /**
     * Makes the character start running once the game is started from the menu.
     */
    public void StartGame()
    {
        _anim.SetTrigger("Run");
    }

    /**
     * Detects collision with the enemy and plays the attack animation
     * appropiately.
     * 
     * @param collision The collision we are detecting.
     */
    private void OnCollisionEnter(Collision collision)
    {
        if (!dm.isDead() && collision.gameObject.transform.CompareTag("Enemy"))
        { 
            this.Attack();
            elimSinceHeal++;
            if (ColorDataBase.ScreenShake())
            {
                _cs.AttackShake();
            }
            sCounter.AddAttackScore(streakMultiplier);
            StartCoroutine(slashSpawner.Slash(this.GetSlashDelay(slashID)));
        }
    }

    /**
     * Plays the Slide animation.
     */
    public void Slide()
    {
        if (Input.GetKeyDown(KeyCode.S) || SwipeInput.Instance.SwipeDown)
        {
            _anim.SetTrigger("Slide");
            player.ChangeBCHeight();
        }
        
        if (_anim.GetAnimatorTransitionInfo(0).IsName("Slide -> Running") ||
            _anim.GetAnimatorTransitionInfo(0).IsName("SlideAgain -> Running"))
        {
            player.Invoke("resetBC", 0f);
        }
    }

    /**
     * Sets elim since heal back to 0 if player heals.
     */
    public void SetElimSinceHeal()
    {
        elimSinceHeal = 0;
    }


    /**
     * Resets the attackMultiplier upon missing an enemy.
     */
    public void resetAttackMultiplier()
    {
        // send the score counter the current streak before resetting it
        sCounter.SetHighestStreak(streak);
        streak = 0;
        streakMultiplier = 1;
    }

    /**
     * Gets the streak so it can be dispayed by the score counter.
     * 
     * @return the streak
     */
    public int GetStreak()
    {
        return streak;
    }

    /**
     * Gets the streakMultiplier so it can be displayed by the score
     * counter.
     * 
     * @return the streakMultiplier
     */
    public int GetStreakMultiplier()
    {
        return streakMultiplier;
    }

    /**
     * Increments the streak counter and streakMultiplier counter (if applicable)
     */
    private void IncrementStreakCounter()
    {
        // increment the streakMultiplier every 10 kills
        streak++;
        
        
        if (streak % 10 == 0)
        {
            streakMultiplier++;
            sCounter.DisplayScoreMult(streakMultiplier);
            sCounter.DisplayStreak(streak);
        }
    }

    /**
     * Sets the Attack animation trigger.
     */
    private void Attack()
    {
        // delay a little bit due to the large hitbox on enemies
        Invoke("IncrementStreakCounter", 0.25f);
        // slide attack
        if (this.IsSliding())
        {
            if (_anim.GetAnimatorTransitionInfo(0).IsName("Slide -> Running"))
            {
                _anim.SetTrigger("SlideAttack");
                player.setDeflects(0);
                slashID = -1;
                totalAttacks++;
                return;
            }
            player.ChangeBCHeight();
            _anim.SetTrigger("SlideAttack");
            _anim.SetTrigger("SlideAgain");
            player.setDeflects(0);
            slashID = -1;
            totalAttacks++;
            return;
        }
        if (this.IsSlideChain())
        {
            if (_anim.GetAnimatorTransitionInfo(0).IsName("SlideAgain -> Running"))
            {
                _anim.SetTrigger("SlideAttack");
                player.setDeflects(0);
                slashID = -1;
                totalAttacks++;
                return;
            }
            player.ChangeBCHeight();
            _anim.SetTrigger("SlideAttack");
            _anim.SetTrigger("Slide");
            player.setDeflects(0);
            slashID = -1;
            totalAttacks++;
            return;
        }


        if (attacks == 0)
        {
            _anim.SetTrigger("Attack");
            attacks++;
            slashID = 0;
        }
        else if (attacks == 1)
        {
            _anim.SetTrigger("AttackAgain");
            attacks++;
            slashID = 1;
        }
        else if (attacks == 2)
        {
            _anim.SetTrigger("Attack3");
            attacks = 0;
            slashID = 2;
        }
        player.setDeflects(0);
        totalAttacks++;
    }

    /**
     * Gets the total number of attacks.
     */
    public int GetAttacks()
    {
        return totalAttacks;
    }

    /**
     * Resets our attack counter if we haven't attacked in a few frames.
     */
    private void UpdateAttackCounter()
    {
       if (attacks > 0)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > attackReset)
            {
                attacks = 0;
                attackTimer = 0f;
            }
        }
    }

    /**
     * Gets how long of a delay we should have before playing our slash
     * particle effect.
     * 
     * @return The delay timer based on which attack animation we are playing.
     */
    private float GetSlashDelay(int attacks)
    {
        int attackNum = attacks;
        switch (attackNum)
        {
            // handles the slide case
            case -1:
                return 0.15f;

            // first attack
            case 0:
                return 0.15f;

            // second attack
            case 1:
                return 0.16f;

            // third attack
            case 2:
                return 0.2f;

            default:
                return 0f;
        }
    }

    /**
     * Updates the number of deflects to loop through deflect
     * animation cycles.
     */
    public void UpdateDeflectCount()
    {
        _anim.SetInteger("deflects", player.getDeflects());   
    }

    /**
     * Updates our deflecting status.
     */
    public void UpdateDeflect()
    {
        if (_anim.GetBool("Deflecting"))
        {
            _anim.SetBool("Deflecting", false);
        }
        else
        {
            _anim.SetBool("Deflecting", true);
        }
    }

    /**
     * Checks if an attack animation is currently playing.
     * 
     * @return True if an attack animation is currently playing.
     */
    public bool IsAttacking()
    {
        return attacks > 0;
    }

    /**
     * Checks if the hero is currently sliding.
     * 
     * @return True if the hero is curently in the Slide state.
     */
    public bool IsSliding()
    {
        return _anim.GetCurrentAnimatorStateInfo(0).IsName("Slide");
    }

    /**
     * Checks to see if the hero is in the second sliding state or not.
     * 
     * @return True if the hero is currently in the second Slide state.
     */
    public bool IsSlideChain()
    {
        return _anim.GetCurrentAnimatorStateInfo(0).IsName("SlideAgain");
    }

    /**
     * Checks if the hero is currently running.
     * 
     * @return True if the hero is currently in the Running state.
     */
    public bool IsRunning()
    {
        return _anim.GetCurrentAnimatorStateInfo(0).IsName("Running");
    }

    /**
     * Condenses our update logic to make our Update() method more readable.
     */
    private void UpdateAnimationState()
    {
        this.Slide();
        this.UpdateDeflectCount();
        this.UpdateAttackCounter();
    }

    /**
     * Called once per frame.
     */
    void FixedUpdate()
    {
        this.UpdateAnimationState();
        if (elimSinceHeal >= 200 && PlayerPrefs.GetString("kill200noheal", "locked") == "locked")
        {
            AchievementEvents.aEvents.UnlockAchievementTrigger("kill200noheal");
        }
    }

    public void Die()
    {
        _anim.SetTrigger("die");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("restart", "false");
    }
}
