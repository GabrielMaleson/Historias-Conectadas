using UnityEngine;
using UnityEngine.UI;

public class HorizontalBackgroundScroller : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The speed at which the background scrolls")]
    public float scrollSpeed = 0.5f;

    [Tooltip("How much to multiply the mouse movement (sensitivity)")]
    public float mouseSensitivity = 1f;

    [Tooltip("The minimum X position for the background")]
    public float minXPosition = -1000f;

    [Tooltip("The maximum X position for the background")]
    public float maxXPosition = 1000f;

    private Vector2 initialPosition;
    private float lastMouseX;
    private RectTransform rectTransform;
    private RawImage backgroundImage;

    void Start()
    {
        // Get the RectTransform and Image components
        rectTransform = GetComponent<RectTransform>();
        backgroundImage = GetComponent<RawImage>();

        if (rectTransform == null || backgroundImage == null)
        {
            Debug.LogError("This script requires both RectTransform and Image components on a UI element.");
            return;
        }

        // Store the initial position
        initialPosition = rectTransform.anchoredPosition;

        // Initialize last mouse position
        lastMouseX = Input.mousePosition.x;
    }

    void Update()
    {
        if (rectTransform == null || backgroundImage == null) return;

        // Get current mouse position
        float currentMouseX = Input.mousePosition.x;

        // Calculate mouse movement delta
        float mouseDeltaX = (currentMouseX - lastMouseX) * mouseSensitivity;

        // Update the last mouse position
        lastMouseX = currentMouseX;

        // Calculate the new X position
        float newXPos = rectTransform.anchoredPosition.x + mouseDeltaX * scrollSpeed * Time.deltaTime;

        // Clamp the position between min and max values
        newXPos = Mathf.Clamp(newXPos, minXPosition, maxXPosition);

        // Apply the new position
        rectTransform.anchoredPosition = new Vector2(newXPos, rectTransform.anchoredPosition.y);

        Rect uvRect = backgroundImage.uvRect;
        uvRect.x += mouseDeltaX * scrollSpeed * Time.deltaTime * 0.01f;
        backgroundImage.uvRect = uvRect;
    }

    // Reset to initial position (optional)
    public void ResetPosition()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = initialPosition;
        }
    }
}