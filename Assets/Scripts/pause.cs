using UnityEngine;
using UnityEngine.Audio; // Required for AudioMixer
using UnityEngine.UI;    // Required for Slider

public class GamePause : MonoBehaviour
{
    [Header("Pause Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private GameObject pauseMenuUI;

    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer; // Assign in inspector
    [SerializeField] private Slider volumeSlider;  // Assign in inspector
    [SerializeField] private string volumeParameter = "MasterVolume"; // Name of exposed parameter in AudioMixer

    private bool isPaused = false;
    private float previousVolume; // To store volume before mute

    private void Start()
    {
        // Initialize volume slider if assigned
        if (volumeSlider != null)
        {
            // Load saved volume or set default
            float savedVolume = PlayerPrefs.GetFloat(volumeParameter, 0.75f);
            SetVolume(savedVolume);
            volumeSlider.value = savedVolume;

            // Add listener for slider changes
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    public void PauseGame()
    {
        // Pause the game time
        Time.timeScale = 0f;

        // Set the pause state
        isPaused = true;

        // Show pause menu if assigned
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        // Resume normal game time
        Time.timeScale = 1f;

        // Set the pause state
        isPaused = false;

        // Hide pause menu if assigned
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void SetVolume(float volume)
    {
        // Convert linear 0-1 slider value to logarithmic dB scale
        float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;

        // Set the mixer volume
        if (audioMixer != null)
        {
            audioMixer.SetFloat(volumeParameter, dB);
        }

        // Save the volume setting
        PlayerPrefs.SetFloat(volumeParameter, volume);
        PlayerPrefs.Save();
    }

    // Optional: Mute toggle functionality
    public void ToggleMute()
    {
        if (volumeSlider != null)
        {
            if (volumeSlider.value > 0)
            {
                // Store current volume before muting
                previousVolume = volumeSlider.value;
                volumeSlider.value = 0;
            }
            else
            {
                // Restore previous volume
                volumeSlider.value = previousVolume;
            }
        }
    }
}