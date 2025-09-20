using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ActivateChildrenOnFullAlpha : MonoBehaviour
{
    private Image image;
    private bool previousAlphaState = false;

    void Start()
    {
        image = GetComponent<Image>();
        UpdateChildrenActivation();
    }

    void Update()
    {
        // Check if alpha state has changed
        bool currentAlphaState = image.color.a >= 0.999f; // Using 0.999f to account for floating point precision

        if (currentAlphaState != previousAlphaState)
        {
            UpdateChildrenActivation();
            previousAlphaState = currentAlphaState;
        }
    }

    private void UpdateChildrenActivation()
    {
        bool shouldActivate = image.color.a >= 0.999f;

        // Activate/deactivate all direct children
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(shouldActivate);
        }
    }

    // Optional: Public method to manually check and update
    public void CheckAlphaAndUpdateChildren()
    {
        UpdateChildrenActivation();
    }
}