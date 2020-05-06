using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;
using System.Runtime.ExceptionServices;

/* 12/28/2019 11:15pm - FileIO_GameData
 * Manages the saving and loading of savedata.
 * 
 * 
 * Added SimpleJSON Library -- So far at the moment only using this for the vulnerabilities, but
 * at some point I also want to streamline everything so I will have to redo the inventory section as well ( 4/21/2020 10:30pm )
 */


public static class FileIO_GameData {
    // Too lazy for this shit rn so here: https://www.youtube.com/watch?v=6uMFEM-napE ( 12/28/2019 11:15pm )

    #region Inventory
    private static readonly string _PATH = Application.dataPath + "/Save Data";

    // Loads savedata to the game; will return the inventory list ( 2/25/2020 5:36pm )
    public static List<Item> LoadInventory() {
        if (File.Exists(_PATH + "/inventory.json")) {
            JSONObject _json = (JSONObject)JSON.Parse(File.ReadAllText(_PATH + "/inventory.json"));
            List<Item> _outputList = new List<Item>();

            for (int i = 0; i < _json["Inventory"].AsArray.Count; i++) {
                _outputList.Add(new Item(ItemType.ReturnItemFromID(_json["Inventory"].AsArray[i]["ID"].AsInt), _json["Inventory"].AsArray[i]["Number"].AsInt));
            }

            return _outputList;
        } else {
            Debug.Log(_PATH + "/inventory.json " + "does not exist");
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
            _item.Add("Number", _itemObj.number);

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
        File.WriteAllText(_PATH + "/inventory.json", _json.ToString());
    }

    #endregion

    #region Enemy Inventory
    
    public static List<EnemyDefinition> LoadEnemyInv() {
        if (File.Exists(_PATH + "/vulnerabilities.json")) {
            JSONObject _json = (JSONObject)JSON.Parse(File.ReadAllText(_PATH + "/vulnerabilities.json"));

            List<EnemyDefinition> _outputList = new List<EnemyDefinition>();

            // Main Loop, loops through JSON whilst adding each definition ( 4/21/2020 11:25pm )
            for (int i = 0; i < _json["EnemyInv"].AsArray.Count; i++) {

                // Adds each vulnerability to separate list (conversion from array to list) ( 4/21/2020 11:25pm )
                List<int> _sList = new List<int>();
                for (int j = 0; j < _json["EnemyInv"].AsArray[i]["VUL"].AsArray.Count; j++) {
                    _sList.Add(_json["EnemyInv"].AsArray[i]["VUL"].AsArray[j]);
                }

                // Actual adding to the list of enemy definitions ( 4/21/2020 11:26pm )
                _outputList.Add(new EnemyDefinition {
                    enemyID = _json["EnemyInv"].AsArray[i]["ID"].AsInt,
                    vulnerabilities = _sList
                });
                
            }

            return _outputList;
        } else {
            Debug.Log(_PATH + "/vulnerabilities.json " + "does not exist");
            return null;
        }
    }

    // Utilizes SimpleJSON in order to parse, first function i made in this library ( 4/21/2020 10:56pm )
    public static void SaveEnemyInv(List<EnemyDefinition> _enemyInv) {
        JSONObject _json = new JSONObject();
        JSONArray _enemyDef = new JSONArray();

        // Looks through each enemy definition, and parses them into JSON ( 4/21/2020 11:18pm )
        foreach (EnemyDefinition enemyDefinition in _enemyInv) {
            JSONObject _definition = new JSONObject();

            _definition.Add("ID", enemyDefinition.enemyID);

            if (enemyDefinition.vulnerabilities.Count > 0) {
                JSONArray _vul = new JSONArray();

                foreach (int _itemID in enemyDefinition.vulnerabilities) {
                    _vul.Add(_itemID);
                }

                _definition.Add("VUL", _vul);
            }

            _enemyDef.Add(_definition);
        }

        _json.Add("EnemyInv", _enemyDef);


        /* Final Output LIke This:
         * {"EnemyInv": [
		 *          {"ID": 0, "VUL": [2]}, 
         *          {"ID": 1, "VUL": [3, 4]}
	     *      ]
         *  }
         * 
         */

        File.WriteAllText(_PATH + "/vulnerabilities.json", _json.ToString());
    }

    #endregion
}