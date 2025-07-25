using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private DraggedItem draggedItemPrefab;

    public InventoryItem item;
    private DraggedItem currentDraggedItem;
    private Vector2 originalPosition;
    private bool wasUsed;
    private Vector2 dragOffset;
    private Canvas topmostCanvas;

    public InventoryItem Item => item;
    public bool IsEmpty => item == null;

    public void Setup(InventoryItem newItem)
    {
        item = newItem;
        iconImage.sprite = item.icon;
        iconImage.enabled = true;
        tooltipText.text = $"<b>{item.itemName}</b>\n{item.description}";
        tooltipPanel.SetActive(false);
    }

    public void ClearSlot()
    {
        item = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
        tooltipPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            tooltipPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;

        tooltipPanel.SetActive(false);
        originalPosition = iconImage.rectTransform.anchoredPosition;

        // Create dragged item from prefab
        currentDraggedItem = Instantiate(draggedItemPrefab, topmostCanvas.transform);
        currentDraggedItem.Setup(item);
        currentDraggedItem.RectTransform.SetAsLastSibling();

        // Match the size of the original icon
        currentDraggedItem.RectTransform.sizeDelta = iconImage.rectTransform.rect.size;

        // Set initial position to match mouse position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            topmostCanvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera ?? Camera.main,
            out Vector2 localPoint);

        currentDraggedItem.RectTransform.localPosition = localPoint;

        // Calculate offset based on where we clicked on the original icon
        Vector2 iconScreenPoint = RectTransformUtility.WorldToScreenPoint(
            eventData.pressEventCamera ?? Camera.main,
            iconImage.rectTransform.position);

        dragOffset = (Vector2)currentDraggedItem.RectTransform.position - eventData.position;

        // Hide original icon
        iconImage.enabled = false;
        wasUsed = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentDraggedItem == null) return;

        // Convert screen position to local position within canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            topmostCanvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera ?? Camera.main,
            out Vector2 localPoint);

        // Apply the offset to maintain cursor position relative to the dragged object
        currentDraggedItem.RectTransform.localPosition = localPoint + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentDraggedItem == null) return;

        // Destroy the dragged item
        Destroy(currentDraggedItem.gameObject);

        if (!wasUsed)
        {
            // Show original icon again if item wasn't used
            iconImage.enabled = true;
        }
        else
        {
            // Clear slot if item was used (dropped somewhere valid)
            ClearSlot();
        }
    }
}