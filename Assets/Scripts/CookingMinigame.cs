using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
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
    public bool GotBeans = false;
    public bool GotGarlic = false;
    public bool GrabbedOnions = false;
    public bool GrabbedSausages = false;
    public bool GrabbedPeppers = false;
    public bool GrabbedBacon = false;
    public bool GrabbedBeans = false;
    public bool GrabbedGarlic = false;

    public GameObject onionButton;
    private GameObject baconButton;
    private GameObject garlicButton;
    private GameObject beansButton;
    private GameObject pepperButton;
    private GameObject sausageButton;
    public GameObject kitchenButton;

    // Singleton pattern to persist across scenes
    public static CookingMinigame instance;

    private void Awake()
    {
        // Ensure only one instance exists and persist across scenes
        if (instance == null)
        {
            FindIngredientButtons();
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FindIngredientButtons()
    {
        // Find all objects with tags, even if disabled
        onionButton = FindObjectByTag("OnionButton");
        baconButton = FindObjectByTag("BaconButton");
        garlicButton = FindObjectByTag("GarlicButton");
        beansButton = FindObjectByTag("BeansButton");
        pepperButton = FindObjectByTag("PepperButton");
        sausageButton = FindObjectByTag("SausageButton");
        kitchenButton = FindObjectByTag("KitchenButton");
    }

    private GameObject FindObjectByTag(string tag)
    {
        // Find all objects with the tag, including inactive ones
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag(tag) && obj.scene.isLoaded)
            {
                return obj;
            }
        }
        return null;
    }

    [YarnCommand("startcooking")]
    public void StartMinigame()
    {
        InventoryManager.Instance.AddProgress("Did soup");
        isMinigameActive = true;
        kitchenButton.SetActive(true);

        // Reset all objectives when starting a new minigame
        GotOnions = false;
        GotSausages = false;
        GotPeppers = false;
        GotBacon = false;
        GotGarlic = false;
        GotBeans = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Use a coroutine to wait until the end of the frame when objects are fully loaded
        StartCoroutine(FindObjectsAfterLoad());
    }

    private IEnumerator FindObjectsAfterLoad()
    {
        // Wait until the end of the frame to ensure all objects are loaded
        yield return new WaitForEndOfFrame();

        FindIngredientButtons();
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        if (GrabbedBacon) 
        {
            baconButton.SetActive(false);
        }
        if (GrabbedBeans)
        {
            beansButton.SetActive(false);
        }
        if (GrabbedGarlic)
        {
            garlicButton.SetActive(false);
        }
        if (GrabbedSausages)
        {
            sausageButton.SetActive(false);
        }
        if (GrabbedPeppers)
        {
            pepperButton.SetActive(false);
        }
        if (GrabbedOnions)
        {
            onionButton.SetActive(false);
        }
        if (isMinigameActive)
        {
            kitchenButton.SetActive(true);
        }
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

        // Reset minigame state
        isMinigameActive = false;

        Destroy(gameObject, 3f); 
    }

    // Public methods to update the objectives
    public void SetOnions(bool value) { GotOnions = value; }
    public void SetSausages(bool value) { GotSausages = value; }
    public void SetPeppers(bool value) { GotPeppers = value; }
    public void SetBacon(bool value) { GotBacon = value; }
    public void SetGarlic(bool value) { GotGarlic = value; }
    public void SetBeans(bool value) { GotBeans = value; }
    public void GrabOnions(bool value) { GrabbedOnions = value; }
    public void GrabSausages(bool value) { GrabbedSausages = value; }
    public void GrabPeppers(bool value) { GrabbedPeppers = value; }
    public void GrabBacon(bool value) { GrabbedBacon = value; }
    public void GrabGarlic(bool value) { GrabbedGarlic = value; }
    public void GrabBeans(bool value) { GrabbedBeans = value; }
}