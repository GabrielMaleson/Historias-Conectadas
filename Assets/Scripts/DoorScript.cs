using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    public bool DoorOpened = false;

    public void EnterDoor()
    {
        if (DoorOpened)
        {
            Debug.Log("Door entered");
            SceneManager.LoadScene("Kitchen");
        }

        else
        {
            return;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        InventorySlot item = collision.GetComponent<InventorySlot>();
        GameObject inventoryManager = GameObject.FindGameObjectWithTag("Inventory");

        if (item != null && item.slotTags.Contains("Door Key"))
        {
            if (!DoorOpened)
            {
                DoorOpened = true;
                Debug.Log("Door opened with key!");
            }
        }
    }
}
