using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CreditsScroller : MonoBehaviour
{
    [Header("TextMeshPro Settings")]
    [SerializeField] private TextMeshProUGUI creditsText;

    [Header("Scrolling Settings")]
    [SerializeField] private float scrollSpeed = 30f;
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float endDelay = 5f;

    [Header("Credits Content")]
    [TextArea(5, 10)]
    [SerializeField]
    private string creditsContent =
        "Game Director\nJohn Smith\n\n" +
        "Lead Programmer\nJane Doe\n\n" +
        "Art Director\nBob Johnson\n\n" +
        "Sound Designer\nAlice Williams";

    private RectTransform textRectTransform;
    private Vector2 initialPosition;
    public InventoryButtonHandler itemhandler;
    private bool isScrolling = false;
    private float timer = 0f;

    void Start()
    {
        if (creditsText == null)
        {
            Debug.LogError("Credits TextMeshPro component is not assigned!");
            return;
        }

        textRectTransform = creditsText.GetComponent<RectTransform>();
        initialPosition = textRectTransform.anchoredPosition;

        // Set the credits text
        creditsText.text = creditsContent;

        // Start scrolling after delay
        Invoke("StartScrolling", startDelay);
    }

    void Update()
    {
        if (isScrolling)
        {
            // Move text upward
            textRectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            // Check if text has scrolled past the screen
            CheckIfFinished();
        }
    }

    private void StartScrolling()
    {
        isScrolling = true;
        // Reset position to bottom of screen
        textRectTransform.anchoredPosition = initialPosition;
    }

    private void CheckIfFinished()
    {
        // Get the height of the text content
        float textHeight = creditsText.preferredHeight;

        // Check if the top of the text has scrolled past the top of the viewport
        if (textRectTransform.anchoredPosition.y > textHeight + Screen.height / 2)
        {
            isScrolling = false;
            Invoke("ResetCredits", endDelay);
        }
    }

    private void ResetCredits()
    {
        // Reset position and prepare for potential restart
        textRectTransform.anchoredPosition = initialPosition;
        // Optionally: trigger an event or load next scene here
        itemhandler.LoadASceneItem("Title Screen");
    }

    // Public method to set credits content programmatically
    public void SetCreditsContent(string newContent)
    {
        creditsContent = newContent;
        creditsText.text = creditsContent;

        // Reset scrolling
        textRectTransform.anchoredPosition = initialPosition;
        isScrolling = false;
        CancelInvoke();
        Invoke("StartScrolling", startDelay);
    }

    // Public method to start scrolling manually
    public void StartScrollingManually()
    {
        textRectTransform.anchoredPosition = initialPosition;
        isScrolling = false;
        CancelInvoke();
        Invoke("StartScrolling", startDelay);
    }

    // Public method to stop scrolling
    public void StopScrolling()
    {
        isScrolling = false;
        CancelInvoke();
    }
}