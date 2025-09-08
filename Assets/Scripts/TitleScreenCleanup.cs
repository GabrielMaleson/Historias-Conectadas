using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenCleanup : MonoBehaviour
{
    void Start()
    {
        CleanupNonTitleScreenObjects();
    }

    public void CleanupNonTitleScreenObjects()
    {
        Time.timeScale = 1f;
        // Get all active GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> objectsToDestroy = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Don't destroy this cleanup script's object
            if (obj == gameObject) continue;

            // Don't destroy objects called "[Debug Updater]"
            if (obj.name == "[Debug Updater]") continue;

            // Check if this object or any of its parents have the TitleScreen tag
            if (IsInTitleScreenHierarchy(obj)) continue;

            // Don't destroy objects that are part of the title screen scene
            if (obj.scene.name == SceneManager.GetActiveScene().name) continue;

            objectsToDestroy.Add(obj);
        }

        // Destroy all identified objects
        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
            Debug.Log("Destroyed object: " + obj.name);
        }

        Debug.Log("Title screen cleanup completed. Destroyed " + objectsToDestroy.Count + " objects.");
    }

    private bool IsInTitleScreenHierarchy(GameObject obj)
    {
        // Check current object and all parents up the hierarchy
        Transform current = obj.transform;

        while (current != null)
        {
            if (current.CompareTag("TitleScreen"))
            {
                return true;
            }
            current = current.parent;
        }

        return false;
    }
}