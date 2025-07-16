using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private GameObject InvUI;

    private bool isInventoryOpen = false; // Tracks the current state

    public UnityEvent<InventoryItem> OnItemAdded = new UnityEvent<InventoryItem>();

    private List<InventoryItem> items = new List<InventoryItem>();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure inventory starts closed
        InvUI.SetActive(false);
        isInventoryOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            // Toggle the state
            isInventoryOpen = !isInventoryOpen;
            InvUI.SetActive(isInventoryOpen);
        }
    }

    public void AddItem(InventoryItem item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            OnItemAdded.Invoke(item);
            Debug.Log($"Added item: {item.itemName}");
        }
    }

    public void ToggleInventory(bool state)
    {
        isInventoryOpen = state;
        InvUI.SetActive(state);
    }

    public bool HasItem(InventoryItem item)
    {
        return items.Contains(item);
    }

    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(items);
    }
}