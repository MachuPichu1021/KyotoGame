using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    private AudioSource source;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sfx)
    {
        source.PlayOneShot(sfx);
    }

    public void Stop()
    {
        source.Stop();
    }
}
