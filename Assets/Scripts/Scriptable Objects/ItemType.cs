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
    public int baseAtk; // The base attack damage that the weapon will do ( 4/24/2020 12:47am )
}
