using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class AmericioScript : MonoBehaviour
{
    public DialogueRunner dialogue;
    public InventoryItem phone;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        InventorySlot item = collision.GetComponent<InventorySlot>();
        GameObject inventoryManager = GameObject.FindGameObjectWithTag("Inventory");

        if (item != null && item.slotTags.Contains("Phone"))
        {
            DoGoodThing(phone);
            dialogue.StartDialogue("americiosuccess");
        }
    }
    private void DoGoodThing(InventoryItem item)
    {
        dialogue.StartDialogue("americiosucess");
        InventoryManager.Instance.RemoveItem(item);
    }
}
