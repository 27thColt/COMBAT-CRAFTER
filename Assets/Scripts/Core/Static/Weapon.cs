using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  5/10/2020 5:48pm - Weapon Class
    Used for WeaponType items, contains a couple more values

*/
public class Weapon : Item {
    public WeaponType _weaponType;
    public int maxDurability;
    public int currentDurability;
    public Weapon(ItemType _itemType, int _number = 1) : base(_itemType, _number) {
        _weaponType = _itemType as WeaponType;

        maxDurability = _weaponType.maxDurability;
        currentDurability = maxDurability;
    }

    override public void AddItem(int _amt = 1) {
        Debug.LogError(_weaponType.name + " is not stackable!");
    }
    // Removes the durability ( 5/11/2020 10:46am )
    override public void RemoveItem(int _amt = 1) {
        currentDurability -= 1;
        atk = _weaponType.baseAtk - _weaponType.atkDecayRate * _amt;
    }
}
