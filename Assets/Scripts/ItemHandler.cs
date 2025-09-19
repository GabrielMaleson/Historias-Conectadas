using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class InventoryButtonHandler : MonoBehaviour
{
    [SerializeField] private InventoryItem item;
    [SerializeField] private GameObject fadein;

    public void AddItemToInventory(InventoryItem item)
    {
        InventoryManager.Instance.AddItem(item);
    }

    public void RemoveItemFromInventory(InventoryItem item)
    {
        InventoryManager.Instance.RemoveItem(item);
    }

    public void ToggleInventory()
    {
        if (InventoryManager.Instance != null)
        {
            bool currentState = InventoryManager.Instance.GetAllItems().Count > 0;
            InventoryManager.Instance.ToggleInventory(!currentState);
        }
        else
        {
            Debug.LogError("InventoryManager instance not found!");
        }
    }

    // Updated scene loading with audio transition support
    public void LoadASceneItem(string scene)
    {
        // Prepare audio manager for scene change
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PrepareForSceneChange();
        }

        // Instant scene load - audio transitions happen automatically in background
        SceneManager.LoadScene(scene);
    }

    public void FadeIn(GameObject fadein)
    {
        StartCoroutine(FadeInCoroutine(fadein));
    }

    private IEnumerator FadeInCoroutine(GameObject fadein)
    {
        Image image = fadein.GetComponent<Image>();
        if (image == null) yield break;

        float duration = 1f;
        float currentTime = 0f;

        fadein.SetActive(true);
        Color color = image.color;
        color.a = 0f;
        image.color = color;

        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / duration);
            color.a = alpha;
            image.color = color;
            currentTime += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        image.color = color;
    }

    public void AddProgressToInventory(string thing)
    {
        InventoryManager.Instance.AddProgress(thing);
    }

    public void RemoveProgressToInventory(string thing)
    {
        InventoryManager.Instance.RemoveProgress(thing);
    }

    public void GrabOnions(bool thing)
    {
        if (CookingMinigame.instance != null)
            CookingMinigame.instance.GrabOnions(thing);
    }

    public void GrabPeppers(bool thing)
    {
        if (CookingMinigame.instance != null)
            CookingMinigame.instance.GrabPeppers(thing);
    }

    public void GrabGarlic(bool thing)
    {
        if (CookingMinigame.instance != null)
            CookingMinigame.instance.GrabGarlic(thing);
    }

    public void GrabBeans(bool thing)
    {
        if (CookingMinigame.instance != null)
            CookingMinigame.instance.GrabBeans(thing);
    }

    public void GrabSausages(bool thing)
    {
        if (CookingMinigame.instance != null)
            CookingMinigame.instance.GrabSausages(thing);
    }

    public void GrabBacon(bool thing)
    {
        if (CookingMinigame.instance != null)
            CookingMinigame.instance.GrabBacon(thing);
    }
}