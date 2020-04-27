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
    private ItemType _attackingItem = null;
    private EnemyType _defendingEnemy = null;

    [SerializeField]
    private PlayerObject _playerObj;

    /* A random object must be created because if new random objects are generated within too close of a time, the numbers may end up become the same ( 4/27/2020 2:51pm )
     * System.Random utilizes the system time in order to generate random numbers
     */ 
    private System.Random _rnd = new System.Random();

    #region Delegates

    public delegate void PerformDamage(int _damage);
    public static event PerformDamage OnPlayerAttack;
    public static event PerformDamage OnEnemyAttack;

    #endregion

    void Awake() {
        Crafter.OnItemCrafted += UpdateAttackingItem;
        EnemyObject.OnEnemySelected += UpdateDefendingEnemy;
        OnBattlestateChanged += BattleManagerListener;
    }

    private void Start() {
        _playerObj = FindObjectOfType<PlayerObject>();
    }

    #region Functions

    public bool CheckVulnerabilities(EnemyType _defEnemy, ItemType _attItem) {
        for (int i = 0; i < _defEnemy.vulnerabilities.Length; i++) {
            if (_defEnemy.vulnerabilities[i] == _attItem)
                return true;
        }

        return false;
    }

    // This will be used to calculate the damage dealt ( 4/24/2020 1:04am )
    public int CalculateAttackDamage(int _atk, float _modifier = 1) {
        return (int)Math.Round(4.5 * _atk * _modifier);
    }

    #endregion

    #region Event Listeners

    public void UpdateAttackingItem(ItemType _item) {
        if (_item != null)
            _attackingItem = _item;
    }

    // Will update the defending enemy once the enemy has been selected. Will also change the gamestate ( 12/27/2019 1:08pm )
    public void UpdateDefendingEnemy(EnemyType _enemy) {
        _defendingEnemy = _enemy;

        SetCurrentState(Bstate.player_ATTACK);
    }

    // Fires when the gamestate has been changed ( 12/27/2019 1:14pm )
    public void BattleManagerListener(Bstate _state) {
        if (_state == Bstate.player_ATTACK) {
            float _vulModifier = 1;

            if (CheckVulnerabilities(_defendingEnemy, _attackingItem)) {
                // Add vulnerability to enemy inventory ( 4/24/2020 1:03am )
                EnemyInventory.instance.AddEnemyVul(_defendingEnemy, _attackingItem);

                _vulModifier = 1.5f;
                print("It's Super Effective!");
            } else {

                _vulModifier = 1.0f;
                print("Not Very Effective...");
            }

            OnPlayerAttack(CalculateAttackDamage(_attackingItem.baseAtk, _vulModifier));

        // When the it is time for the enemies to attack the player ( 4/27/2020 2:29pm )
        }  else if (_state == Bstate.enemy_ATTACK) {
            int _count = WaveManager.instance.enemyList.Count;

            int i = _rnd.Next(0, _count - 1);

            OnEnemyAttack(CalculateAttackDamage(WaveManager.instance.enemyList[i].enemyType.baseAtk));

        // Reset all values ( 4/27/2020 3:07pm )
        } else if (_state == Bstate.game_ROUNDRESET) {
            _defendingEnemy = null;
            _attackingItem = null;

            SetCurrentState(Bstate.player_CRAFT);
        }
    }

    #endregion

}
