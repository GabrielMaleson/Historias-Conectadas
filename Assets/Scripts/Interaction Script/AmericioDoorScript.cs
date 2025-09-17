using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class AmericioDoorScript : MonoBehaviour
{
    public Button button;
    public DialogueRunner dialogue;
    public InventoryItem goodsoup;
    public InventoryItem badsoup;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        InventorySlot item = collision.GetComponent<InventorySlot>();
        GameObject inventoryManager = GameObject.FindGameObjectWithTag("Inventory");

        if (item != null && item.slotTags.Contains("Sopa Queimada"))
        {
            DoBadThing(badsoup);
            button.enabled = true;
        }

        else if (item != null && item.slotTags.Contains("Sopa Boa"))
        {
            DoGoodThing(goodsoup);
            button.enabled = true;
        }
    }
    private void DoGoodThing(InventoryItem item)
    {
        dialogue.StartDialogue("americioroomwin");
        InventoryManager.Instance.RemoveItem(item);
        InventoryManager.Instance.AddProgress("Good soup");
        InventoryManager.Instance.AddProgress("Gave soup");
    }
    private void DoBadThing(InventoryItem item)
    {
        dialogue.StartDialogue("americioroomlose");
        InventoryManager.Instance.RemoveItem(item);
        InventoryManager.Instance.AddProgress("Bad soup");
        InventoryManager.Instance.AddProgress("Gave soup");
    }
    public void EnterDoor()
    {
        // Get the InventoryManager instance
        InventoryManager inventoryManager = InventoryManager.Instance;

        if (inventoryManager != null && inventoryManager.gameProgress.Contains("Gave soup") && !inventoryManager.gameProgress.Contains("Euclides Time"))
        {
            SceneManager.LoadScene("Americio Room");
        }
        else if (inventoryManager.gameProgress.Contains("Gave soup") && inventoryManager.gameProgress.Contains("Euclides Time"))
        {
            dialogue.StartDialogue("americiodone");
        }
        else
        {
            dialogue.StartDialogue("americiono");
        }
    }

}
