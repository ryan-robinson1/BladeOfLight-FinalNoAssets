using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDisplaySwapper : MonoBehaviour
{
    public List<GameObject> heroDSwords;
    private int ind;

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        ind = PlayerPrefs.GetInt("equipped-sword", 0);
        for (int i = 0; i < heroDSwords.Count; i++)
        {
            if (i == ind)
            {
                heroDSwords[i].transform.gameObject.SetActive(true);
            }
            else
            {
                heroDSwords[i].transform.gameObject.SetActive(false);
            }
        }
    }

    /**
     * Sets the sword viewed in the display.
     */
    public void SetDisplaySword(int id)
    {
        heroDSwords[ind].transform.gameObject.SetActive(false);
        ind = id;
        heroDSwords[ind].transform.gameObject.SetActive(true);
    }
}
