using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;
using System;
using System.Collections.Generic;

[System.Serializable]
public class CookingMinigameState
{
    public bool isMinigameActive = false;
    public bool GotOnions = false;
    public bool GotSausages = false;
    public bool GotPeppers = false;
    public bool GotBacon = false;
    public bool GotGarlic = false;
    public bool GotBeans = false;

    // Track which ingredients have been obtained (for UI button states)
    public List<string> obtainedIngredients = new List<string>();
}

public class CookingMinigame : MonoBehaviour
{
    public string initialScene = "Kitchen";
    public DialogueRunner dialogueRunner;
    public InventoryItem soupItem;

    // Reference to ingredient buttons (assign in inspector or find dynamically)
    public GameObject onionButton;
    public GameObject baconButton;
    public GameObject garlicButton;
    public GameObject beansButton;
    public GameObject pepperButton;
    public GameObject sausageButton;

    // Singleton pattern to persist across scenes
    private static CookingMinigame instance;

    // Persistent state
    private CookingMinigameState state = new CookingMinigameState();

    // Public properties to access state
    public bool isMinigameActive
    {
        get => state.isMinigameActive;
        set => state.isMinigameActive = value;
    }

    public bool GotOnions
    {
        get => state.GotOnions;
        set { state.GotOnions = value; if (value) AddObtainedIngredient("Onion"); }
    }

    public bool GotSausages
    {
        get => state.GotSausages;
        set { state.GotSausages = value; if (value) AddObtainedIngredient("Sausage"); }
    }

    public bool GotPeppers
    {
        get => state.GotPeppers;
        set { state.GotPeppers = value; if (value) AddObtainedIngredient("Pepper"); }
    }

    public bool GotBacon
    {
        get => state.GotBacon;
        set { state.GotBacon = value; if (value) AddObtainedIngredient("Bacon"); }
    }

    public bool GotGarlic
    {
        get => state.GotGarlic;
        set { state.GotGarlic = value; if (value) AddObtainedIngredient("Garlic"); }
    }

    public bool GotBeans
    {
        get => state.GotBeans;
        set { state.GotBeans = value; if (value) AddObtainedIngredient("Beans"); }
    }

    private void Awake()
    {
        // Ensure only one instance exists and persist across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find ingredient buttons in the scene
        FindIngredientButtons();

        // When returning to the cooking scene, restore all states
        if (scene.name != initialScene)
        {
            RestoreButtonStates();
            RestoreIngredientButtonStates();
        }
    }


private void FindIngredientButtons()
    {
        // Assign these tags to your button GameObjects in the Inspector
        onionButton = GameObject.FindWithTag("OnionButton");
        baconButton = GameObject.FindWithTag("BaconButton");
        garlicButton = GameObject.FindWithTag("GarlicButton");
        beansButton = GameObject.FindWithTag("BeansButton");
        pepperButton = GameObject.FindWithTag("PepperButton");
        sausageButton = GameObject.FindWithTag("SausageButton");
    }

    private void AddObtainedIngredient(string ingredientName)
    {
        if (!state.obtainedIngredients.Contains(ingredientName))
        {
            state.obtainedIngredients.Add(ingredientName);
        }
    }

    private void RestoreIngredientButtonStates()
    {
        // Set ingredient buttons inactive if they've already been obtained
        foreach (var ingredient in state.obtainedIngredients)
        {
            SetIngredientButtonState(ingredient, false);
        }
    }

    private void SetIngredientButtonState(string ingredientName, bool active)
    {
        GameObject button = null;

        switch (ingredientName)
        {
            case "Onion": button = onionButton; break;
            case "Bacon": button = baconButton; break;
            case "Garlic": button = garlicButton; break;
            case "Beans": button = beansButton; break;
            case "Pepper": button = pepperButton; break;
            case "Sausage": button = sausageButton; break;
        }

        if (button != null)
        {
            button.SetActive(active);

            // Also disable the button component if the object is still active
            var buttonComponent = button.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.enabled = active;
            }
        }
    }

    private void RestoreButtonStates()
    {
        // Find all ingredient scripts and disable their buttons if minigame is active
        var ingredientScripts = FindObjectsOfType<IngredientScript>();
        foreach (var script in ingredientScripts)
        {
            if (script != null && script.button != null)
            {
                script.button.enabled = !isMinigameActive;
            }
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
        GotGarlic = false;
        GotBeans = false;
        state.obtainedIngredients.Clear();

        // Disable interaction buttons immediately
        DisableAllButtons();

        // Make sure all ingredient buttons are active at start
        ResetIngredientButtons();
    }

    private void ResetIngredientButtons()
    {
        // Reactivate all ingredient buttons at the start of minigame
        SetIngredientButtonState("Onion", true);
        SetIngredientButtonState("Bacon", true);
        SetIngredientButtonState("Garlic", true);
        SetIngredientButtonState("Beans", true);
        SetIngredientButtonState("Pepper", true);
        SetIngredientButtonState("Sausage", true);
    }

    private void DisableAllButtons()
    {
        var ingredientScripts = FindObjectsOfType<IngredientScript>();
        foreach (var script in ingredientScripts)
        {
            if (script != null && script.button != null)
            {
                script.button.enabled = false;
            }
        }
    }

    private void Update()
    {
        if (!isMinigameActive) return;

        // Check for win condition - only when onion is added as the final ingredient
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

        // Re-enable buttons when minigame is complete
        EnableAllButtons();

        // Optionally destroy the cooking minigame manager after returning to kitchen
        Destroy(gameObject, 3f); // Destroy after 3 seconds
    }

    private void EnableAllButtons()
    {
        var ingredientScripts = FindObjectsOfType<IngredientScript>();
        foreach (var script in ingredientScripts)
        {
            if (script != null && script.button != null)
            {
                script.button.enabled = true;
            }
        }
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
    public void SetGarlic(bool value) { GotGarlic = value; }
    public void SetBeans(bool value) { GotBeans = value; }
}