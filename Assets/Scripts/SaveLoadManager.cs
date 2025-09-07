using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
        // Note: You'll need to make gameProgress public or add a GetGameProgress() method to InventoryManager
        // For now, I'll assume you add this method to InventoryManager:
        // public List<string> GetGameProgress() { return new List<string>(gameProgress); }
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

        // Clear current inventory and progress
        InventoryManager inventory = InventoryManager.Instance;
        if (inventory == null)
        {
            Debug.LogError("InventoryManager not found!");
            return;
        }

        // Clear current items (you might need to add a ClearItems() method to InventoryManager)
        // For now, we'll remove items one by one from a copy of the list
        List<InventoryItem> currentItems = new List<InventoryItem>(inventory.GetAllItems());
        foreach (InventoryItem item in currentItems)
        {
            inventory.RemoveItem(item);
        }

        // Clear game progress (you might need to add a ClearProgress() method)
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

        // Load the saved scene
        SceneManager.LoadScene(data.savedSceneName);

        Debug.Log($"Game loaded from scene: {data.savedSceneName}");
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, saveFileName);
    }

    // Helper method to find items by name (you might want to implement this differently)
    private InventoryItem GetItemByName(string itemName)
    {
        // This is a simple implementation - you might want to create an ItemDatabase
        // or use Resources.Load if you have items in a Resources folder
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