using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;
using System.Runtime.ExceptionServices;

/* 12/28/2019 11:15pm - IO Inventory
 * Manages the saving and loading of inventory savedata.
 * 
 * 
 * Added SimpleJSON Library -- So far at the moment only using this for the vulnerabilities, but
 * at some point I also want to streamline everything so I will have to redo the inventory section as well ( 4/21/2020 10:30pm )
 */


public static class IO_Inventory {
    // Too lazy for this shit rn so here: https://www.youtube.com/watch?v=6uMFEM-napE ( 12/28/2019 11:15pm )

    private static readonly string _PATH = Application.dataPath + "/Save Data/inventory.json";

    // Loads savedata to the game; will return the inventory list ( 2/25/2020 5:36pm )
    public static List<Item> LoadInventory() {
        if (File.Exists(_PATH)) {
            JSONObject _json = (JSONObject)JSON.Parse(File.ReadAllText(_PATH));
            List<Item> _outputList = new List<Item>();

            for (int i = 0; i < _json["Inventory"].AsArray.Count; i++) {
                ItemType _itemType = Inventory.ReturnItemFromID(_json["Inventory"].AsArray[i]["ID"].AsInt);

                if (_itemType is WeaponType) {
                    _outputList.Add(new Weapon(_itemType as WeaponType, System.Guid.NewGuid(), 1,  _json["Inventory"].AsArray[i]["Durability"].AsInt));
                } else if (_itemType is PotionType) {
                    _outputList.Add(new Potion(_itemType,  System.Guid.NewGuid(), 1));
                } else { 
                    _outputList.Add(new Item(_itemType,  System.Guid.NewGuid(), _json["Inventory"].AsArray[i]["Number"].AsInt));
                }
            }

            return _outputList;
        } else {
            Debug.LogError(_PATH + "does not exist");
            return null;
        }
    }

    // Saves inventory; called through inventory class ( 4/21/2020 3:00pm )
    public static void SaveInventory(List<Item> _inventory) {
        JSONObject _json = new JSONObject();
        JSONArray _inv = new JSONArray();

        foreach (Item _itemObj in _inventory) {
            JSONObject _item = new JSONObject();
            
            _item.Add("ID", _itemObj.itemType.ID);

            // Weapon Item Clause ( 5/11/2020 1:21pm )
            if (_itemObj is Weapon) {
                Weapon _weaponObj = _itemObj as Weapon;
                _item.Add("Durability", _weaponObj.currentDurability);

            // Generic Item Clause ( 5/11/2020 1:20pm )
            } else if (_itemObj is Potion) { 
                // No need to add the number because potions should not be stackable ( 5/14/2020 3:51pm )

            } else {
                _item.Add("Number", _itemObj.number);
            }
            

            _inv.Add(_item);
        }

        _json.Add("Inventory", _inv);
        

        /* Final Output LIke This:
         * {"Inventory": [
		 *          {"ID": 0, "Number": 1}, 
         *          {"ID": 1, "VUL": 3}
	     *      ]
         *  }
         * 
         */

        // This function will just create the directory if none exist ( 2/25/2020 6:06pm )
        File.WriteAllText(_PATH, _json.ToString());
    }
    
}