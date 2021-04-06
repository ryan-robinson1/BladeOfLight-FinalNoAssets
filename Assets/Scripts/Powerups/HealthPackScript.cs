using UnityEngine;

/**
 * Script for the health pack object. Keeps track of its position with
 * respect to the hero and heals the hero upon being collided with.
 * 
 * @author Maxfield Barden
 */

public class HealthPackScript : MonoBehaviour
{
    private float healAmount = 25f;
    public Material canisterColor;
    DamageManager _dm;

    AudioManager _am;

    private GameObject healthCanister;
    public Transform objectToFollow;
    private float _removalDistance = 10f;
    private float positionDifference;

    // Start is called before the first frame update
    void Start()
    {
        canisterColor.SetColor("emissionColor", ColorDataBase.GetCurrentHeroColor());
        canisterColor.SetColor("baseColor", ColorDataBase.GetHeroAlbedo());
        objectToFollow = GameObject.FindGameObjectWithTag("Player").transform;

        _am = FindObjectOfType<AudioManager>();

        healthCanister = transform.GetChild(0).gameObject;

    }

    /**
     * Heals the player object upon being collided with.
     * 
     * @param collision The collision box we are detecting.
     */
    public void HealPlayer(GameObject hero)
    {
        if (PlayerPrefs.GetString("firsthealth", "locked") == "locked")
        {
            AchievementEvents.aEvents.UnlockAchievementTrigger("firsthealth");
        }
        _dm = hero.GetComponent<DamageManager>();
        if (_dm.health < _dm.startHealth)
        {
            _dm.Heal(healAmount);
            _am.Play("HealthRegen");
            Destroy(healthCanister);
        }
    }

    /**
     * Destroys the health pack on collision with hero.
     */
    private void Destruct()
    {
        if (-positionDifference > _removalDistance)
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        positionDifference = transform.position.x - (objectToFollow.position.x);
        Destruct();
    }
}
