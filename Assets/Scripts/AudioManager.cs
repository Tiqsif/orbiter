using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton instance
    public float volume = 0.5f;
    public float musicVolume = 0.05f;
    public float ambienceVolume = 0.5f;
    public AudioClip musicClip;
    public AudioClip[] ambienceClips;
    private static AudioManager _instance;

    // Public instance to access the AudioManager
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Search for existing instance
                _instance = FindObjectOfType<AudioManager>();

                // If no instance found, create a new one
                if (_instance == null)
                {
                    GameObject singleton = new GameObject("AudioManager");
                    _instance = singleton.AddComponent<AudioManager>();
                    //DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // If an instance already exists and it's not this one, destroy this one
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Set the instance to this one
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        // Subscribe to the OnSceneLoaded event
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnSceneLoaded event
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Play the music clip when a new scene is loaded
        foreach (AudioClip ambienceClip in ambienceClips)
        {
            PlayAmbience(ambienceClip, true);
        }
        PlayMusic(musicClip, true);
    }


    private void Start()
    {
#if UNITY_EDITOR
        foreach (AudioClip ambienceClip in ambienceClips)
        {
            PlayAmbience(ambienceClip, true);
        }
        PlayMusic(musicClip, true);
#endif
    }



    // Method to play a sound effect
    public AudioSource PlaySFX(AudioClip clip, bool loop = false)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null. Cannot play sound.");
            return null;
        }
        //Debug.Log("Playing SFX "+ clip.name);
        // Create a temporary GameObject
        GameObject tempAudioObject = new GameObject("TempSFX: " + clip.name);
        tempAudioObject.transform.position = AudioManager.Instance.transform.position;
        tempAudioObject.transform.parent = AudioManager.Instance.transform;
        // Add an AudioSource component to the GameObject
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        // Set the clip to the AudioSource
        audioSource.clip = clip;

        // Play the clip
        audioSource.Play();

        // Destroy the GameObject after the clip has finished playing
        if (!loop) Destroy(tempAudioObject, clip.length);
        return audioSource;
    }


    public void PlaySFX(AudioClip clip, float delay)
    {
        StartCoroutine(PlaySFXRoutine(clip, delay));
    }

    public void PlaySFX(AudioClip clip, float delay, float volumeModifier)
    {
        StartCoroutine(PlaySFXRoutine(clip, delay, volumeModifier));
    }
    
    IEnumerator PlaySFXRoutine(AudioClip clip, float delay, float volumeModifier)
    {
        yield return new WaitForSeconds(delay);
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null. Cannot play sound.");
            yield break;
        }
        //Debug.Log("Playing SFX "+ clip.name);
        // Create a temporary GameObject
        GameObject tempAudioObject = new GameObject("TempSFX: " + clip.name);
        tempAudioObject.transform.position = AudioManager.Instance.transform.position;
        tempAudioObject.transform.parent = AudioManager.Instance.transform;
        // Add an AudioSource component to the GameObject
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.volume = volume * volumeModifier;
        // Set the clip to the AudioSource
        audioSource.clip = clip;

        // Play the clip
        audioSource.Play();

        // Destroy the GameObject after the clip has finished playing
        Destroy(tempAudioObject, clip.length);

    }
    
    IEnumerator PlaySFXRoutine(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySFX(clip);
    }

    /// <summary>
    /// kills any instance of the clip already playing
    /// </summary>
    /// <param name="clip"></param>
    public void KillSFX(AudioClip clip)
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out AudioSource audioSource))
            {
                if (audioSource.clip == clip)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    public AudioSource GetSFXSource(AudioClip clip)
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out AudioSource audioSource))
            {
                if (audioSource.clip == clip)
                {
                    return audioSource;
                }
            }
        }
        return null;
    }
    public void PlayMusic(AudioClip musicClip, bool isLooping = false)
    {
        if (musicClip == null)
        {
            Debug.LogWarning("AudioClip is null. Cannot play music.");
            return;
        }

        // Create a temporary GameObject
        GameObject tempAudioObject = new GameObject(musicClip.name);
        tempAudioObject.transform.position = AudioManager.Instance.transform.position;

        // Add an AudioSource component to the GameObject
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.volume = musicVolume;
        audioSource.loop = isLooping;
        // Set the clip to the AudioSource
        audioSource.clip = musicClip;

        // Play the clip
        audioSource.Play();

    }

    public void PlayAmbience(AudioClip ambienceClip, bool isLooping = false)
    {
        if (ambienceClip == null)
        {
            Debug.LogWarning("AudioClip is null. Cannot play music.");
            return;
        }

        // Create a temporary GameObject
        GameObject tempAudioObject = new GameObject(ambienceClip.name);
        tempAudioObject.transform.position = AudioManager.Instance.transform.position;

        // Add an AudioSource component to the GameObject
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.volume = ambienceVolume;
        audioSource.loop = isLooping;
        // Set the clip to the AudioSource
        audioSource.clip = ambienceClip;

        // Play the clip
        audioSource.Play();

    }
}