using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class CookingMinigame : MonoBehaviour
{
    // Timer settings
    public float timeRemaining = 300f; // 5 minutes in seconds
    public TMP_Text timerText;
    public string initialScene = "Kitchen";

    // Minigame state
    public bool isMinigameActive = false;

    // Player objectives
    public bool GotOnions = false;
    public bool GotSausages = false;
    public bool GotPeppers = false;
    public bool GotGarlic = false;
    public bool GotBacon = false;

    // Singleton pattern to persist across scenes
    private static CookingMinigame instance;

    private void Awake()
    {
        // Ensure only one instance exists and persist across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [YarnCommand("startcooking")]
    public void StartMinigame()
    {
        isMinigameActive = true;
        // Reset all objectives when starting a new minigame
        GotOnions = false;
        GotSausages = false;
        GotPeppers = false;
        GotGarlic = false;
        GotBacon = false;
    }

    private void Update()
    {
        if (!isMinigameActive) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();

            // Check for win condition
            if (GotOnions && GotSausages && GotPeppers && GotGarlic && GotBacon)
            {
                Debug.Log("They win");
                timeRemaining = 0; // Stop the timer
            }
        }
        else
        {
            // Timer has ended
            if (!GotOnions || !GotSausages || !GotPeppers || !GotGarlic || !GotBacon)
            {
                Debug.Log("They lose");
            }

            // Return to initial scene
            if (SceneManager.GetActiveScene().name != initialScene)
            {
                SceneManager.LoadScene(initialScene);
            }

            // Reset minigame state
            isMinigameActive = false;

            // Optionally destroy the timer after returning to kitchen
            Destroy(gameObject);
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Public methods to update the objectives
    public void SetOnions(bool value) { GotOnions = value; }
    public void SetSausages(bool value) { GotSausages = value; }
    public void SetPeppers(bool value) { GotPeppers = value; }
    public void SetGarlic(bool value) { GotGarlic = value; }
    public void SetBacon(bool value) { GotBacon = value; }
}