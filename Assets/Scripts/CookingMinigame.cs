using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class CookingMinigame : MonoBehaviour
{
    public string initialScene = "Kitchen";
    public DialogueRunner dialogueRunner;
    public InventoryItem soupItem; // Changed from sopaBoa to generic soupItem

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
        InventoryManager.Instance.AddProgress("Did soup");
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

        // Check for win condition
        if (GotOnions && GotSausages && GotPeppers && GotGarlic && GotBacon)
        {
            Debug.Log("They win");
            InventoryManager.Instance.AddItem(soupItem);
            dialogueRunner.StartDialogue("wincooking");
            CompleteMinigame();
        }
    }

    private void CompleteMinigame()
    {
        // Return to initial scene
        if (SceneManager.GetActiveScene().name != initialScene)
        {
            SceneManager.LoadScene(initialScene);
        }

        // Reset minigame state
        isMinigameActive = false;

        // Optionally destroy the cooking minigame manager after returning to kitchen
        Destroy(gameObject);
    }

    // Public methods to update the objectives
    public void SetOnions(bool value) { GotOnions = value; }
    public void SetSausages(bool value) { GotSausages = value; }
    public void SetPeppers(bool value) { GotPeppers = value; }
    public void SetGarlic(bool value) { GotGarlic = value; }
    public void SetBacon(bool value) { GotBacon = false; }
}