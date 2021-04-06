using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientPlayerSound : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerAnimationController _pac;
    AudioManager _am;
    bool playingFootsteps = false;
    void Start()
    {
        _pac = GetComponent<PlayerAnimationController>();
        _am = FindObjectOfType<AudioManager>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (_pac.IsRunning() && !playingFootsteps){
            _am.Play("Footsteps");
            playingFootsteps = true;
        }
     /*   else
        {
            _am.Stop("Footsteps");
            playingFootsteps = false;
        }*/
    }
}
