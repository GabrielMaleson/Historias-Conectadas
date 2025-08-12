using UnityEngine;
using UnityEngine.UI;

public class DialogueEffects : MonoBehaviour
{
    private Color[] originalColors;
    private bool[] originalInteractableStates;
    private Image[] backgroundImages;
    private Selectable[] interactableElements;

    // Call this to darken the background and disable interactions
    private void DialogueDarken()
    {
        // Get the Background Canvas (make sure this is the exact name in your scene)
        GameObject backgroundCanvas = GameObject.Find("Background Canvas");

        if (backgroundCanvas != null)
        {
            // Get all Image components in children
            backgroundImages = backgroundCanvas.GetComponentsInChildren<Image>(true);
            originalColors = new Color[backgroundImages.Length];

            // Store original colors and set new darkened colors
            for (int i = 0; i < backgroundImages.Length; i++)
            {
                originalColors[i] = backgroundImages[i].color;
                backgroundImages[i].color = new Color(150f / 255f, 150f / 255f, 150f / 255f, backgroundImages[i].color.a);
            }

            // Get all interactable elements and disable them
            interactableElements = backgroundCanvas.GetComponentsInChildren<Selectable>(true);
            originalInteractableStates = new bool[interactableElements.Length];

            for (int i = 0; i < interactableElements.Length; i++)
            {
                originalInteractableStates[i] = interactableElements[i].interactable;
                interactableElements[i].interactable = false;
            }
        }
        else
        {
            Debug.LogWarning("Background Canvas not found in scene!");
        }
    }

    // Call this to restore the background and enable interactions
    private void DialogueBrighten()
    {
        // Restore image colors
        if (backgroundImages != null && originalColors != null)
        {
            for (int i = 0; i < backgroundImages.Length; i++)
            {
                if (backgroundImages[i] != null) // Check if the reference is still valid
                {
                    backgroundImages[i].color = originalColors[i];
                }
            }
        }

        // Restore interactable states
        if (interactableElements != null && originalInteractableStates != null)
        {
            for (int i = 0; i < interactableElements.Length; i++)
            {
                if (interactableElements[i] != null) // Check if the reference is still valid
                {
                    interactableElements[i].interactable = originalInteractableStates[i];
                }
            }
        }
    }
}