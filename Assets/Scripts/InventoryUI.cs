using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform slotsParent; // Parent containing the slot GameObjects

    private InventorySlot[] slots; // Now an array for better performance

    private void Start()
    {
        // Get all existing slot components at start
        slots = slotsParent.GetComponentsInChildren<InventorySlot>(true);

        // Initialize all slots as empty
        foreach (var slot in slots)
        {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        InventoryManager.Instance.OnItemAdded.AddListener(AddItemToUI);
        InventoryManager.Instance.OnItemRemoved.AddListener(RemoveItemFromUI);

        // Initialize with existing items
        foreach (var item in InventoryManager.Instance.GetAllItems())
        {
            AddItemToUI(item);
        }
    }

    private void AddItemToUI(InventoryItem item)
    {
        // Find the first empty slot
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.Setup(item);

                // Only activate the slot if the inventory UI is currently active
                if (gameObject.activeInHierarchy)
                {
                    slot.gameObject.SetActive(true);
                }
                return;
            }
        }

        Debug.LogWarning("No empty slots available in inventory!");
    }

    private void RemoveItemFromUI(InventoryItem item)
    {
        // Find the slot containing this item and clear it
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.Item == item)
            {
                slot.ClearSlot();
                slot.gameObject.SetActive(false);
                return;
            }
        }
    }

    public void ClearInventoryUI()
    {
        foreach (var slot in slots)
        {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }
    }
}