using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip backgroundMusic;

    [Header("SFX Clips")]
    public AudioClip footstepSound;
    public AudioClip doorSound;
    public AudioClip pickupSound;
    public AudioClip flashlightClick;
    public AudioClip deathScream;
    public AudioClip grannyFootstep;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMusic(backgroundMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Quick access methods
    public void PlayFootstep()
    {
        PlaySFX(footstepSound);
    }

    public void PlayDoor()
    {
        PlaySFX(doorSound);
    }

    public void PlayPickup()
    {
        PlaySFX(pickupSound);
    }

    public void PlayFlashlightClick()
    {
        PlaySFX(flashlightClick);
    }

    public void PlayDeath()
    {
        PlaySFX(deathScream);
    }

    public void PlayGrannyFootstep()
    {
        PlaySFX(grannyFootstep);
    }
}