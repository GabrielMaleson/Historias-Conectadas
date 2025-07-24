using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private GameObject dragColliderPrefab; // Prefab with collider for the dragged item

    private InventoryItem item;
    private RectTransform dragObject;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private bool wasUsed;
    private Transform originalParent;
    private GameObject dragCollider; // The collider object
    private Vector3 dragOffset; // Offset to make cursor appear at the drag point

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
        originalParent = transform;

        // Create drag object
        dragObject = Instantiate(iconImage.gameObject, GetTopmostCanvas().transform).GetComponent<RectTransform>();
        dragObject.name = "Dragged Item";
        dragObject.SetAsLastSibling();

        // Calculate offset to make cursor appear at the drag point
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            dragObject.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera ?? Camera.main,
            out Vector3 worldPoint);
        dragOffset = dragObject.position - worldPoint;

        // Set up drag image
        Image dragImage = dragObject.GetComponent<Image>();
        dragImage.sprite = iconImage.sprite;
        dragImage.raycastTarget = false;

        canvasGroup = dragObject.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;

        // Create collision object
        if (dragColliderPrefab != null)
        {
            dragCollider = Instantiate(dragColliderPrefab, worldPoint, Quaternion.identity);
            dragCollider.GetComponent<Collider2D>().isTrigger = true;
            dragCollider.tag = "DraggedItem";

            // Match the sprite if needed
            SpriteRenderer sr = dragCollider.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = iconImage.sprite;
        }

        iconImage.enabled = false;
        wasUsed = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragObject == null) return;

        // Update UI drag object position
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            dragObject.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera ?? Camera.main,
            out Vector3 worldPoint);

        dragObject.position = worldPoint + dragOffset;

        // Update collider position if it exists
        if (dragCollider != null)
        {
            dragCollider.transform.position = worldPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragObject == null) return;

        Destroy(dragObject.gameObject);

        // Handle collider
        if (dragCollider != null)
        {
            // Check if we dropped on a valid target
            Collider2D[] hits = Physics2D.OverlapPointAll(dragCollider.transform.position);
            bool usedOnTarget = false;

            foreach (var hit in hits)
            {
                if (hit != null && hit.gameObject != dragCollider)
                {
                    // Check if the hit object can receive this item
                    var receiver = hit.GetComponent<IItemReceiver>();
                    if (receiver != null && receiver.CanReceiveItem(item))
                    {
                        receiver.ReceiveItem(item);
                        usedOnTarget = true;
                        break;
                    }
                }
            }

            Destroy(dragCollider);

            if (usedOnTarget)
            {
                wasUsed = true;
            }
        }

        if (!wasUsed)
        {
            iconImage.enabled = true;
        }
        else
        {
            ClearSlot();
        }
    }

    private Canvas GetTopmostCanvas()
    {
        Canvas[] parentCanvases = GetComponentsInParent<Canvas>();
        return parentCanvases[parentCanvases.Length - 1];
    }
}

// Interface for objects that can receive items
public interface IItemReceiver
{
    bool CanReceiveItem(InventoryItem item);
    void ReceiveItem(InventoryItem item);
}