using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Security.Cryptography;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Image iconImage;

    private RectTransform rectTransform;
    
    InventorySlot slot;

    Transform parentAfterDrag;
    public RectTransform RectTransform => rectTransform ??= GetComponent<RectTransform>();

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin drag");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("draggin");
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("draggin over");
        Destroy(this);
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentAfterDrag = transform;
        }
        else
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

            GameObject current = transform.GetChild(0).gameObject;
            DraggableItem currentDraggable = current.GetComponent<DraggableItem>();

            currentDraggable.transform.SetParent(draggableItem.parentAfterDrag);
            draggableItem.parentAfterDrag = transform;
        }
    }
    public void Setup(InventoryItem item)
    {
        iconImage.sprite = item.icon;
        iconImage.raycastTarget = false; // Important to prevent raycast blocking
    }
}