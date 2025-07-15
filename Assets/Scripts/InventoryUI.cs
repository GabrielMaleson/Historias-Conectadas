using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemsParent;
    [SerializeField] private GameObject inventorySlotPrefab;

    private List<GameObject> slots = new List<GameObject>();

    private void Start()
    {
        InventoryManager.Instance.OnItemAdded.AddListener(AddItemToUI);

        // Initialize with existing items
        foreach (var item in InventoryManager.Instance.GetAllItems())
        {
            AddItemToUI(item);
        }
    }

    private void AddItemToUI(InventoryItem item)
    {
        GameObject slot = Instantiate(inventorySlotPrefab, itemsParent);
        slots.Add(slot);

        InventorySlot slotScript = slot.GetComponent<InventorySlot>();
        if (slotScript != null)
        {
            slotScript.Setup(item);
        }
    }

    public void ClearInventoryUI()
    {
        foreach (var slot in slots)
        {
            Destroy(slot);
        }
        slots.Clear();
    }
}