using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*  5/4/2020 5:44pm - Item Class
 *  This class is the actual item itself. Holds a number (for how many items) and a type.
 * 
 */
public class Item {
    public ItemType itemType;
    public int number;
    public int atk; // Stored value for the attack value of the item ( 5/11/2020 10:37pm )

    public Item(ItemType _itemType, int _number) {
        itemType = _itemType;
        atk = itemType.baseAtk;
    }

    virtual public void AddItem(int _amt = 1) {
        number += _amt;
    }

    virtual public void RemoveItem(int _amt = 1) {
        number -= _amt;
    }
}
