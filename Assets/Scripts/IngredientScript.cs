using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngredientScript : MonoBehaviour
{
    [SerializeField] private CookingMinigame cook;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        InventorySlot item = collision.GetComponent<InventorySlot>();

        if (item != null && item.slotTags.Contains("Sausage"))
        {
            if (!cook.GotSausages)
            {
                cook.SetSausages(true);
                Debug.Log("Got the sausages!");
            }
        }
        if (item != null && item.slotTags.Contains("Pepper"))
        {
            if (!cook.GotPeppers)
            {
                cook.SetPeppers(true);
                Debug.Log("Got the peppers!");
            }
        }
        if (item != null && item.slotTags.Contains("Garlic"))
        {
            if (!cook.GotSausages)
            {
                cook.SetGarlic(true);
                Debug.Log("Got the Garlic!");
            }
        }
        if (item != null && item.slotTags.Contains("Bacon"))
        {
            if (!cook.GotBacon)
            {
                cook.SetBacon(true);
                Debug.Log("Got the bacon!");
            }
        }
        if (item != null && item.slotTags.Contains("Onion"))
        {
            if (!cook.GotOnions)
            {
                cook.SetOnions(true);
                Debug.Log("Got the onion!");
            }
        }
    }
}
