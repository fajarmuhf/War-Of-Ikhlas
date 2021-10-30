using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Item : NetworkBehaviour
{
    [SerializeField] private InventoryItem thisItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (isServer)
        {
            if (other.CompareTag("Player"))
            {
                AddItemToInventory(other);
                Destroy(this.gameObject);
                NetworkServer.Destroy(this.gameObject);
            }
        }
    }

    public void AddItemToInventory(Collider2D other)
    {
        if (other.GetComponent<Player>().playerInventory && thisItem)
        {
            int found = 0;
            for (int i = 0; i < other.GetComponent<Player>().playerInventory.myInventory.Count; i++)
            {
                if (other.GetComponent<Player>().playerInventory.myInventory[i].itemId == thisItem.itemId)
                {
                    other.GetComponent<Player>().playerInventory.myInventory[i].numberHeld++;
                }
                else
                {
                    found++;
                }
            }
            Debug.Log("f "+found);
            Debug.Log("c "+other.GetComponent<Player>().playerInventory.myInventory.Count);
            if (found == other.GetComponent<Player>().playerInventory.myInventory.Count)
            {
                InventoryItem itemBaru = new InventoryItem();
                itemBaru.itemId = thisItem.itemId;
                itemBaru.itemName = thisItem.itemName;
                itemBaru.itemDescription = thisItem.itemDescription;
                itemBaru.itemImage = thisItem.itemImage;
                itemBaru.itemImageIndex = thisItem.itemImageIndex;
                itemBaru.numberHeld = thisItem.numberHeld;
                itemBaru.usable = thisItem.usable;
                itemBaru.usable = thisItem.unique;
                itemBaru.thisEvent = thisItem.thisEvent;
                other.GetComponent<Player>().playerInventory.myInventory.Add(itemBaru);
                PlayerInventory temp = other.GetComponent<Player>().playerInventory;
                other.GetComponent<Player>().playerInventory = null;
                other.GetComponent<Player>().playerInventory = temp;
            }
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
