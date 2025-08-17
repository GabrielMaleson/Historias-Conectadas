using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class ClaraScript : MonoBehaviour
{
    public DialogueRunner dialoguelol;
    public InventoryItem document;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        InventorySlot item = collision.GetComponent<InventorySlot>();
        GameObject inventoryManager = GameObject.FindGameObjectWithTag("Inventory");

        if (item != null && item.slotTags.Contains("Document"))
        {
            Debug.Log("Door opened with key!");
            DoThing(document);
        }
    }

    private void DoThing(InventoryItem item)
    {
        dialoguelol.StartDialogue("receptionsuccess");
        InventoryManager.Instance.RemoveItem(item);
    }
}
