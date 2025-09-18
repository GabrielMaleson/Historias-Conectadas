using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer References")]
    public AudioMixerGroup masterGroup;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    [Header("Settings")]
    public float defaultFadeDuration = 1.0f;
    public float sceneTransitionFadeDuration = 1.5f;

    private List<AudioSource> musicSources = new List<AudioSource>();
    private List<AudioSource> sfxSources = new List<AudioSource>();
    private Dictionary<AudioSource, float> originalVolumes = new Dictionary<AudioSource, float>();
    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find all audio sources in the new scene and categorize them
        FindAndCategorizeAudioSources();
    }

    void FindAndCategorizeAudioSources()
    {
        // Clear previous lists
        musicSources.Clear();
        sfxSources.Clear();
        originalVolumes.Clear();

        // Find all active audio sources in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in allAudioSources)
        {
            // Skip if no output group is assigned
            if (source.outputAudioMixerGroup == null) continue;

            // Store original volume for fade operations
            originalVolumes[source] = source.volume;

            // Categorize based on mixer group
            if (source.outputAudioMixerGroup == musicGroup)
            {
                musicSources.Add(source);
            }
            else if (source.outputAudioMixerGroup == sfxGroup ||
                     source.outputAudioMixerGroup == masterGroup)
            {
                sfxSources.Add(source);
            }
        }
    }

    // New method: Load scene with audio fade transition
    public void LoadSceneWithFade(string sceneName, float fadeDuration = -1)
    {
        if (isTransitioning) return;

        float duration = fadeDuration > 0 ? fadeDuration : sceneTransitionFadeDuration;
        StartCoroutine(SceneTransitionWithFade(sceneName, duration));
    }

    private IEnumerator SceneTransitionWithFade(string sceneName, float fadeDuration)
    {
        isTransitioning = true;

        // Fade out all music
        yield return StartCoroutine(FadeAllMusic(fadeDuration, 0f));

        // Stop all music completely after fade
        foreach (AudioSource source in musicSources)
        {
            if (source.isPlaying)
            {
                source.Stop();
                source.volume = originalVolumes[source]; // Reset volume
            }
        }

        // Load the new scene
        SceneManager.LoadScene(sceneName);

        // Wait one frame for the new scene to initialize
        yield return null;

        // Find new audio sources in the scene
        FindAndCategorizeAudioSources();

        // Fade in any music that should be playing in the new scene
        yield return StartCoroutine(FadeAllMusic(fadeDuration, 1f));

        isTransitioning = false;
    }

    // Enhanced fade coroutine that returns IEnumerator
    private IEnumerator FadeAllMusic(float duration, float targetVolume)
    {
        List<Coroutine> fadeCoroutines = new List<Coroutine>();

        // Start all fade operations
        foreach (AudioSource source in musicSources)
        {
            if (source.isPlaying || targetVolume > 0) // Allow fading in even if not playing yet
            {
                if (targetVolume > 0 && !source.isPlaying)
                {
                    source.volume = 0; // Start from silent if fading in
                    source.Play();
                }

                fadeCoroutines.Add(StartCoroutine(FadeAudioSource(source, duration, targetVolume)));
            }
        }

        // Wait for all fades to complete
        foreach (Coroutine coroutine in fadeCoroutines)
        {
            yield return coroutine;
        }
    }

    private IEnumerator FadeAudioSource(AudioSource source, float duration, float targetVolume)
    {
        float currentTime = 0;
        float startVolume = source.volume;
        float actualTargetVolume = targetVolume * originalVolumes[source]; // Scale by original volume

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, actualTargetVolume, currentTime / duration);
            yield return null;
        }

        source.volume = actualTargetVolume;

        // If volume is zero, stop the audio (unless we're in the middle of a transition)
        if (targetVolume == 0 && !isTransitioning)
        {
            source.Stop();
            source.volume = originalVolumes[source]; // Reset to original volume
        }
    }

    public void StopAllMusicWithFade(float duration = 0.5f)
    {
        StartCoroutine(FadeAllMusic(duration, 0f));
    }

    public void PlayAllMusicWithFade(float duration = 0.5f)
    {
        StartCoroutine(FadeAllMusic(duration, 1f));
    }

    public void FadeAllSFX(float duration, float targetVolume)
    {
        foreach (AudioSource source in sfxSources)
        {
            if (source.isPlaying)
            {
                StartCoroutine(FadeAudioSource(source, duration, targetVolume));
            }
        }
    }

    // Clean up when destroyed
    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}