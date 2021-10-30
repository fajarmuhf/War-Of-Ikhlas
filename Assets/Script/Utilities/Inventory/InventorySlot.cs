using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNumberText;
    [SerializeField] private Image itemImage;

    [Header("Variable item")] 
    public InventoryItem thisItem;
    public InventoryManager thisManager;

    public void Setup(InventoryItem newItem, InventoryManager newManager)
    {
        thisItem = newItem;
        thisManager = newManager;
        if (thisItem)
        {
            Sprite[] s = Resources.LoadAll<Sprite>(thisItem.itemImage);
            itemImage.sprite = s[thisItem.itemImageIndex];
            itemNumberText.text = "" + thisItem.numberHeld;
        }
    }

    public void ClickOn()
    {
        if (thisItem)
        {
            thisManager.SetupDescriptionAndButton(thisItem.itemDescription,thisItem.usable,thisItem);
        }
    }
}
