using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform slotsParent; // Parent containing slot position GameObjects
    [SerializeField] private GameObject inventorySlotPrefab;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private List<Transform> slotPositions = new List<Transform>();

    private void Start()
    {
        // Get all slot position transforms
        foreach (Transform child in slotsParent)
        {
            slotPositions.Add(child);
        }

        // Initialize slots at each position
        InitializeSlots();

        InventoryManager.Instance.OnItemAdded.AddListener(AddItemToUI);
        InventoryManager.Instance.OnItemRemoved.AddListener(RemoveItemFromUI);

        // Initialize with existing items
        foreach (var item in InventoryManager.Instance.GetAllItems())
        {
            AddItemToUI(item);
        }
    }

    private void InitializeSlots()
    {
        // Create a slot at each predefined position
        foreach (var pos in slotPositions)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, pos.position, Quaternion.identity, pos);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            slotObj.SetActive(false); // Start inactive
            slots.Add(slot);
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
                slot.gameObject.SetActive(true); // Activate when item is added
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
            if (slot.Item == item)
            {
                slot.ClearSlot();
                slot.gameObject.SetActive(false); // Deactivate when item is removed
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