using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Security.Cryptography;

public class DraggableItem : MonoBehaviour, IDropHandler
{
    
    InventorySlot slot;

    Transform parentAfterDrag;

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            InventorySlot draggableItem = dropped.GetComponent<InventorySlot>();
            draggableItem.parentAfterDrag = transform;
        }
        else
        {
            GameObject dropped = eventData.pointerDrag;
            InventorySlot draggableItem = dropped.GetComponent<InventorySlot>();

            GameObject current = transform.GetChild(0).gameObject;
            InventorySlot currentDraggable = current.GetComponent<InventorySlot>();

            currentDraggable.transform.SetParent(draggableItem.parentAfterDrag);
            draggableItem.parentAfterDrag = transform;
        }
    }
}