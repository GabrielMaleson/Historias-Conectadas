using UnityEngine;
using UnityEngine.UI;

public class ButtonItemPickup : MonoBehaviour
{
    public InventoryItem item;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PickupItem);
        }
    }

    private void PickupItem()
    {
        if (item != null)
        {
            InventoryManager.Instance.AddItem(item);
        }
    }
}