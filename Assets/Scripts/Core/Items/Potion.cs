﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  5/14/2020 1:49pm - Potion Class
    Inherits from the item class


*/
[System.Serializable]
public class Potion : Item {
    public PotionType potionType;

    public Potion(ItemType _itemType, int _UID, int _number) : base(_itemType, _UID, _number) {
        potionType = _itemType as PotionType;
    }

    override public void AddInstance(int _amt = 1) {
        // Potions are not stackable ( 8/1/2020 2:34pm )
        // Debug.LogError(potionType.itemName + " is not stackable!");
    }

    override public bool RemoveInstance(int _amt = 1) {
        // Potions are not stackable so there will always be one of a certain item ( 5/14/2020 1:52pm )
        return false;
    }

    override public void OnCraft() {
        Debug.LogError("Potion types are not craftable!");
    }
}
