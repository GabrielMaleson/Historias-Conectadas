using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public UnityEvent<InventoryItem> OnItemAdded = new UnityEvent<InventoryItem>();

    private List<InventoryItem> items = new List<InventoryItem>();

    private void Awake()
    {
            DontDestroyOnLoad(gameObject);
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

    public bool HasItem(InventoryItem item)
    {
        return items.Contains(item);
    }

    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(items);
    }
}