using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class CookingMinigame : MonoBehaviour
{
    public string initialScene = "Kitchen";
    public DialogueRunner dialogueRunner;
    public InventoryItem soupItem;

    // Minigame state
    public bool isMinigameActive = false;

    // Player objectives - now tracked for sequence
    public bool GotOnions = false;
    public bool GotSausages = false;
    public bool GotPeppers = false;
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
        GotBacon = false;
    }

    private void Update()
    {
        if (!isMinigameActive) return;

        // Check for win condition - only when onion is added as the final ingredient
        // The dialogue is triggered in IngredientScript when onion is added
        if (GotOnions && GotSausages && GotPeppers && GotBacon)
        {
            CompleteMinigame();
        }
    }

    private void CompleteMinigame()
    {
        // Add the soup item to inventory
        InventoryManager.Instance.AddItem(soupItem);

        // Return to initial scene after a short delay to allow dialogue to finish
        if (SceneManager.GetActiveScene().name != initialScene)
        {
            Invoke("LoadInitialScene", 2f); // Wait 2 seconds before loading
        }

        // Reset minigame state
        isMinigameActive = false;

        // Optionally destroy the cooking minigame manager after returning to kitchen
        Destroy(gameObject, 3f); // Destroy after 3 seconds
    }

    private void LoadInitialScene()
    {
        SceneManager.LoadScene(initialScene);
    }

    // Public methods to update the objectives
    public void SetOnions(bool value) { GotOnions = value; }
    public void SetSausages(bool value) { GotSausages = value; }
    public void SetPeppers(bool value) { GotPeppers = value; }
    public void SetBacon(bool value) { GotBacon = value; }
}