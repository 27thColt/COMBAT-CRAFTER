using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleStateClass;


/* 12/27/2019 12:47pm - Battle Manager
 * Handles game batle logic and all that shit
 * reconciles the differences between man and AI
 * 
 */ 
public class BattleManager : MonoBehaviour {

    // Item used to attack (comes from resultItem in the Crafter) ( 12/27/2019 12:50pm )
    public Item attackingItem;
    public Enemy defendingEnemy;


    void Awake() {
        Crafter.OnItemCrafted += UpdateAttackingItem;
        EnemyScript.OnEnemySelected += UpdateDefendingEnemy;
        OnBattlestateChanged += BattleManagerListener;
    }

    #region Functions

    public bool CheckVulnerabilities(Enemy _defEnemy, Item _attItem) {
        for (int i = 0; i < _defEnemy.vulnerabilities.Length; i++) {
            if (defendingEnemy.vulnerabilities[i] == _attItem)
                return true;
        }

        return false;
    }

    #endregion

    #region Event Listeners

    public void UpdateAttackingItem(Item _item) {
        if (_item != null)
            attackingItem = _item;
    }

    // Will update the defending enemy once the enemy has been selected. Will also change the gamestate ( 12/27/2019 1:08pm )
    public void UpdateDefendingEnemy(Enemy _enemy) {
        defendingEnemy = _enemy;

        //FileIO_GameData.SaveVulnerabilities();
        //FileIO_GameData.AddVulnerability(defendingEnemy.ID, attackingItem.ID);

        SetCurrentState(Battlestate.game_CALCULATE);
    }

    // Fires when the gamestate has been changed ( 12/27/2019 1:14pm )
    public void BattleManagerListener(Battlestate _state) {
        if (_state == Battlestate.game_CALCULATE) {
            if (CheckVulnerabilities(defendingEnemy, attackingItem)) {
                print("It's Super Effective!");

                EnemyInventory.instance.AddEnemyVul(defendingEnemy, attackingItem);
            } else {
                print("Not Very Effective...");
            }
        }
    }

    #endregion

}
