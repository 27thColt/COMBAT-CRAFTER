using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  9/7/2020 2:50pm - Loot Table
    Object used to hold an item and the probability of the player obtaining one

    Designed to be in lists
*/
[System.Serializable]
public class LootTable {
    private System.Random _rnd;
    public List<weightedItem> weightedList;
    public int totalWeight;

    public ItemType GenerateFromTable() {
        _rnd = new System.Random();

        int i = _rnd.Next(0, totalWeight + 1);

        foreach(weightedItem WI in weightedList) {
            if (i < WI.weight) {
                return WI.itemType;
            } else {
                i = i - WI.weight;
            }   
        }

        return null;
    }
}

[System.Serializable]
public struct weightedItem {
    public ItemType itemType;
    public int weight;
}