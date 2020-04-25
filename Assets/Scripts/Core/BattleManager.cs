using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleState;

/* 12/27/2019 12:47pm - Battle Manager
 * Handles game batle logic and all that shit
 * reconciles the differences between man and AI
 * 
 */
public class BattleManager : MonoBehaviour {

    // Item used to attack (comes from resultItem in the Crafter) ( 12/27/2019 12:50pm )
    private ItemType attackingItem;
    private EnemyType defendingEnemy;

    public delegate void PerformDamage(int _damage);
    public static event PerformDamage OnDamagePerformed;


    void Awake() {
        Crafter.OnItemCrafted += UpdateAttackingItem;
        EnemyScript.OnEnemySelected += UpdateDefendingEnemy;
        OnBattlestateChanged += BattleManagerListener;
    }

    #region Functions

    public bool CheckVulnerabilities(EnemyType _defEnemy, ItemType _attItem) {
        for (int i = 0; i < _defEnemy.vulnerabilities.Length; i++) {
            if (defendingEnemy.vulnerabilities[i] == _attItem)
                return true;
        }

        return false;
    }

    // This will be used to calculate the damage dealt from the attack to the enemy ( 4/24/2020 1:04am )
    public int CalculateDamage(EnemyType _enemy, ItemType _item) {
        int _outputDamage;
        float vulModifer;

        if (CheckVulnerabilities(_enemy, _item)) {
            vulModifer = 1.5f;
        } else {
            vulModifer = 1.0f;
        }

        _outputDamage = (int)Math.Round(4.5 * _item.baseAtk * vulModifer);

        return _outputDamage;
    }

    #endregion

    #region Event Listeners

    public void UpdateAttackingItem(ItemType _item) {
        if (_item != null)
            attackingItem = _item;
    }

    // Will update the defending enemy once the enemy has been selected. Will also change the gamestate ( 12/27/2019 1:08pm )
    public void UpdateDefendingEnemy(EnemyType _enemy) {
        defendingEnemy = _enemy;

        SetCurrentState(Bstate.game_CALCULATE);
    }

    // Fires when the gamestate has been changed ( 12/27/2019 1:14pm )
    public void BattleManagerListener(Bstate _state) {
        if (_state == Bstate.game_CALCULATE) {
            if (CheckVulnerabilities(defendingEnemy, attackingItem)) {
                // Add vulnerability to enemy inventory ( 4/24/2020 1:03am )
                EnemyInventory.instance.AddEnemyVul(defendingEnemy, attackingItem);
                print("It's Super Effective!");
            } else {
                print("Not Very Effective...");
            }

            OnDamagePerformed(CalculateDamage(defendingEnemy, attackingItem));
        }
    }

    #endregion

}
