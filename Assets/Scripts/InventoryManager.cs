using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private GameObject InvUI;

    public bool isInventoryOpen = false;
    public UnityEvent<InventoryItem> OnItemAdded = new UnityEvent<InventoryItem>();
    public UnityEvent<InventoryItem> OnItemRemoved = new UnityEvent<InventoryItem>();

    private List<InventoryItem> items = new List<InventoryItem>();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InvUI.SetActive(false);
        Canvas canvas = GetComponentInChildren<Canvas>();
        isInventoryOpen = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (canvas != null)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCanvas();
    }

    private void UpdateCanvas()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    private void StopCanvas() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

    public void RemoveItem(InventoryItem item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            OnItemRemoved.Invoke(item);
            Debug.Log($"Removed item: {item.itemName}");
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