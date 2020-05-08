using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*  5/8/2020 3:32pm - Save Button
    Basically just saves the inventory

*/
public class SaveButton : MonoBehaviour {
    public void OnSaveButtonPressed() {
        IO_Inventory.SaveInventory(Inventory.instance.itemInv);
        Debug.Log("Inventory Saved!");
    }
}
