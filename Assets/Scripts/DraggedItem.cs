using UnityEngine;
using UnityEngine.UI;

public class DraggedItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private RectTransform rectTransform;
    public RectTransform RectTransform => rectTransform ??= GetComponent<RectTransform>();

    public void Setup(InventoryItem item)
    {
        iconImage.sprite = item.icon;
        iconImage.raycastTarget = false; // Important to prevent raycast blocking
    }
}