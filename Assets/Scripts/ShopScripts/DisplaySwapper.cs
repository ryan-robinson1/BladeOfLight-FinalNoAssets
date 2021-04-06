using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySwapper : MonoBehaviour
{
    public List<GameObject> displaySwords;
    private int ind;

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        ind = PlayerPrefs.GetInt("equipped-sword", 0);
        for (int i = 0; i < displaySwords.Count; i++)
        {
            if (i == ind)
            {
                displaySwords[i].transform.gameObject.SetActive(true);
            }
            else
            {
                displaySwords[i].transform.gameObject.SetActive(false);
            }
        }
    }

    /**
     * Sets the sword viewed in the display.
     */
    public void SetDisplaySword(int id)
    {
        displaySwords[ind].transform.gameObject.SetActive(false);
        ind = id;
        displaySwords[ind].transform.gameObject.SetActive(true);
    }
}
