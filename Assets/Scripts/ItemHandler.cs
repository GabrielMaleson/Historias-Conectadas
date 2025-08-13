using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryButtonHandler : MonoBehaviour
{
    // Reference to the InventoryItem you want to add/remove
    [SerializeField] private InventoryItem item;

    // Method to add item to inventory
    public void AddItemToInventory(InventoryItem item)
    {
        InventoryManager.Instance.AddItem(item);
    }

    // Method to remove item from inventory
    public void RemoveItemFromInventory(InventoryItem item)
    {
        InventoryManager.Instance.RemoveItem(item);
    }

    // Method to toggle inventory UI
    public void ToggleInventory()
    {
        if (InventoryManager.Instance != null)
        {
            bool currentState = InventoryManager.Instance.GetAllItems().Count > 0; // Or use any other condition
            InventoryManager.Instance.ToggleInventory(!currentState);
        }
        else
        {
            Debug.LogError("InventoryManager instance not found!");
        }
    }
    public void LoadASceneItem(string scene)
    {
        SceneManager.LoadScene(scene);
    }

}

  

