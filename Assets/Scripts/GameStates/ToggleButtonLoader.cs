using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButtonLoader : MonoBehaviour
{
    public string playerPrefString;
    public string initialState;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString(playerPrefString, initialState) == "true")
        {
            GetComponent<SimpleToggleButton.ToggleButton>().SetToggleState(true);
        }
        else
        {
            GetComponent<SimpleToggleButton.ToggleButton>().SetToggleState(false);
        }
    }
}
