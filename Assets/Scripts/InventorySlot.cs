using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    private static GameObject tooltipPanel;
    private static TMP_Text tooltipText;

    [Header("Drag Settings")]
    [SerializeField] private Canvas parentCanvas;

    public Transform parentAfterDrag;
    private InventoryItem item;
    private RectTransform rectTransform;
    private GameObject inventory;
    private bool isDragging;
    public List<string> slotTags = new List<string>();
    private Vector3 originalPosition;
    private Transform originalParent;

    public InventoryItem Item => item;
    public bool IsEmpty => item == null;

    private void Awake()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory");
        if (parentCanvas == null)
        {
            parentCanvas = GetComponentInParent<Canvas>();
        }

        rectTransform = GetComponent<RectTransform>();

        // Initialize static tooltip references if not set
        if (tooltipPanel == null)
        {
            tooltipPanel = GameObject.FindGameObjectWithTag("InventoryTooltip");
            if (tooltipPanel != null)
            {
                tooltipText = tooltipPanel.GetComponentInChildren<TMP_Text>();
                tooltipPanel.SetActive(false);
            }
        }
    }

    public void Setup(InventoryItem newItem)
    {
        item = newItem;
        iconImage.sprite = item?.icon;
        iconImage.enabled = item != null;

        if (item != null)
        {
            // Clear existing tags and copy all item tags to slot tags
            slotTags.Clear();
            slotTags.AddRange(newItem.itemTags);
        }
        else
        {
            slotTags.Clear();
        }
    }

    public void ClearSlot()
    {
        item = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
        slotTags.Clear();
    }

    #region Pointer Events
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsEmpty && tooltipPanel != null)
        {
            tooltipText.text = $"<b>{item.itemName}</b>\n{item.description}";
            tooltipPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
    #endregion

    #region Drag and Drop
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsEmpty || isDragging) return;

        if (inventory != null)
        {
            inventory.SetActive(false);
        }
        isDragging = true;
        originalPosition = rectTransform.localPosition;
        originalParent = transform.parent;

        // Set as last sibling to appear on top of other UI elements
        transform.SetParent(parentCanvas.transform);
        transform.SetAsLastSibling();

        // Hide tooltip during drag
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(),
            eventData.position,
            parentCanvas.worldCamera,
            out Vector2 localPoint);

        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;

        // If we didn't drop on a valid slot, return to original position
        if (parentAfterDrag == null)
        {
            transform.SetParent(originalParent);
            rectTransform.localPosition = originalPosition;
        }
        else
        {
            transform.SetParent(parentAfterDrag);
            rectTransform.localPosition = Vector3.zero;
            parentAfterDrag = null;
        }
    }
    #endregion
}