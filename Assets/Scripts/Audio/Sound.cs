using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound 
{

    public AudioClip clip;
    [Range(0f,1f)]
    public float volume;
    [Range(0.1f,3f)]
    public float pitch;

    [Range(0, 256)]
    public int priority;
    public string name;
    public bool loop;
    public bool playOnAwake;
    public bool mute;

    [HideInInspector]
    public AudioSource source;

    

}
