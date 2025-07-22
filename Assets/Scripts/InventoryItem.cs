using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;
    public List<string> itemTags = new List<string>();
}