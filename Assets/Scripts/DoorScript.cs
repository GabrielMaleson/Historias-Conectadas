using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class DoorScript : MonoBehaviour
{
    public DialogueRunner dialogue;
    public void EnterDoor()
    {
        // Get the InventoryManager instance
        InventoryManager inventoryManager = InventoryManager.Instance;

        if (inventoryManager != null && inventoryManager.gameProgress.Contains("clarification"))
        {
            SceneManager.LoadScene("Hallway");
        }
        else
        {
            dialogue.StartDialogue("livingroomnah");
        }
    }
}
