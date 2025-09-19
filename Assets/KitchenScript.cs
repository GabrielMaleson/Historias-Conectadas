using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class KitchenScript : MonoBehaviour
{
    public DialogueRunner dialogue;
    public InventoryButtonHandler itemload; 
    public void EnterDoor()
    {
        // Get the InventoryManager instance
        InventoryManager inventoryManager = InventoryManager.Instance;

        if (inventoryManager != null && inventoryManager.gameProgress.Contains("Did Cooking"))
        {
            dialogue.StartDialogue("lucibothered");
        }
        else
        {
            itemload = FindObjectOfType<InventoryButtonHandler>();
            itemload.LoadASceneItem("Kitchen");
        }
    }

}
