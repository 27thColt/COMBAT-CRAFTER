using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                if (item.itemType is WeaponType) AddItem(item.itemType as WeaponType);
                else AddItem(item.itemType, item.number);
            }
        } else {
            Debug.LogError("Inventory cannot be loaded!");
        }
    }

    #region AddItem

    // Generic Add Item Function ( 5/10/2020 10:14pm )
    public void AddItem(ItemType _itemType, int _amt = 1) {
        // Will create the list if it does not exist ( 5/4/2020 7:46pm )
        if (itemInv == null) itemInv = new List<Item>(); 

        foreach (Item _item in itemInv) {

            // Item does exist in list and will add one to it ( 5/4/2020 5:56pm )
            if (_item.itemType == _itemType) {
            
                Debug.Log("Adding " + _amt + " to " + _item.itemType.itemName + " in inventory");

                _item.AddItem();
                ReturnItemFromArray(_itemType).GetComponent<ItemObject>().UpdateItemInfo(_item);
                
                return;
            }
        }

        // Item does not exist and will create a new instance in the list ( 5/4/2020 5:56pm )

        Debug.Log("Creating " + _amt + " " + _itemType.itemName);

        itemInv.Add(new Item(_itemType, _amt));
        CreateItemObj(_itemType, _amt);

        return;      
    }

    // Add Weapon function ( 5/10/2020 10:15pm )
    public void AddItem(WeaponType _weaponType) {
        // Will create the list if it does not exist ( 5/4/2020 7:46pm )
        if (itemInv == null) itemInv = new List<Item>(); 

        Debug.Log("Creating " + _weaponType.itemName + " WEAPON");

        itemInv.Add(new Weapon(_weaponType));
        CreateItemObj(_weaponType as WeaponType);

        return;
    }

    #endregion

    public void RemoveItem(ItemType _itemType) {
        for (int i = 0; i < itemInv.Count; i++) {
            if (itemInv[i].itemType == _itemType) {
                Debug.Log(itemInv[i].itemType.itemName + " " + itemInv[i].number);
                // If there is only 1 more item left, destroy the game object itself ( 5/5/2020 4:32pm )
                if (itemInv[i].number == 1) {
                    Debug.Log("only one left");
                    Destroy(ReturnItemFromArray(_itemType));

                    itemInv.Remove(itemInv[i]);

                    return;
                } else {
                    itemInv[i].RemoveItem();
                    ReturnItemFromArray(_itemType).GetComponent<ItemObject>().UpdateItemInfo(itemInv[i]);

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

    #region Static Functions

    // Creates an Item Object and sets it as the child of the inventory window ( 5/4/2020 5:52pm )
    public static void CreateItemObj(ItemType _itemType, int _amt = 1) {
        try {
            GameObject _parent = GameObject.FindGameObjectWithTag("InventoryWindow").GetComponentInChildren<GridLayoutGroup>().gameObject;

            GameObject _itemObj = Object.Instantiate(Resources.Load("Prefabs/Item") as GameObject, _parent.transform);

            _itemObj.transform.localScale = new Vector3(1, 1, 1);

            if (_itemType is WeaponType) {
                _itemObj.GetComponent<ItemObject>().SetItem(new Weapon(_itemType as WeaponType, _amt));
            } else {
                _itemObj.GetComponent<ItemObject>().SetItem(new Item(_itemType, _amt));
            }
            
        } catch {
            Debug.Log("Cannot Create Item, object with 'InventoryWindow' tag does not exist!");
        } 
    }

    // Returns the array of ItemObjs in the inventory Panel ( 5/5/2020 10:45am )
    public static List<GameObject> ReturnItemObjArray() {
        try {
            List<GameObject> _list = new List<GameObject>(); 

            foreach (Transform _child in GameObject.FindGameObjectWithTag("InventoryWindow").GetComponentInChildren<GridLayoutGroup>().gameObject.transform) {
                if (_child.gameObject.GetComponent<ItemObject>() != null) {
                    _list.Add(_child.gameObject);
                }
            }

            return _list;
        } catch {
            Debug.Log("Object with 'InventoryWindow' tag does not exist!");

            return null;
        }
    }

    // Returns the specific item object from the array ( 5/5/2020 10:47pm )
    public static GameObject ReturnItemFromArray(ItemType _itemType) {
        List<GameObject> _list = ReturnItemObjArray();

        foreach (GameObject _item in _list) {
            if (_item.GetComponent<ItemObject>().item.itemType == _itemType) {
                return _item;
            }
        }

        return null;
    }

    /* Not sure about the efficiency of this function; 
     * Used only during loading the Inventory savedata because serializing an 'item' scriptable object will just return a reference ID.
     * 
     * So instead I store the the item's ID instead. ( 2/29/2020 3:07pm )
     *
     */
    public static ItemType ReturnItemFromID(int _id) {
        ItemType[] _itemList = Resources.LoadAll<ItemType>("Item Types");
        for (int i = 0; i < _itemList.Length; i++) {
            if (_itemList[i].ID == _id) {
                return _itemList[i];
            }
        }

        return null;
    }

    #endregion

}
