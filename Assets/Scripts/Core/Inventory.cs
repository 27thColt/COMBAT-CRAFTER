using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 5/30/2019 7:55pm - Inventory script
 * The actual inventory which stores the list of items.
 * Using a lot of the code from Brackey's game Ludum Dare 41 game "Crafty" as a reference for a lot of the code here
 * 
 * Also, pretty sure that this file does not reference any other file which is good.
 * 
 * In the far future, hoping to add system where is reads data off of an external file to gather the list
 */ 
public class Inventory : MonoBehaviour {
    #region Singleton

    private static Inventory _inventory;

    // hehe, learned this from Brackeys - Singleton is apparently a technique that restricts a class to a single instance ( 5/31/2019 1:48pm )
    public static Inventory instance {
        get {
            if (!_inventory) {
                _inventory = FindObjectOfType(typeof(Inventory)) as Inventory;

                if (!_inventory) {
                    Debug.LogError("There needs to be one active Inventory script on a GameObject in your scene.");
                }
            }

            return _inventory;
        }
    }

    #endregion
    
    public List<Item> itemInv;
    public List<ItemType> startingInv; // ngl im not sure if I could've done without this but thsi straight from brackeys ( 5/30/2019 9:06pm )
    // Starting item list should be explicitly stated

    void Start() {
        LoadItems();
    }

    #region Functions

    // Will Load all of the items from a File onto the actual inventory ( 12/29/2019 1:33pm )
    public void LoadItems() {
        Debug.Log("Loading Items");
        if (IO_Inventory.LoadInventory() != null) {
            List<Item> _itemList = IO_Inventory.LoadInventory();

            foreach (Item item in _itemList) {
                AddItem(item.itemType, item.number);
            }
        } else {

            // If no inventory exists, it will create the File ( 2/25/2020 6:03pm )
            foreach (ItemType item in startingInv) {
                AddItem(item);
            }
        }
    }

    // Will add an item to the inventory. Simple, right? ( 5/30/2019 8:01pm )
    public void AddItem(ItemType _itemType, int _amt = 1) {
        if (itemInv != null) {
            foreach (Item _item in itemInv) {

                // Item does exist in list and will add one to it ( 5/4/2020 5:56pm )
                if (_item.itemType == _itemType) {
                    
                    Debug.Log("Adding " + _amt + " to " + _item.itemType.itemName + " in inventory");

                    _item.AddItem();
                    Item.ReturnItemFromArray(_itemType).GetComponent<ItemObject>().UpdateItemInfo(_item);
                    
                    return;
                }
            }

            // Item does not exist and will create a new instance in the list ( 5/4/2020 5:56pm )
            Debug.Log("Creating " + _amt + " " + _itemType.itemName);

            itemInv.Add(new Item(_itemType, _amt));
            Item.CreateItemObj(_itemType, _amt);

            return;

        } else {
            // Will create the list if it does not exist ( 5/4/2020 7:46pm )

            Debug.Log("Creating " + _amt + " " + _itemType.itemName);


            itemInv = new List<Item>();
            itemInv.Add(new Item(_itemType, _amt));
            Item.CreateItemObj(_itemType, _amt);

            return;
        }
        
    }


    public void RemoveItem(ItemType _itemType) {
        for (int i = 0; i < itemInv.Count; i++) {
            if (itemInv[i].itemType == _itemType) {
                Debug.Log(itemInv[i].itemType.itemName + " " + itemInv[i].number);
                // If there is only 1 more item left, destroy the game object itself ( 5/5/2020 4:32pm )
                if (itemInv[i].number == 1) {
                    Debug.Log("only one left");
                    Destroy(Item.ReturnItemFromArray(_itemType));

                    itemInv.Remove(itemInv[i]);

                    return;
                } else {
                    itemInv[i].RemoveItem();
                    Item.ReturnItemFromArray(_itemType).GetComponent<ItemObject>().UpdateItemInfo(itemInv[i]);

                    return;
                }
            }
        }
    }

    public bool HasItem(ItemType _itemType) {
        foreach (Item _item in itemInv) {
            if (_item.itemType == _itemType)
                return true;
        }

        return false;
    }


    #endregion

}
