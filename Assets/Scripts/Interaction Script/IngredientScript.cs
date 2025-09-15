using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public InventoryItem beans;
    public Button button;

    private void Start()
    {
        // Check if cooking minigame exists and restore button state
        var cookingMinigame = FindObjectOfType<CookingMinigame>();
        if (cookingMinigame != null && cookingMinigame.isMinigameActive)
        {
            button.enabled = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        InventorySlot item = collision.GetComponent<InventorySlot>();
        var cookingMinigame = FindObjectOfType<CookingMinigame>();

        if (cookingMinigame != null && cookingMinigame.isMinigameActive)
        {
            if (item != null && item.slotTags.Contains("Bacon"))
            {
                if (!cookingMinigame.GotBacon)
                {
                    cookingMinigame.SetBacon(true);
                    Debug.Log("Got the bacon!");
                    DoThing(bacon);
                    dialogueRunner.StartDialogue("lucibacon");
                    // Bacon button will be deactivated automatically through the state system
                }
            }
            else if (item != null && item.slotTags.Contains("Pepper"))
            {
                if (cookingMinigame.GotBacon && !cookingMinigame.GotPeppers)
                {
                    cookingMinigame.SetPeppers(true);
                    Debug.Log("Got the peppers!");
                    DoThing(pepper);
                    dialogueRunner.StartDialogue("lucipepper");
                }
                else if (!cookingMinigame.GotBacon)
                {
                    Debug.Log("Need bacon first!");
                    dialogueRunner.StartDialogue("luciwrong");
                }
            }
            else if (item != null && item.slotTags.Contains("Sausage"))
            {
                if (cookingMinigame.GotBacon && cookingMinigame.GotPeppers && !cookingMinigame.GotSausages)
                {
                    cookingMinigame.SetSausages(true);
                    Debug.Log("Got the sausages!");
                    DoThing(sausage);
                    dialogueRunner.StartDialogue("lucisausage");
                }
                else if (!cookingMinigame.GotBacon || !cookingMinigame.GotPeppers)
                {
                    Debug.Log("Need bacon and peppers first!");
                    dialogueRunner.StartDialogue("luciwrong");
                }
            }
            else if (item != null && item.slotTags.Contains("Onion"))
            {
                if (cookingMinigame.GotBacon && cookingMinigame.GotPeppers && cookingMinigame.GotSausages && !cookingMinigame.GotOnions)
                {
                    cookingMinigame.SetOnions(true);
                    Debug.Log("Got the onion!");
                    DoThing(onion);
                    dialogueRunner.StartDialogue("wincooking");
                }
                else if (!cookingMinigame.GotBacon || !cookingMinigame.GotPeppers || !cookingMinigame.GotSausages)
                {
                    Debug.Log("Need bacon, peppers, and sausages first!");
                    dialogueRunner.StartDialogue("luciwrong");
                }
            }
            else if (item != null && item.slotTags.Contains("Garlic"))
            {
                // Wrong ingredient - garlic
                cookingMinigame.SetGarlic(true);
                Debug.Log("Wrong ingredient - garlic!");
                DoThing(garlic);
                dialogueRunner.StartDialogue("luciwrong");
            }
            else if (item != null && item.slotTags.Contains("Beans"))
            {
                // Wrong ingredient - beans
                cookingMinigame.SetBeans(true);
                Debug.Log("Wrong ingredient - beans!");
                DoThing(beans);
                dialogueRunner.StartDialogue("luciwrong");
            }
        }
    }

    private void DoThing(InventoryItem item)
    {
        InventoryManager.Instance.RemoveItem(item);
    }
}