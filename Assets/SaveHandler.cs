using UnityEngine;
using UnityEngine.UI;

public class GameSaveHandler : MonoBehaviour
{
    /// <summary>
    /// Saves the current game state
    /// </summary>
    public void SaveGame()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.SaveGame();
            Debug.Log("Game saved successfully!");

        }
        else
        {
            Debug.LogError("SaveLoadManager instance not found! Make sure it exists in the scene.");
        }
    }

    /// <summary>
    /// Loads the saved game state
    /// </summary>
    public void LoadGame()
    {
        if (SaveLoadManager.Instance != null)
        {
            if (SaveLoadManager.Instance.SaveExists())
            {
                SaveLoadManager.Instance.LoadGame();
                Debug.Log("Game loading initiated...");
            }
            else
            {
                Debug.LogWarning("No save file exists to load!");

                // Optional: Show UI feedback to player
                ShowNoSaveFileMessage();
            }
        }
        else
        {
            Debug.LogError("SaveLoadManager instance not found! Make sure it exists in the scene.");
        }
    }

    /// <summary>
    /// Deletes the current save file
    /// </summary>
    public void DeleteSave()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.DeleteSave();
            Debug.Log("Save file deleted!");

        }
        else
        {
            Debug.LogError("SaveLoadManager instance not found!");
        }
    }

    /// <summary>
    /// Checks if a save file exists
    /// </summary>
    public bool SaveExists()
    {
        if (SaveLoadManager.Instance != null)
        {
            return SaveLoadManager.Instance.SaveExists();
        }

        Debug.LogError("SaveLoadManager instance not found!");
        return false;
    }


    /// <summary>
    /// Can be called from other scripts to trigger a save
    /// </summary>
    public static void QuickSave()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.SaveGame();
            Debug.Log("Quick save completed!");
        }
    }

    /// <summary>
    /// Can be called from other scripts to trigger a load
    /// </summary>
    public static void QuickLoad()
    {
        if (SaveLoadManager.Instance != null && SaveLoadManager.Instance.SaveExists())
        {
            SaveLoadManager.Instance.LoadGame();
            Debug.Log("Quick load initiated...");
        }
    }

    /// <summary>
    /// Optional method to show UI feedback when no save file exists
    /// </summary>
    private void ShowNoSaveFileMessage()
    {
        // You can implement UI feedback here, such as:
        // - Showing a popup message
        // - Playing a sound
        // - Displaying text on screen

        Debug.Log("Show some UI feedback that no save file exists");
    }

    // Optional: Auto-save functionality
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // Auto-save when app is paused (mobile devices)
        if (pauseStatus)
        {
            // SaveGame();
        }
    }
}