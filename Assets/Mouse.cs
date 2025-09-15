using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueMouseController : MonoBehaviour, IPointerClickHandler
{
    [Header("Dialogue References")]
    public DialogueRunner dialogueRunner;
    public StaticImageTagManager staticImageManager;

    [Header("Mouse Appearance")]
    public Image mouseImage;
    public Sprite hurryUpSprite;
    public Sprite nextLineSprite;

    [Header("Settings")]
    public bool hideInNonDialogueMode = true;

    private bool isDialogueModeActive = false;
    private bool isHurryUpState = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Ensure components are set up
        if (mouseImage == null)
            mouseImage = GetComponent<Image>();

        if (staticImageManager == null)
            staticImageManager = FindObjectOfType<StaticImageTagManager>();

        if (dialogueRunner == null)
            dialogueRunner = FindObjectOfType<DialogueRunner>();

        // Hide initially if configured to do so
        if (hideInNonDialogueMode)
        {
            SetMouseVisibility(false);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateStuff();
    }

    private void UpdateStuff()
    {
        staticImageManager = FindObjectOfType<StaticImageTagManager>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        // Re-subscribe to events after scene load
        SubscribeToStaticImageManagerEvents();
    }

    private void Start()
    {
        // Subscribe to StaticImageTagManager events instead of Yarn commands
        SubscribeToStaticImageManagerEvents();
    }

    private void SubscribeToStaticImageManagerEvents()
    {
        // Unsubscribe first to avoid duplicate subscriptions
        UnsubscribeFromStaticImageManagerEvents();

        if (staticImageManager != null)
        {
            // Listen for when darken/introdarken commands are processed
            staticImageManager.OnDialogueDarken += HandleDialogueStart;
            staticImageManager.OnDialogueSuperDarken += HandleDialogueStart;
            staticImageManager.OnDialogueBrighten += HandleDialogueEnd;
        }
    }

    private void UnsubscribeFromStaticImageManagerEvents()
    {
        if (staticImageManager != null)
        {
            staticImageManager.OnDialogueDarken -= HandleDialogueStart;
            staticImageManager.OnDialogueSuperDarken -= HandleDialogueStart;
            staticImageManager.OnDialogueBrighten -= HandleDialogueEnd;
        }
    }

    private void Update()
    {
        if (isDialogueModeActive)
        {
            Cursor.visible = false;
            // Follow the mouse position
            FollowMouse();

            // Check for any mouse click (left, right, or middle button)
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                HandleMouseClick();
            }
        }
    }

    private void FollowMouse()
    {
        if (mouseImage != null && mouseImage.canvas != null)
        {
            // Convert mouse position to canvas position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mouseImage.canvas.transform as RectTransform,
                Input.mousePosition,
                mouseImage.canvas.worldCamera,
                out Vector2 localPoint
            );

            // Set the position
            mouseImage.rectTransform.localPosition = localPoint;
        }
    }

    private void HandleDialogueStart()
    {
        ActivateDialogueMode();
    }

    private void HandleDialogueEnd()
    {
        DeactivateDialogueMode();
    }

    public void ActivateDialogueMode()
    {
        isDialogueModeActive = true;
        SetMouseVisibility(true);
        ResetToHurryUpState();
    }

    public void DeactivateDialogueMode()
    {
        isDialogueModeActive = false;
        Cursor.visible = true;
        if (hideInNonDialogueMode)
        {
            SetMouseVisibility(false);
        }
        ResetToHurryUpState();
    }

    private void SetMouseVisibility(bool visible)
    {
        if (mouseImage != null)
        {
            mouseImage.enabled = visible;

            // Disable raycast target since we're detecting clicks globally
            mouseImage.raycastTarget = false;
        }
    }

    private void ResetToHurryUpState()
    {
        isHurryUpState = true;
        if (mouseImage != null && hurryUpSprite != null)
        {
            mouseImage.sprite = hurryUpSprite;
        }
    }

    private void SwitchToNextLineState()
    {
        isHurryUpState = false;
        if (mouseImage != null && nextLineSprite != null)
        {
            mouseImage.sprite = nextLineSprite;
        }
    }

    // Handle mouse clicks anywhere on the screen
    private void HandleMouseClick()
    {
        if (!isDialogueModeActive || dialogueRunner == null)
            return;

        if (isHurryUpState)
        {
            // Request to hurry up the current line
            dialogueRunner.RequestHurryUpLine();
            SwitchToNextLineState();
        }
        else
        {
            // Request to move to the next line
            dialogueRunner.RequestNextLine();
            ResetToHurryUpState();
        }
    }

    // This method can be removed or kept empty since we're using global click detection
    public void OnPointerClick(PointerEventData eventData)
    {
        // Empty implementation since we're using global click detection
        // This method is only kept to maintain the IPointerClickHandler interface
    }

    private void OnDestroy()
    {
        // Clean up event handlers
        UnsubscribeFromStaticImageManagerEvents();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Public methods for manual control if needed
    public void ForceActivateDialogueMode()
    {
        ActivateDialogueMode();
    }

    public void ForceDeactivateDialogueMode()
    {
        DeactivateDialogueMode();
    }
}