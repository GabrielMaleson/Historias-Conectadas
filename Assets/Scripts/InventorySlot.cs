using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text tooltipText;

    private InventoryItem item;
    private bool wasUsed;

    public InventoryItem Item => item;
    public bool IsEmpty => item == null;

    public bool IsDragging = true;

    public void Setup(InventoryItem newItem)
    {
        item = newItem;
        iconImage.sprite = item.icon;
        iconImage.enabled = true;
        tooltipText.text = $"<b>{item.itemName}</b>\n{item.description}";
        tooltipPanel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
            MakeItDrag();
    }

    private void MakeItDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Pressed left-click.");
            IsDragging = true;
            gameObject.AddComponent<DraggableItem>();
        }
        else
        {
            if (!IsDragging)
                Destroy(GetComponent<DraggableItem>());
        }
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

    public void MarkAsUsed()
    {
        wasUsed = true;
    }
}