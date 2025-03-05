using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip bgMusic;
    public AudioClip shapePickUp;
    public AudioClip shapePlace;
    public AudioClip shapeRotate;
    public AudioClip shapeFlip;
    public AudioClip winSound;

    private float defaultMusicVolume;
    private bool isMusicEnabled = true;
    private bool isSfxEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        defaultMusicVolume = musicSource.volume;
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (isMusicEnabled && !musicSource.isPlaying)
        {
            musicSource.clip = bgMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void ToggleMenuMusic(bool isMenuOpen)
    {
        musicSource.volume = isMenuOpen ? defaultMusicVolume * 0.3f : defaultMusicVolume;
    }

    public void ToggleMusic()
    {
        isMusicEnabled = !isMusicEnabled;
        musicSource.mute = !isMusicEnabled;

        if (isMusicEnabled)
            PlayMusic();
        else
            musicSource.Stop();
    }

    public void ToggleSFX()
    {
        isSfxEnabled = !isSfxEnabled;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (isSfxEnabled && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayShapePickUp() => PlaySFX(shapePickUp);
    public void PlayShapePlace() => PlaySFX(shapePlace);
    public void PlayShapeRotate() => PlaySFX(shapeRotate);
    public void PlayShapeFlip() => PlaySFX(shapeFlip);
    public void PlayWinSound() => PlaySFX(winSound);
}


