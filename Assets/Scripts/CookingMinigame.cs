using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class CookingMinigame : MonoBehaviour
{
    // Timer settings
    public float timeRemaining = 300f; // 5 minutes in seconds
    public float maxTime = 300f; // Store the initial time
    public Image timerImage; // Reference to the Image component with Fill Type set to Filled
    public string initialScene = "Kitchen";
    public GameObject Timer;
    public DialogueRunner dialogueRunner;
    public InventoryItem sopaQueimada;
    public InventoryItem sopaBoa;
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
            maxTime = timeRemaining; // Store the initial time as max time
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [YarnCommand("startcooking")]
    public void StartMinigame()
    {
        InventoryManager.Instance.AddProgress("Did soup");
        Timer.SetActive(true);
        isMinigameActive = true;
        timeRemaining = maxTime; // Reset timer to max time
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
                InventoryManager.Instance.AddItem(sopaBoa);
                timeRemaining = 0; // Stop the timer
                UpdateTimerDisplay(); // Update display one last time
                dialogueRunner.StartDialogue("wincooking");
            }
        }
        else
        {
            // Timer has ended
            if (!GotOnions || !GotSausages || !GotPeppers || !GotGarlic || !GotBacon)
            {
                Debug.Log("They lose");
                InventoryManager.Instance.AddItem(sopaQueimada);
                dialogueRunner.StartDialogue("losecooking");
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
        if (timerImage != null)
        {
            // Calculate fill amount (1 = full time remaining, 0 = no time remaining)
            timerImage.fillAmount = timeRemaining / maxTime;
        }
    }

    // Public methods to update the objectives
    public void SetOnions(bool value) { GotOnions = value; }
    public void SetSausages(bool value) { GotSausages = value; }
    public void SetPeppers(bool value) { GotPeppers = value; }
    public void SetGarlic(bool value) { GotGarlic = value; }
    public void SetBacon(bool value) { GotBacon = value; }
}