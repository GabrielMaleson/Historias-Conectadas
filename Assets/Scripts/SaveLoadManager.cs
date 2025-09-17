using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    [SerializeField] private string saveFileName = "savegame.dat";
    private bool isLoading = false;

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
        InventoryManager inventory = InventoryManager.Instance;
        if (inventory == null)
        {
            Debug.LogError("InventoryManager not found during save!");
            return;
        }

        SaveData data = new SaveData();
        data.savedSceneName = SceneManager.GetActiveScene().name;

        foreach (InventoryItem item in inventory.GetAllItems())
        {
            data.savedItemNames.Add(item.itemName);
        }

        data.savedGameProgress = new List<string>(inventory.gameProgress);

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

        isLoading = true;
        StartCoroutine(LoadGameCoroutine(data));
    }

    private IEnumerator LoadGameCoroutine(SaveData data)
    {
        // Load the main scene first
        SceneManager.LoadScene(data.savedSceneName);

        // Wait for the scene to load completely
        yield return null;

        // Load additive scenes
        SceneManager.LoadScene("InventoryStuff", LoadSceneMode.Additive);
        SceneManager.LoadScene("PauseStuff", LoadSceneMode.Additive);

        // Wait for all scenes to finish loading
        yield return new WaitForSeconds(0.1f);

        // Find InventoryManager after scenes are loaded
        InventoryManager inventory = FindObjectOfType<InventoryManager>();
        if (inventory == null)
        {
            Debug.LogError("InventoryManager not found after scene load!");
            isLoading = false;
            yield break;
        }

        // Clear current items
        List<InventoryItem> currentItems = new List<InventoryItem>(inventory.GetAllItems());
        foreach (InventoryItem item in currentItems)
        {
            inventory.RemoveItem(item);
        }

        // Load saved items
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
        inventory.gameProgress.Clear();
        foreach (string progress in data.savedGameProgress)
        {
            inventory.AddProgress(progress);
        }

        Debug.Log($"Game loaded from scene: {data.savedSceneName}");
        isLoading = false;
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, saveFileName);
    }

    private InventoryItem GetItemByName(string itemName)
    {
        // Use Resources.Load to find items by name instead of searching all objects
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