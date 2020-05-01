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
    public List<ItemType> itemInv;
    public List<ItemType> startingInv; // ngl im not sure if I could've done without this but thsi straight from brackeys ( 5/30/2019 9:06pm )
    // Starting item list should be explicitly stated

    [SerializeField]
    private GameObject itemPrefab = null; // should be Dragged&Dropped in the inventory gameObject ( 5/30/2019 7:57pm )

    [SerializeField]
    private Transform inventoryGrid = null; // and also this

    [SerializeField]
    private GameObject ghostItem = null; // Displays an item in the inventory without actually being an item ( 6/12/2019 12:48pm )

    #region Singleton

    // hehe, learned this from Brackeys - Singleton is apparently a technique that restricts a class to a single instance ( 5/31/2019 1:48pm )
    public static Inventory instance;

    private void Awake() {
        instance = this;
    }

    #endregion

    void Start() {
        LoadItems();
    }

    #region Functions

    // Will Load all of the items from a File onto the actual inventory ( 12/29/2019 1:33pm )
    public void LoadItems() {
        if (FileIO_GameData.LoadInventory() != null) {
            List<ItemType> _itemList = FileIO_GameData.LoadInventory();

            foreach (ItemType item in _itemList) {
                AddItem(item);
            }
        } else {

            // If no inventory exists, it will create the File ( 2/25/2020 6:03pm )
            foreach (ItemType item in startingInv) {
                AddItem(item);
            }
        }
    }

    // Will add an item to the inventory, referenced as _item. Simple, right? ( 5/30/2019 8:01pm )
    public void AddItem(ItemType _item) {
        Debug.Log("Adding " + _item.itemName + " to inventory");

        itemInv.Add(_item);

        GameObject itemObj = Instantiate(itemPrefab, inventoryGrid);
        itemObj.GetComponent<ItemObject>().SetItem(_item);

        FileIO_GameData.SaveInventory(itemInv);
    }

    // Creation of ghost items. Not actually interactable. ( 6/12/2019 1:05pm )
    public GameObject CreateGhostItem(ItemType _item) {
        GameObject itemObj = Instantiate(ghostItem, inventoryGrid);
        itemObj.GetComponent<ItemObject>().SetItem(_item);

        return itemObj;
    }

    public void RemoveAllGhostItems() {
        GameObject[] itemObj = GameObject.FindGameObjectsWithTag("GhostItem");

        for (int i = 0; i < itemObj.Length; i++)
            Destroy(itemObj[i]);
    }

    // Just realize I might never need this, but I'm keeping it just in case ( 5/31/2019 1:26pm )
    public void RemoveItem(ItemType _item) {
        Debug.Log("Removing " + _item.itemName + " from inventory");

        itemInv.Remove(_item); //By the way this doesn't work as of now ( 5/31/2019 1:27pm )
    }

    public bool HasItem(ItemType _item) {
        return itemInv.Contains(_item);
    }


    #endregion

}
