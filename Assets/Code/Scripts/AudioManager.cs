using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene load event
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
        LoadVolumeSettings();
        PlayMusic();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe when destroyed
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusic(); // Restart music on each level load
    }

    private void LoadVolumeSettings()
    {
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSlider != null)
        {
            musicSlider.value = musicSource.volume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = sfxSource.volume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    public void PlayMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop(); // Ensure the previous music stops before playing again
        }

        musicSource.clip = bgMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayShapePickUp() => PlaySFX(shapePickUp);
    public void PlayShapePlace() => PlaySFX(shapePlace);
    public void PlayShapeRotate() => PlaySFX(shapeRotate);
    public void PlayShapeFlip() => PlaySFX(shapeFlip);

    public void PlayWinSound()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }

        sfxSource.PlayOneShot(winSound);
        StartCoroutine(ResumeMusicAfterWinSound(winSound.length));
    }

    private IEnumerator ResumeMusicAfterWinSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        musicSource.UnPause();
    }
}
