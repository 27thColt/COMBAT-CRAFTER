using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 4/21/2020 2:47pm - Enemy Inventory
 * 
 * Alright so finally just realzied how to make the whole, "saving enemy vulnerabilties" thing gonna work
 * Basically an inventory of known enemy vulnerabilities
 * 
 * Just realized that its almost a year since I started work on this, my god progress is so slow
 */

public class EnemyInventory : MonoBehaviour {
    #region Singleton
    private static EnemyInventory _enemyInventory;

    public static EnemyInventory instance {
        get {
            if (!_enemyInventory) {
                _enemyInventory = FindObjectOfType(typeof(EnemyInventory)) as EnemyInventory;

                if (!_enemyInventory) {
                    Debug.LogError("There needs to be one active EnemyInventory script on a GameObject in your scene.");
                }
            }

            return _enemyInventory;
        }
    }

    #endregion

    public List<EnemyDefinition> enemyInventory = new List<EnemyDefinition>();

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
    
    public void LoadEnemyDefs() {
        if (IO_EnemyInv.LoadEnemyInv() != null) {
            enemyInventory = IO_EnemyInv.LoadEnemyInv();
        }
    }

    // Adds an enemy to the inventory, doesn't do anything if it already exists ( 4/21/2020 5:10pm )
    public void AddEnemyDef(EnemyType _enemy) {
        if (enemyInventory.Count != 0 && CheckInvFor(_enemy) != null)
            return;


        EnemyDefinition _enemyDef = new EnemyDefinition {
            enemyID = _enemy.ID,
            vulnerabilities = new List<int>()
        };

        enemyInventory.Add(_enemyDef);
        Debug.Log("Adding " + _enemy.enemyName + " to inventory");


        IO_EnemyInv.SaveEnemyInv(enemyInventory);
        return;
    }

    // Adds enemy vulnerability to an existing enemy, adds the enemy if it does not exist yet ( 4/21/2020 5:11pm )
    public void AddEnemyVul(EnemyType _enemy, ItemType _item) {
        if (CheckInvFor(_enemy) != null) {
            if (!CheckInvFor(_enemy).vulnerabilities.Contains(_item.ID)) {
                Debug.Log("Adding " + _item.itemName + " to " + _enemy.enemyName + " definition");
                CheckInvFor(_enemy).vulnerabilities.Add(_item.ID);

                IO_EnemyInv.SaveEnemyInv(enemyInventory);
            }
            return;
        }

        Debug.Log("Adding " + _item.itemName + " to " + _enemy.enemyName + " definition");
        AddEnemyDef(_enemy);
        AddEnemyVul(_enemy, _item);

        IO_EnemyInv.SaveEnemyInv(enemyInventory);
        return;
    }

    public EnemyDefinition CheckInvFor(EnemyType _enemy) {
        for (int i = 0; i < enemyInventory.Count; i++) {
            if (enemyInventory[i].enemyID == _enemy.ID) {
                return enemyInventory[i];
            }
        }

        return null;
    }

}

// Enemy Definitions are added to the inventory-- each one contains the current known vulnerabilities ( 4/21/2020 2:58pm )
public class EnemyDefinition {
    public int enemyID;
    public List<int> vulnerabilities;
}
