using System;
using TMPro;
using UnityEngine;
public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Information")] 
    public PlayerInventory playerInventory;
    [SerializeField] private GameObject blankInventorySlot;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject useButton;
    public InventoryItem currentItem;
    
    public void SetTextAndButton(string description,bool buttonActive)
    {
        descriptionText.text = description;
        if (buttonActive)
        {
            useButton.SetActive(true);
        }
        else
        {
            useButton.SetActive(false);
        }
    }

    void MakeInventorySlot()
    {
        playerInventory = Player.localPlayer.playerInventory;
        if (playerInventory)
        {
            for (int i=0;i<playerInventory.myInventory.Count;i++)
            {
                GameObject temp = Instantiate(blankInventorySlot, inventoryPanel.transform.position, Quaternion.identity);
                temp.transform.SetParent(inventoryPanel.transform);
                temp.transform.localScale = new Vector3(1, 1, 1);
                InventorySlot newSlot = temp.GetComponent<InventorySlot>();
                newSlot.Setup(playerInventory.myInventory[i],this);
            }
        }
    }

    void ClearInventorySlot()
    {
        for (int i=0;i<inventoryPanel.transform.childCount;i++)
        {
            Destroy(inventoryPanel.transform.GetChild(i).gameObject);
        }
    }

    private void OnEnable()
    {
        ClearInventorySlot();
        MakeInventorySlot();
        SetTextAndButton("",false);
    }

    public void SetupDescriptionAndButton(string newDescription,bool isButtonUsable,InventoryItem newItem)
    {
        currentItem = newItem;
        descriptionText.text = newDescription;
        useButton.SetActive(isButtonUsable);
    }

    public void UseButtonPressed()
    {
        currentItem.Use();
    }

    public void CloseInventory()
    {
        this.gameObject.SetActive(false);
    }
    
}