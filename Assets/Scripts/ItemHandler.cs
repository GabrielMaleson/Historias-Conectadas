using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
public class InventoryButtonHandler : MonoBehaviour
{
    // Reference to the InventoryItem you want to add/remove
    [SerializeField] private InventoryItem item;
    [SerializeField] private GameObject fadein;

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
    public void FadeIn(GameObject fadein)
    {
        StartCoroutine(FadeInCoroutine(fadein));
    }

    public void AddProgressToInventory(string thing)
    {
        InventoryManager.Instance.AddProgress(thing);
    }
    public void RemoveProgressToInventory(string thing)
    {
        InventoryManager.Instance.RemoveProgress(thing);
    }


    private IEnumerator FadeInCoroutine(GameObject fadein)
    {
        Image image = fadein.GetComponent<Image>();

        float duration = 5f;
        float currentTime = 0f;

            Color color = image.color;
            color.a = 0f;
            image.color = color;
        // Fade in over time
        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / duration);
            {
                color.a = alpha;
                image.color = color;
            }

            currentTime += Time.deltaTime;
            yield return null;
        }
        {
            Color finalColor = image.color;
            finalColor.a = 1f;
            image.color = finalColor;
        }
    }
}

  

