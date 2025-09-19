using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer Groups")]
    public AudioMixerGroup musicGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    [Header("Debug")]
    public bool enableDebugLogs = true;

    private AudioSource persistentAudioSource;
    private List<AudioSource> currentSceneMusicSources = new List<AudioSource>();
    private bool shouldContinuePreviousMusic = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Create a persistent audio source for cross-scene fading
            persistentAudioSource = gameObject.AddComponent<AudioSource>();
            persistentAudioSource.outputAudioMixerGroup = musicGroup;
            persistentAudioSource.loop = true;
            persistentAudioSource.playOnAwake = false;

            SceneManager.sceneLoaded += OnSceneLoaded;

            DebugLog("AudioManager initialized with persistent audio source");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Find music sources in the initial scene
        FindCurrentSceneMusicSources();

        // Play any music in the starting scene
        if (currentSceneMusicSources.Count > 0)
        {
            DebugLog("Found " + currentSceneMusicSources.Count + " music sources in starting scene");
            foreach (AudioSource source in currentSceneMusicSources)
            {
                if (source.playOnAwake && !source.isPlaying)
                {
                    source.Play();
                    DebugLog("Playing music: " + source.clip.name);
                }
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DebugLog("Scene loaded: " + scene.name);

        // Capture current scene music sources
        FindCurrentSceneMusicSources();

        // If new scene has no music sources, continue previous music
        if (currentSceneMusicSources.Count == 0 && persistentAudioSource.isPlaying)
        {
            DebugLog("No music sources in new scene - continuing previous music");
            shouldContinuePreviousMusic = true;

            // Just ensure persistent audio is playing at normal volume
            persistentAudioSource.volume = 1f;
            return;
        }

        // If we have a previous song playing and new scene has music, fade it out
        if (persistentAudioSource.isPlaying && !shouldContinuePreviousMusic)
        {
            DebugLog("Fading out previous music: " + persistentAudioSource.clip.name);
            StartCoroutine(FadeOutPreviousMusic());
        }
        else
        {
            DebugLog("No previous music to fade out");
        }

        // Play new scene music if any exists
        if (currentSceneMusicSources.Count > 0)
        {
            DebugLog("Fading in " + currentSceneMusicSources.Count + " new music sources");
            shouldContinuePreviousMusic = false;
            StartCoroutine(FadeInNewMusic());
        }
        else if (shouldContinuePreviousMusic)
        {
            DebugLog("Continuing previous scene's music: " + persistentAudioSource.clip.name);
        }
        else
        {
            DebugLog("No music sources found in new scene and no previous music to continue");
        }
    }

    void FindCurrentSceneMusicSources()
    {
        currentSceneMusicSources.Clear();
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in allAudioSources)
        {
            // Skip our persistent audio source and null objects
            if (source == null || source == persistentAudioSource) continue;

            // Check if this is a music source (either has music group or plays on awake)
            if (source.outputAudioMixerGroup == musicGroup || source.playOnAwake)
            {
                currentSceneMusicSources.Add(source);
                DebugLog("Found music source: " + source.name + " with clip: " + (source.clip != null ? source.clip.name : "None"));
            }
        }
    }

    // Call this before scene change to capture current music state
    public void PrepareForSceneChange()
    {
        DebugLog("Preparing for scene change...");
        shouldContinuePreviousMusic = false;

        // Find any currently playing music in the scene
        AudioSource currentlyPlaying = FindCurrentlyPlayingMusic();

        if (currentlyPlaying != null)
        {
            DebugLog("Capturing music state: " + currentlyPlaying.clip.name + " at time: " + currentlyPlaying.time);

            // Transfer to persistent audio source
            persistentAudioSource.clip = currentlyPlaying.clip;
            persistentAudioSource.time = currentlyPlaying.time;
            persistentAudioSource.volume = currentlyPlaying.volume;
            persistentAudioSource.loop = currentlyPlaying.loop;
            persistentAudioSource.Play();

            // Stop the original source
            currentlyPlaying.Stop();

            DebugLog("Persistent audio source now playing: " + persistentAudioSource.clip.name);
        }
        else if (persistentAudioSource.isPlaying)
        {
            DebugLog("No scene music found, but persistent audio is already playing: " + persistentAudioSource.clip.name);
            // Keep the persistent audio playing as-is
        }
        else
        {
            DebugLog("No music currently playing to capture");
        }
    }

    private AudioSource FindCurrentlyPlayingMusic()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in allAudioSources)
        {
            if (source != persistentAudioSource && source.isPlaying &&
                (source.outputAudioMixerGroup == musicGroup || source.playOnAwake))
            {
                return source;
            }
        }

        return null;
    }

    private IEnumerator FadeOutPreviousMusic()
    {
        if (!persistentAudioSource.isPlaying) yield break;

        float currentTime = 0;
        float startVolume = persistentAudioSource.volume;

        DebugLog("Starting fade out from volume: " + startVolume);

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            persistentAudioSource.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeDuration);
            yield return null;
        }

        persistentAudioSource.Stop();
        persistentAudioSource.volume = startVolume; // Reset volume for next time
        DebugLog("Fade out completed");
    }

    private IEnumerator FadeInNewMusic()
    {
        // Wait a frame to ensure scene is fully loaded
        yield return null;

        if (currentSceneMusicSources.Count == 0) yield break;

        DebugLog("Starting fade in for " + currentSceneMusicSources.Count + " sources");

        foreach (AudioSource source in currentSceneMusicSources)
        {
            if (source != null && source.clip != null)
            {
                float targetVolume = source.volume; // Store original volume
                source.volume = 0f; // Start muted

                if (!source.isPlaying)
                {
                    source.Play();
                    DebugLog("Started playing: " + source.clip.name);
                }

                StartCoroutine(FadeSingleSource(source, targetVolume));
            }
        }
    }

    private IEnumerator FadeSingleSource(AudioSource source, float targetVolume)
    {
        float currentTime = 0;
        float startVolume = source.volume;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / fadeDuration);
            yield return null;
        }

        source.volume = targetVolume; // Ensure exact target volume
        DebugLog("Fade in completed for: " + source.clip.name);
    }

    // Public method to manually stop continuing music if needed
    public void StopContinuingMusic()
    {
        if (shouldContinuePreviousMusic && persistentAudioSource.isPlaying)
        {
            StartCoroutine(FadeOutPreviousMusic());
            shouldContinuePreviousMusic = false;
        }
    }

    // Public method to manually start continuing music if needed
    public void ContinueMusicInThisScene()
    {
        if (persistentAudioSource.isPlaying && !shouldContinuePreviousMusic)
        {
            shouldContinuePreviousMusic = true;
            persistentAudioSource.volume = 1f;
        }
    }

    private void DebugLog(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[AudioManager] " + message);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    // Helper method to check what's happening
    public void DebugStatus()
    {
        Debug.Log("=== AudioManager Status ===");
        Debug.Log("Persistent source playing: " + persistentAudioSource.isPlaying);
        Debug.Log("Persistent clip: " + (persistentAudioSource.clip != null ? persistentAudioSource.clip.name : "None"));
        Debug.Log("Current scene sources: " + currentSceneMusicSources.Count);
        Debug.Log("Continue previous music: " + shouldContinuePreviousMusic);

        foreach (AudioSource source in currentSceneMusicSources)
        {
            if (source != null)
            {
                Debug.Log(" - " + source.name + ": " + (source.isPlaying ? "Playing" : "Not playing") +
                         ", Clip: " + (source.clip != null ? source.clip.name : "None"));
            }
        }
    }
}