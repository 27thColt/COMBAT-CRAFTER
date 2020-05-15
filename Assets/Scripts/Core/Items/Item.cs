using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*  5/4/2020 5:44pm - Item Class
 *  This class is the actual item itself. Holds a number (for how many items) and a type.
 * 
 */
 [System.Serializable]
public class Item {
    public ItemType itemType;
    public int number;
    public int UID; // Unique Identifier,  just an integer for now yoikes ( 5/13/2020 11:04 )
    public int atk; // Stored value for the attack value of the item ( 5/11/2020 10:37pm )

    public Item(ItemType _itemType, int _UID, int _number = 1) {
        itemType = _itemType;
        number = _number;
        atk = itemType.baseAtk;
        UID = _UID;
    }

    virtual public void AddInstance(int _amt = 1) {
        number += _amt;
    }

    // Returns false if the item is completely depleted and true if there are still items left ( 5/11/2020 2:44pm )
    virtual public bool RemoveInstance(int _amt = 1) {
        if (number == 1) {
            return false;
        } else {
            number -= _amt;
            return true;
        } 
    }

    // Function which is called upon being crafted ( 5/13/2020 3:57pm )
    virtual public void OnCraft() {
        Inventory.instance.AddItem(this);
    }
}
