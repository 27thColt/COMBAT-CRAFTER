using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;

/*  5/8/2020 3:38pm - IO Enemy Inventory
    Manages the saving and loading of the enemy inventory



*/
public static class IO_EnemyInv {
    private static readonly string _PATH = Application.dataPath + "/Save Data/vulnerabilities.json";
    
    public static List<EnemyDefinition> LoadEnemyInv() {
        if (File.Exists(_PATH)) {
            JSONObject _json = (JSONObject)JSON.Parse(File.ReadAllText(_PATH));

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
            Debug.LogError(_PATH + "does not exist!");
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

        File.WriteAllText(_PATH, _json.ToString());
    }
}
