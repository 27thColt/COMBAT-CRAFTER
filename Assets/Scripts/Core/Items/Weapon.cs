using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  5/10/2020 5:48pm - Weapon Class
    Used for WeaponType items, contains a couple more values


    5/11/2020 1:22pm - Honestly really dont feel like my current method for implementing this Weapon subclass is efficient so yeah

*/
[System.Serializable]
public class Weapon : Item {
    public WeaponType weaponType;
    public int maxDurability;
    public int currentDurability;


    public Weapon(ItemType itemType, System.Guid UID, int number = 1, int currentDurability = 0) : base(itemType, UID, number) {
        weaponType = itemType as WeaponType;

        maxDurability = weaponType.maxDurability;
        if (currentDurability == 0) this.currentDurability = maxDurability; // When no durability given, just use the max ( 5/11/2020 1:30pm )
        else this.currentDurability = currentDurability;

        // Updates the attack stat ( 5/13/2020 12:37pm )
        atk = atk - weaponType.atkDecayRate * (maxDurability - this.currentDurability);
    }

    override public void AddInstance(int amt = 1) {
        if (currentDurability < maxDurability) {
            currentDurability += amt;
            atk = atk + weaponType.atkDecayRate * (maxDurability - currentDurability);
            Debug.Log("Attack of " + itemType.itemName + " is " + atk);
        }
    }
    // Removes the durability ( 5/11/2020 10:46am )
    override public bool RemoveInstance(int amt = 1) {
        if (currentDurability == 1) {
            return false;
        } else {
            currentDurability -= amt;
            atk = atk - weaponType.atkDecayRate * (maxDurability - currentDurability);
            Debug.Log("Attack of " + itemType.itemName + " is " + atk);
            return true;
        }
    }
}
