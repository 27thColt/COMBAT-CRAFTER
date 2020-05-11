using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  5/10/2020 2:49pm - Weapon Type Scriptable Object
    This class inherits from the ItemType class
    Notes:

    WEAPON ITEMS

    Crafted weapons have a durability stat and will break when their durability is over

    This means they may be used multiple times before they break

    Weapons typically have a higher initial attack stat than other items, but reduce as their durability goes down
    
    Weapons do significantly less damage when they are not effective to a certain enemy type

    (regular not effective dmg modifier: 1
    weapon not effective dmg modifier: 0.75)
*/

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 2)]
public class WeaponType : ItemType {
    public int maxDurability = 3; // For how many times an item can last ( 5/10/2020 3:49pm )
    public int atkDecayRate = 1; // How much the atk will decay when durability is decreased
}
