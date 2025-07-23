using UnityEngine;

public class HorizontalBackgroundScroller : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The speed at which the background scrolls")]
    public float scrollSpeed = 0.5f;

    [Tooltip("How much to multiply the mouse movement (sensitivity)")]
    public float mouseSensitivity = 1f;

    [Tooltip("The minimum X position for the background")]
    public float minXPosition = -10f;

    [Tooltip("The maximum X position for the background")]
    public float maxXPosition = 10f;

    private Vector3 initialPosition;
    private float lastMouseX;
    private Renderer backgroundRenderer;

    void Start()
    {
        // Store the initial position
        initialPosition = transform.position;

        // Get the renderer component
        backgroundRenderer = GetComponent<Renderer>();

        if (backgroundRenderer == null)
        {
            Debug.LogError("No Renderer component found on this GameObject. This script requires a Renderer.");
        }

        // Initialize last mouse position
        lastMouseX = Input.mousePosition.x;
    }

    void Update()
    {
        if (backgroundRenderer == null) return;

        // Get current mouse position
        float currentMouseX = Input.mousePosition.x;

        // Calculate mouse movement delta
        float mouseDeltaX = (currentMouseX - lastMouseX) * mouseSensitivity;

        // Update the last mouse position
        lastMouseX = currentMouseX;

        // Calculate the new offset
        float newXOffset = transform.position.x + mouseDeltaX * scrollSpeed * Time.deltaTime;

        // Clamp the position between min and max values
        newXOffset = Mathf.Clamp(newXOffset, minXPosition, maxXPosition);

        // Apply the new position
        transform.position = new Vector3(newXOffset, transform.position.y, transform.position.z);

        // Alternative approach using material offset (uncomment if you prefer this method)
        /*
        float offsetX = backgroundRenderer.material.mainTextureOffset.x + mouseDeltaX * scrollSpeed * Time.deltaTime;
        backgroundRenderer.material.mainTextureOffset = new Vector2(offsetX, 0);
        */
    }

    // Reset to initial position (optional)
    public void ResetPosition()
    {
        transform.position = initialPosition;
    }
}