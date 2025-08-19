using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

public class ClaraScript : MonoBehaviour
{
    public Button button;
    public DialogueRunner dialoguelol;
    public InventoryItem document;
    public InventoryItem goodsoup;
    public InventoryItem badsoup;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        InventorySlot item = collision.GetComponent<InventorySlot>();
        GameObject inventoryManager = GameObject.FindGameObjectWithTag("Inventory");

        if (item != null && item.slotTags.Contains("Document"))
        {
            Debug.Log("Door opened with key!");
            DoThing(document);
            button.enabled = true;
        }

        else if (item != null && item.slotTags.Contains("Sopa Queimada"))
        {
            Debug.Log("Door opened with key!");
            DoBadThing(badsoup);
            button.enabled = true;
        }

        else if (item != null && item.slotTags.Contains("Sopa Boa"))
        {
            Debug.Log("Door opened with key!");
            DoGoodThing(goodsoup);
            button.enabled = true;
        }
    }

    private void DoThing(InventoryItem item)
    {
        dialoguelol.StartDialogue("receptionsuccess");
        InventoryManager.Instance.RemoveItem(item);
    }
    private void DoGoodThing(InventoryItem item)
    {
        dialoguelol.StartDialogue("receptionplaytestwin");
        InventoryManager.Instance.RemoveItem(item);
    }
    private void DoBadThing(InventoryItem item)
    {
        dialoguelol.StartDialogue("receptionplaytestlose");
        InventoryManager.Instance.RemoveItem(item);
    }
}
