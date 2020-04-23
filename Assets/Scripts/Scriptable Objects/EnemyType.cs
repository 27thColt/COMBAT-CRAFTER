using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 6/12/2019 11:39am - Enemy Scriptable Object
 * The game will consist of you battling enemies using the items that you craft.
 * This class will hold the properties of each enemy.
 * 
 */
[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy", order = 2)]
public class EnemyType : ScriptableObject {
    public string enemyName;
    public int ID = 0;
    public Sprite sprite;
    public ItemType[] vulnerabilities; // What the enemy will be weak to ( 6/12/2019 11:41am )
    public ItemType[] drops; // What the enemy will drop upon defeating ( 6/12/2019 11:41am )
    public int baseHP; // base number of HP that an enemy may have ( 4/23/20220 5:32pm )


    /* Not sure about the efficiency of this function; 
     * Used only during loading the savedata because serializing an 'enemy' scriptable object will just return a reference ID.
     * 
     * So instead I store the the item's ID instead. ( 2/29/2020 3:07pm )
     *
     */
    public static EnemyType ReturnEnemyfromID(int _id) {
        EnemyType[] _enemyList = Resources.LoadAll<EnemyType>("Enemy Types");
        for (int i = 0; i < _enemyList.Length; i++) {
            if (_enemyList[i].ID == _id) {
                return _enemyList[i];
            }
        }

        return null;
    }
}
