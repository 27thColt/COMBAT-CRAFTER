using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  5/13/2020 5:22pm - Potion Type Scriptable Object
    This class inherits from ItemType class

    Notes:

    POTION ITEMS

    Healing items will heal the player on being crafted

    Thinking they can only be obtained by an outside source (outside of battles)

    Perhaps there is an alchemist character that you meet that will give potions if you pay items
*/
[CreateAssetMenu(fileName = "Potion", menuName = "Potion", order = 3)]
public class PotionType : ItemType {
    public int regen; // How much HP the potion will regen ( 5/13/2020 5:27pm )
}
