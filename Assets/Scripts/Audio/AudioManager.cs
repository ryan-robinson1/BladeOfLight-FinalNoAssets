using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        

      
    }
    public void initializeAudioManager()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.mute = s.mute;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.priority = s.priority;

            if (s.playOnAwake)
            {
                Play(s.name);
            }

        }

        // mute the music if player prefs is set to true
        if (PlayerPrefs.GetString("muteMusic", "false") == "true")
        {
            this.MuteAllSounds();
        }
    }

    /**
     * Returns the array of Sound objects.
     * 
     * @return The array of Sounds.
     */
    public Sound[] GetSounds()
    {
        return sounds;
    }

    /**
     * Mutes all the music in the game.
     */
    public void MuteAllSounds()
    {
        AudioListener.volume = 0;
        PlayerPrefs.SetString("muteMusic", "true");
    }
    public void MuteSoundsUnsaved()
    {
        AudioListener.volume = 0;
        Pause("MainTheme");

    }
    public void UnMuteSoundsUnsaved()
    {
        AudioListener.volume = 1;
        UnPause("MainTheme");


    }


    /**
     * Unmutes all the sounds in the game.
     */
    public void UnMuteAllSounds()
    {
        AudioListener.volume = 1;
        PlayerPrefs.SetString("muteMusic", "false");
    }

    /**
     * Pauses every sound playing, used for pausing the game.
     */
    public void PauseAllSounds(Sound[] sounds)
    {
        foreach (Sound sound in sounds)
        {
            sound.source.Pause();
            Debug.Log(sound.source.name);
        }
    }


    /**
     * Plays all sounds, used to resume the game.
     */
    public void UnPauseAllSounds(Sound[] sounds)
    {
        foreach (Sound sound in sounds)
        {
            sound.source.UnPause();
        }
    }

    /**
   * Given a CDF of weights,for example {0.3,0.7,1.0} and an array of sounds, choose a soundrandomly using the weights corresponding to their index
   * 
   * @param weightsCDF Array of floats that represent a CDF of weights
   * @param names The sound names
   * @param pitch The pitch of the sound
   * 
   */
    public void PlaySoundFromArray(string[] names, float[] weightCDF, float pitch)
    {

        if(names.Length == weightCDF.Length)
        {
            int index = ChooseWeightedIndex(weightCDF);
            Play(names[index], pitch);
        }
        else
        {
            Debug.LogError("Error: weightCDF length and names length must be the same");
        }
        
     

    }
    public void Play(string name)
    {
       Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        try
        {
            s.source.Play();
        }
        catch
        {

        }
      
    }
    public void Play(string name, float pitch)
    {
        try
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }
            s.source.pitch = pitch;
            // s.source.Play();


            s.source.PlayOneShot(s.source.clip);
        }
        catch
        {

        }
       
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop(); 
    }
    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Pause();
    }
    public void UnPause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.UnPause();
    }
    /**
     * Given a CDF of weights, for example {0.3,0.7,1.0}, return a random index weighted with the probability at said index
     * 
     * @param weightsCDF Array of floats that should represent a CDF
     * 
     * @return Index with probability at index
     */
    private int ChooseWeightedIndex(float[] weightsCDF)
    {
        System.Random r = new System.Random();
        double random = r.NextDouble();
        
       for(int i = 0; i < weightsCDF.Length; i++)
        {
            if (random < weightsCDF[i])
            {
                return i;
            }
        }
        return 0;
    }

}
