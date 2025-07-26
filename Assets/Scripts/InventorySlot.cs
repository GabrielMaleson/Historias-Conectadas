using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text tooltipText;

    [Header("Drag Settings")]
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private GameObject dragVisualPrefab;

    private InventoryItem item;
    private GameObject currentDragVisual;
    private RectTransform dragTransform;
    private bool isDragging;

    public InventoryItem Item => item;
    public bool IsEmpty => item == null;

    private void Awake()
    {
        if (parentCanvas == null)
        {
            parentCanvas = GetComponentInParent<Canvas>();
        }
    }

    public void Setup(InventoryItem newItem)
    {
        item = newItem;
        iconImage.sprite = item?.icon;
        iconImage.enabled = item != null;

        if (item != null)
        {
            tooltipText.text = $"<b>{item.itemName}</b>\n{item.description}";
        }
    }

    public void ClearSlot()
    {
        item = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
        tooltipPanel.SetActive(false);
    }

    #region Pointer Events
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsEmpty)
            tooltipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);
    }
    #endregion

    #region Drag and Drop
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsEmpty || isDragging) return;

        isDragging = true;

        // Create drag visual
        if (dragVisualPrefab != null && parentCanvas != null)
        {
            currentDragVisual = Instantiate(dragVisualPrefab, parentCanvas.transform);
            currentDragVisual.GetComponent<Image>().sprite = iconImage.sprite;
            dragTransform = currentDragVisual.GetComponent<RectTransform>();
            dragTransform.SetAsLastSibling();
        }

        // Hide tooltip during drag
        tooltipPanel.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || currentDragVisual == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(),
            Input.mousePosition,
            parentCanvas.worldCamera,
            out Vector2 localPoint);

        dragTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Clean up drag visual
        if (currentDragVisual != null)
        {
            Destroy(currentDragVisual);
            currentDragVisual = null;
        }

        isDragging = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag.TryGetComponent<InventorySlot>(out var draggedSlot) || draggedSlot.IsEmpty)
            return;

        // If this slot is empty, take the item
        if (IsEmpty)
        {
            Setup(draggedSlot.Item);
            draggedSlot.ClearSlot();
        }
        else // Otherwise swap items
        {
            var tempItem = Item;
            Setup(draggedSlot.Item);
            draggedSlot.Setup(tempItem);
        }
    }
    #endregion
}