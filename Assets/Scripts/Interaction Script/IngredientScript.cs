using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

public class IngredientScript : MonoBehaviour
{
    [SerializeField] private CookingMinigame cook;
    public DialogueRunner dialogueRunner;
    public InventoryItem sausage;
    public InventoryItem garlic;
    public InventoryItem pepper;
    public InventoryItem onion;
    public InventoryItem bacon;
    public InventoryItem beans; // New beans ingredient
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
            if (item != null && item.slotTags.Contains("Bacon"))
            {
                if (!cook.GotBacon)
                {
                    cook.SetBacon(true);
                    Debug.Log("Got the bacon!");
                    DoThing(bacon);
                    dialogueRunner.StartDialogue("lucibacon");
                }
            }
            else if (item != null && item.slotTags.Contains("Pepper"))
            {
                if (cook.GotBacon && !cook.GotPeppers)
                {
                    cook.SetPeppers(true);
                    Debug.Log("Got the peppers!");
                    DoThing(pepper);
                    dialogueRunner.StartDialogue("lucipepper");
                }
                else if (!cook.GotBacon)
                {
                    Debug.Log("Need bacon first!");
                    dialogueRunner.StartDialogue("luciwrong");
                }
            }
            else if (item != null && item.slotTags.Contains("Sausage"))
            {
                if (cook.GotBacon && cook.GotPeppers && !cook.GotSausages)
                {
                    cook.SetSausages(true);
                    Debug.Log("Got the sausages!");
                    DoThing(sausage);
                    dialogueRunner.StartDialogue("lucisausage");
                }
                else if (!cook.GotBacon || !cook.GotPeppers)
                {
                    Debug.Log("Need bacon and peppers first!");
                    dialogueRunner.StartDialogue("luciwrong");
                }
            }
            else if (item != null && item.slotTags.Contains("Onion"))
            {
                if (cook.GotBacon && cook.GotPeppers && cook.GotSausages && !cook.GotOnions)
                {
                    cook.SetOnions(true);
                    Debug.Log("Got the onion!");
                    DoThing(onion);
                    dialogueRunner.StartDialogue("wincooking");
                }
                else if (!cook.GotBacon || !cook.GotPeppers || !cook.GotSausages)
                {
                    Debug.Log("Need bacon, peppers, and sausages first!");
                    dialogueRunner.StartDialogue("luciwrong");
                }
            }
            else if (item != null && (item.slotTags.Contains("Garlic") || item.slotTags.Contains("Beans")))
            {
                // Wrong ingredients - garlic and beans
                Debug.Log("Wrong ingredient!");
                if (item.slotTags.Contains("Garlic"))
                {
                    DoThing(garlic);
                }
                else if (item.slotTags.Contains("Beans"))
                {
                    DoThing(beans);
                }
                dialogueRunner.StartDialogue("luciwrong");
            }
        }
    }

    private void DoThing(InventoryItem item)
    {
        InventoryManager.Instance.RemoveItem(item);
    }
}