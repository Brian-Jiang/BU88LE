/*
 *  Instruction:
 *  Snap to an empty object
 *  Add sounds in inspector
 *  Call by: 
 *      // FindObjectOfType<AudioManager>().Play("SoundName");
 *  
 */




using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.clip;

            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.audioSource.Play();
    }
}
