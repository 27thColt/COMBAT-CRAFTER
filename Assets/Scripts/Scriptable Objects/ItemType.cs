using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/* 5/30/2019 7:45pm - Item scriptable object
 * holds information for the items used for crafting in the game
 * 
 */ 
 [System.Serializable]
 [CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]
public class ItemType : ScriptableObject {
    public string itemName = "ItemName";
    public int ID = 0;
    public Sprite sprite;
    public bool isWeapon; // If the item is a 'weapon', then it won't be able to be used in the crafter, and will be used immediately to attack enemies ( 6/3/2019 8:15pm )

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
}

