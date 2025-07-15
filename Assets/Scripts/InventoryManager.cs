using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private GameObject InvUI;

    private bool invtoggle = false;

    public UnityEvent<InventoryItem> OnItemAdded = new UnityEvent<InventoryItem>();

    private List<InventoryItem> items = new List<InventoryItem>();

    private void Awake()
    {
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
            if (invtoggle == false)
            {
                ToggleInventory(true);
            }
            else if (invtoggle == true)
            {
                ToggleInventory(false);
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
    public void ToggleInventory(bool invtoggle)
    {
        InvUI.SetActive(invtoggle);
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