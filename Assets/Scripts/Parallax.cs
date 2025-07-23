using UnityEngine;
using UnityEngine.UI; // Required for UI components
public class UIParallax : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField, Range(0f, 1f)]
    private float horizontalEffect = 0.5f;

    [SerializeField, Range(0f, 1f)]
    private float verticalEffect = 0f; // Set to 0 for purely horizontal

    [SerializeField]
    private RectTransform canvasRect; // Drag parent Canvas here

    [SerializeField]
    private Transform targetCamera; // Drag main camera here

    private Vector2 startPosition;
    private Vector2 imageSize;

    private void Awake()
    {
        // Auto-assign camera if null
        if (targetCamera == null)
        {
            targetCamera = Camera.main?.transform;
            if (targetCamera == null)
                Debug.LogError("No camera assigned!", this);
        }

        // Get reference to RectTransform
        RectTransform rt = GetComponent<RectTransform>();
        startPosition = rt.anchoredPosition;
        imageSize = rt.sizeDelta;
    }

    private void LateUpdate()
    {
        if (targetCamera == null || canvasRect == null) return;

        // Calculate normalized camera position (0-1 range)
        Vector2 viewportPos = targetCamera.GetComponent<Camera>().WorldToViewportPoint(targetCamera.position);

        // Adjust for canvas scaling
        Vector2 canvasSize = canvasRect.sizeDelta;
        Vector2 parallaxOffset = new Vector2(
            (viewportPos.x - 0.5f) * canvasSize.x * horizontalEffect,
            (viewportPos.y - 0.5f) * canvasSize.y * verticalEffect
        );

        // Apply movement
        GetComponent<RectTransform>().anchoredPosition = startPosition + parallaxOffset;

        // Optional: Infinite scrolling logic
        if (Mathf.Abs(parallaxOffset.x) > imageSize.x)
        {
            float offsetDirection = Mathf.Sign(parallaxOffset.x);
            startPosition.x += offsetDirection * imageSize.x;
        }
    }
}