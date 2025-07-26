using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private Image dragImage; // New reference for drag visuals

    private InventoryItem item;
    private Transform parentAfterDrag;
    private bool isDragging;
    private GameObject dragClone;

    public InventoryItem Item => item;
    public bool IsEmpty => item == null;

    private void Awake()
    {
        // Create a hidden drag image copy
        dragImage = Instantiate(iconImage, transform.root);
        dragImage.gameObject.SetActive(false);
        dragImage.raycastTarget = false;
    }

    public void Setup(InventoryItem newItem)
    {
        item = newItem;
        iconImage.sprite = item.icon;
        iconImage.enabled = true;
        tooltipText.text = $"<b>{item.itemName}</b>\n{item.description}";
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
        if (IsEmpty) return;

        isDragging = true;

        // Set up drag visual
        dragImage.sprite = iconImage.sprite;
        dragImage.transform.SetParent(transform.root);
        dragImage.transform.SetAsLastSibling();
        dragImage.rectTransform.sizeDelta = iconImage.rectTransform.sizeDelta;
        dragImage.gameObject.SetActive(true);

        // Hide original icon during drag
        iconImage.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
            dragImage.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Clean up drag visual
        dragImage.gameObject.SetActive(false);

        // Restore original icon if still in slot
        if (!IsEmpty)
            iconImage.enabled = true;

        isDragging = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag.TryGetComponent<InventorySlot>(out var draggedSlot))
            return;

        if (IsEmpty)
        {
            // Move item to this empty slot
            Setup(draggedSlot.Item);
            draggedSlot.ClearSlot();
        }
        else
        {
            // Swap items
            var tempItem = Item;
            Setup(draggedSlot.Item);
            draggedSlot.Setup(tempItem);
        }
    }
    #endregion
}