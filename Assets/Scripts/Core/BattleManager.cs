using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleStateManager;

/* 12/27/2019 12:47pm - Battle Manager
 * Handles game batle logic and all that shit
 * reconciles the differences between man and AI
 * 
 */
public class BattleManager : MonoBehaviour {

    // Item used to attack (comes from resultItem in the Crafter) ( 12/27/2019 12:50pm )
    private ItemType attackingItem;
    private EnemyType defendingEnemyType;


    void Awake() {
        Crafter.OnItemCrafted += UpdateAttackingItem;
        EnemyScript.OnEnemySelected += UpdateDefendingEnemy;
        OnBattlestateChanged += BattleManagerListener;
    }

    #region Functions

    public bool CheckVulnerabilities(EnemyType _defEnemy, ItemType _attItem) {
        for (int i = 0; i < _defEnemy.vulnerabilities.Length; i++) {
            if (defendingEnemyType.vulnerabilities[i] == _attItem)
                return true;
        }

        return false;
    }

    #endregion

    #region Event Listeners

    public void UpdateAttackingItem(ItemType _item) {
        if (_item != null)
            attackingItem = _item;
    }

    // Will update the defending enemy once the enemy has been selected. Will also change the gamestate ( 12/27/2019 1:08pm )
    public void UpdateDefendingEnemy(EnemyType _enemy) {
        defendingEnemyType = _enemy;

        SetCurrentState(Battlestate.game_CALCULATE);
    }

    // Fires when the gamestate has been changed ( 12/27/2019 1:14pm )
    public void BattleManagerListener(Battlestate _state) {
        if (_state == Battlestate.game_CALCULATE) {
            if (CheckVulnerabilities(defendingEnemyType, attackingItem)) {
                print("It's Super Effective!");

                EnemyInventory.instance.AddEnemyVul(defendingEnemyType, attackingItem);

            } else {
                print("Not Very Effective...");
            }
        }
    }

    #endregion

}
