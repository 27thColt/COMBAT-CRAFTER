﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 5/30/2019 8:06pm - Item Display script
    * Attached onto the item prefab. Takes information from the item scriptable object & class to be used in the UI.
    * 
    * Gammer - Jaws 2016 is a nice song, actually Gammer is just plain awesome
    */
public class ItemObject : MonoBehaviour {

    public Item item; // Determines the item, duh ( 5/30/2019 8:09pm )

    [SerializeField]
    private TextMeshProUGUI _textMesh = null; // TextMesh Pro game object in the child ( 5/30/2019 8:51pm )

    [SerializeField]
    private Image _displayImage = null; // Image game object in the child ( 5/30/2019 8:57pm )

    [SerializeField]
    private TextMeshProUGUI _number = null;

    public void SetItem(Item _item) {
        item = _item;
        _textMesh.text = _item.itemType.itemName;
        _displayImage.sprite = _item.itemType.sprite;
        _number.text = _item.number.ToString();
    }

    public void UpdateItemInfo(Item _item) {
        _number.text = _item.number.ToString();
    }


}


/*  5/4/2020 5:44pm - Item Class
 *  This class is the actual item itself. Holds a number (for how many items) and a type.
 * 
 */
public class Item {
    public ItemType itemType;
    public int number;

    public Item(ItemType _itemType, int _number) {
        itemType = _itemType;
        number = _number;
    }

    public void AddItem(int _amt = 1) {
        number += _amt;
    }

    public void RemoveItem(int _amt = 1) {
        number -= _amt;
    }

    
    // Creates an Item Object and sets it as the child of the inventory window ( 5/4/2020 5:52pm )
    public static void CreateItemObj(ItemType _itemType, int _amt = 1) {
        try {
            GameObject _parent = GameObject.FindGameObjectWithTag("InventoryWindow").GetComponentInChildren<GridLayoutGroup>().gameObject;

            GameObject _itemObj = Object.Instantiate(Resources.Load("Prefabs/Item") as GameObject, _parent.transform);

            _itemObj.transform.localScale = new Vector3(1, 1, 1);

            _itemObj.GetComponent<ItemObject>().SetItem(new Item(_itemType, _amt));
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
}