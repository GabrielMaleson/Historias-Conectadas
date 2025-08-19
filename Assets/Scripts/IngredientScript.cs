using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngredientScript : MonoBehaviour
{
    [SerializeField] private CookingMinigame cook;
    public InventoryItem sausage;
    public InventoryItem garlic;
    public InventoryItem pepper;
    public InventoryItem onion;
    public InventoryItem bacon;
    public Button button;

    public void Update()
    {
        if (cook.isMinigameActive)
        {
            button.enabled = false;
        }

    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        InventorySlot item = collision.GetComponent<InventorySlot>();
        if (cook.isMinigameActive) 
         {
            if (item != null && item.slotTags.Contains("Sausage"))
            {
                if (!cook.GotSausages)
                {
                    cook.SetSausages(true);
                    Debug.Log("Got the sausages!");
                    DoThing(sausage);
                }
            }
            if (item != null && item.slotTags.Contains("Pepper"))
            {
                if (!cook.GotPeppers)
                {
                    cook.SetPeppers(true);
                    Debug.Log("Got the peppers!");
                    DoThing(pepper);   
                }
            }
            if (item != null && item.slotTags.Contains("Garlic"))
            {
                if (!cook.GotSausages)
                {
                    cook.SetGarlic(true);
                    Debug.Log("Got the Garlic!");
                    DoThing(garlic);
                }
            }
            if (item != null && item.slotTags.Contains("Bacon"))
            {
                if (!cook.GotBacon)
                {
                    cook.SetBacon(true);
                    Debug.Log("Got the bacon!");
                    DoThing(bacon);
                }
            }
            if (item != null && item.slotTags.Contains("Onion"))
            {
                if (!cook.GotOnions)
                {
                    cook.SetOnions(true);
                    Debug.Log("Got the onion!");
                    DoThing(onion);
                }
            }
        }
    }
    private void DoThing(InventoryItem item)
    {
        InventoryManager.Instance.RemoveItem(item);
    }
}
