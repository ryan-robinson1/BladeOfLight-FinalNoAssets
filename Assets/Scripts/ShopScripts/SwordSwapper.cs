using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * SwordSwapper is in charge of equipping and unequipping swords.
 * 
 * @author Maxfield Barden
 */

public class SwordSwapper : MonoBehaviour
{

    public List<GameObject> swords;
    public List<ParticleSystem> slashes;
    private int ind;
    private SwordSlashSpawner slashSpawner;

    /**
     * Sets the active sword.
     */
    public void EquipSword()
    {
        ind = PlayerPrefs.GetInt("equipped-sword", 0);
        for (int i = 0; i < swords.Count; i++)
        {
            if (i == ind)
            {
                swords[i].transform.gameObject.SetActive(true);
            }
            else
            {
                swords[i].transform.gameObject.SetActive(false);
            }
        }

        slashSpawner = this.GetComponent<SwordSlashSpawner>();
        slashSpawner.SetSlash();

    }

    /**
     * Start is called before the first frame update
     */
    private void Start()
    {
        EquipSword();
    }

    /**
     * Swaps out the swords when the player equips a new sword.
     */
    public void SwapSwords(int id)
    {
        swords[ind].transform.gameObject.SetActive(false);
        ind = id;
        swords[ind].transform.gameObject.SetActive(true);
        PlayerPrefs.SetInt("equipped-sword", ind);
        this.GetComponent<SwordSlashSpawner>().SetSlash();
    }

    /**
     * Gets the current active sword.
     */
    public GameObject GetActiveSword()
    {
        return swords[ind];
    }

    /**
     * Gets the current slash attached to the equipped sword model.
     * 
     * @return The slash effect attached to this sword.
     */
    public ParticleSystem GetActiveSlash()
    {
        // slash table should align with sword table, it's an easy solution since
        // we aren't adding a ton of swords
        return slashes[ind];
    }
}
