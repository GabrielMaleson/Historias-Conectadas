using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI; // Added for Button component

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    [SerializeField] private string saveFileName = "savegame.dat";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    private class SaveData
    {
        public string savedSceneName;
        public List<string> savedItemNames = new List<string>();
        public List<string> savedGameProgress = new List<string>();
    }

    public void SaveGame()
    {
        // Get references to the inventory manager
        InventoryManager inventory = InventoryManager.Instance;
        if (inventory == null)
        {
            Debug.LogError("InventoryManager not found!");
            return;
        }

        // Create save data
        SaveData data = new SaveData();

        // Save current scene name
        data.savedSceneName = SceneManager.GetActiveScene().name;

        // Save items (store by name for reference)
        foreach (InventoryItem item in inventory.GetAllItems())
        {
            data.savedItemNames.Add(item.itemName);
        }

        // Save game progress
        data.savedGameProgress = new List<string>(inventory.gameProgress);

        // Serialize and save to file
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetSavePath();

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, data);
        }

        Debug.Log($"Game saved in scene: {data.savedSceneName}");

    }

    public void LoadGame()
    {
        string path = GetSavePath();

        if (!File.Exists(path))
        {
            Debug.LogError("No save file found!");
            return;
        }

        // Deserialize save data
        BinaryFormatter formatter = new BinaryFormatter();
        SaveData data = null;

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            data = (SaveData)formatter.Deserialize(stream);
        }

        if (data == null)
        {
            Debug.LogError("Failed to load save data!");
            return;
        }

        if (data.savedSceneName == "Intro Bus")
        {
            Debug.Log("guy is still on intro bus lol");
        }
        // Load the saved scene
        else
        {
            Debug.Log("did load");
            SceneManager.LoadScene("InventoryStuff", LoadSceneMode.Additive);
        }
        SceneManager.LoadScene(data.savedSceneName);

        // Clear current inventory and progress
        InventoryManager inventory = InventoryManager.Instance;
        if (inventory == null)
        {
            Debug.LogError("InventoryManager not found!");
            return;
        }

        // Clear current items (you might need to add a ClearItems() method to InventoryManager)
        List<InventoryItem> currentItems = new List<InventoryItem>(inventory.GetAllItems());
        foreach (InventoryItem item in currentItems)
        {
            inventory.RemoveItem(item);
        }

        // Clear game progress
        inventory.gameProgress.Clear();

        // Load saved items (you'll need a way to get InventoryItem by name)
        foreach (string itemName in data.savedItemNames)
        {
            InventoryItem item = GetItemByName(itemName);
            if (item != null)
            {
                inventory.AddItem(item);
            }
            else
            {
                Debug.LogWarning($"Item not found: {itemName}");
            }
        }

        // Load saved progress
        foreach (string progress in data.savedGameProgress)
        {
            inventory.AddProgress(progress);
        }

        Debug.Log($"Game loaded from scene: {data.savedSceneName}");
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, saveFileName);
    }

    // Helper method to find items by name (you might want to implement this differently)
    private InventoryItem GetItemByName(string itemName)
    {
        InventoryItem[] allItems = Resources.FindObjectsOfTypeAll<InventoryItem>();
        foreach (InventoryItem item in allItems)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }
        return null;
    }

    public bool SaveExists()
    {
        return File.Exists(GetSavePath());
    }

    public void DeleteSave()
    {
        string path = GetSavePath();
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted");

        }
    }
}