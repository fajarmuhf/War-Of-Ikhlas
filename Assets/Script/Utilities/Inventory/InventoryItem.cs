using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName="New Item",menuName = "Inventory/Items")]
public class InventoryItem : ScriptableObject
{
    public int itemId;
    public string itemName;
    public string itemDescription;
    public string itemImage;
    public int itemImageIndex;
    public int numberHeld;
    public bool usable;
    public bool unique;
    public UnityEvent thisEvent;

    public void Use()
    {
        Debug.Log("Using Item");
        thisEvent.Invoke();
    }
}
