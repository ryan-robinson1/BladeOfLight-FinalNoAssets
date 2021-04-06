using UnityEngine.Analytics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageManager : MonoBehaviour
{
    public Image HealthBar;
    [HideInInspector]
    public float bulletDamage = 2;
    public float startHealth = 100;
    public Material healthIndicator;
    [HideInInspector]
    public float health;
    private float healHealth;

    // could possibly make a list of particle systems
    public ParticleSystem damageEffect;
    public ParticleSystem damageLightEffect;
    public ParticleSystem healEffect;
    public ParticleSystem healEffect1;
    public ParticleSystem healLightFX;

    private ParticleSystem.MainModule lightfxModule;
    private ParticleSystem.MainModule healfxModule;

    private GameOverState gameOverState;
    private CamShaker _cs;

    private bool dead = false;

    private float min = 0f;
    private float max = 0.03f;
    private float t = 0f;
    [HideInInspector]
    public float edgeWidth;


    /**
     * Called before any of the Update methods are called for the first time.
     */
    private void Start()
    {
        health = startHealth;
        healHealth = startHealth;
        edgeWidth = healthIndicator.GetFloat("edgeWidth");
        SetHeroColor();
        this.setDefaultMats();
        
        _cs = this.GetComponent<PlayerController>()._cShaker;
        gameOverState = this.GetComponent<GameOverState>();
    }

    /**
     * Called every fixed frame rate.
     */
    private void FixedUpdate()
    {
        this.UpdateHealthStatus(health);

    }
    public void SetHeroColor()
    {
        healthIndicator.SetColor("dissolveColor",
           ColorDataBase.GetCurrentHeroColor());
        lightfxModule = damageLightEffect.main;
        lightfxModule.startColor = ColorDataBase.GetHeroAlbedo();

        healfxModule = healLightFX.main;
        healfxModule.startColor = ColorDataBase.GetHeroAlbedo();
    }
    public void resetHealth()
    {
        health = startHealth/2;
        this.GetComponent<PlayerAnimationController>().StartGame();
        dead = false;
        Analytics.CustomEvent("Revived");
        healthIndicator.SetFloat("edgeWidth", 0.03f);
    }

    public bool isDead()
    {
        return dead;
    }
    /**
     * Sets our hero to "half" health so we can better see colors in the shop.
     */
    public void SetShopDisplay()
    {
        health = startHealth - (startHealth / 3);
        healHealth = startHealth - (startHealth / 3);
    }

    /**
     * Resets us to full health upon exiting the shop.
     */
    public void OnShopExit()
    {
        health = startHealth;
        healHealth = startHealth;
    }

    //Method executed if hit by bullet. Takes damage.
    public void TakeDamage()
    {
        if (!dead)
        {
            health -= bulletDamage;
            healHealth -= bulletDamage;
            damageEffect.Play();
            damageLightEffect.Play();
            AudioManager.instance.Play("HitSound", Random.Range(0.9f, 1.1f));
            if (ColorDataBase.Vibration())
            {
                Handheld.Vibrate();
            }

            // round is over
            if (health <= 0)
            {
                FindObjectOfType<AudioManager>().Pause("Footsteps");
                gameOverState.enabled = true;
                dead = true;
                this.GetComponent<PlayerAnimationController>().Die();
            }
        }
        
    }

    /**
     * Takes damage from the mine. Damages player by 15 health.
     */
    public void TakeDamageFromMine()
    {
        if (!dead)
        {
            int mineDamage = 70;
            AudioManager.instance.Play("HitSound", Random.Range(0.9f, 1.1f));
            health -= mineDamage;
            healHealth -= mineDamage;
            damageEffect.Play();
            damageLightEffect.Play();
            if (ColorDataBase.Vibration())
            {
                Handheld.Vibrate();
            }
            if (ColorDataBase.ScreenShake())
            {
                _cs.TurretShake();
            }


            if (health <= 0)
            {
                FindObjectOfType<AudioManager>().Pause("Footsteps");
                gameOverState.enabled = true;
                dead = true;
                this.GetComponent<PlayerAnimationController>().Die();
            }
        }
        



    }

    /**
     * Takes damage from the turret when the player collides with the turret.
     * The turret then explodes, taking away half the player's health, a
     * true tragedy.
     */
    public void TakeDamageFromTurret()
    {
        health = health / 2;
        healHealth = healHealth / 2;
        damageEffect.Play();
        damageLightEffect.Play();
        AudioManager.instance.Play("HitSound", Random.Range(0.9f, 1.1f));
        if (ColorDataBase.Vibration())
        {
            Handheld.Vibrate();
        }
        
        if (ColorDataBase.ScreenShake())
        {
            _cs.TurretShake();
        }
        // shouldn't ever end the round since it is always going to be half
        // of the player's health
    }

    /**
     * Updates what color we should be displaying on the Player object
     * based on what the Player's current health is.
     */
    private void UpdateHealthStatus(float health)
    {
        this.UpdateHealing();

        float healthMultiplier = 0.5f;

        var minRender = (1.15f - (health / startHealth)) * healthMultiplier;

        if (dead)
        {
            minRender = 1f;
        }

        healthIndicator.SetFloat("minimumRender", minRender);

        if (this.LowHealth())
        {
            this.AnimateEdgeWidth();
        }

        HealthBar.fillAmount = this.health / startHealth;
    }


    /**
     * Returns if we have low health or not.
     */
    private bool LowHealth()
    {
        return health / startHealth < 0.3f;
    }

    /**
     * Updates the healing display to give gradual health gain upon
     * picking up a health pack.
     */
    private void UpdateHealing()
    {

        if (health < healHealth)
        {
            healEffect.Play();
            healEffect1.Play();
            healLightFX.Play();
            // brings us back up to normal edge with if we were at low health
            float currWidth = edgeWidth;
            edgeWidth = Mathf.Lerp(currWidth, 0.03f, Time.deltaTime * 0.7f);

            float healingMultiplier = 1f;
            health = Mathf.Lerp(health, healHealth, this.GetHealOverTime(Time.deltaTime * healingMultiplier));
            healthIndicator.SetFloat("edgeWidth", edgeWidth);

            // a little hacky but helps the lerp stop executing
            if (healHealth - health <= 0.5f)
            {
                this.FinishHealing();
            }
        }

    }

    /**
     * Helps set the variables back its 4 am im tired but grinding.
     */
    private void FinishHealing()
    {
        healEffect.Stop();
        healEffect1.Stop();
        health = healHealth;
        edgeWidth = 0.03f;
        healthIndicator.SetFloat("edgeWidth", edgeWidth);
    }

    /**
     * Function for how the update healing effect works with respect
     * to time.
     * 
     * @param x The variable being passed in.
     */
    private float GetHealOverTime(float x)
    {
        float y = 2*x;
        return y;
    }

    /**
     * Heals the hero character.
     * 
     * @param healthIncrease The amount we are increasing the health by.
     */
    public void Heal(float healthIncrease)
    {
        var newHealth = health + healthIncrease;
        if (newHealth > startHealth)
        {
            healHealth = startHealth;
        }
        else
        {
            healHealth = newHealth;
        }
    }

    /**
     * Animates the width of the edge for Player object's material.
     * Used when the player is at low health to give a pulsating effect.
     */
    private void AnimateEdgeWidth()
    {
        float speed = 0.7f;
        edgeWidth = Mathf.Lerp(min, max, t);
        healthIndicator.SetFloat("edgeWidth", edgeWidth);

        t += speed * Time.deltaTime;

        if (t > 1f)
        {
            var temp = max;
            max = min;
            min = temp;
            t = 0f;
        }
    }

    /**
     * Called when user quits the application.
     */
    public void OnApplicationQuit()
    {
        this.setDefaultMats();
    }

    /**
     * Returns the material back to its default state.
     */
    private void setDefaultMats()
    {
        healthIndicator.SetColor("dissolveColor",
            ColorDataBase.GetCurrentHeroColor());
        healthIndicator.SetFloat("edgeWidth", 0.03f);
        healthIndicator.SetFloat("noiseScale", 120f);
        healthIndicator.SetFloat("minimumRender", 0.15f);
    }

}
