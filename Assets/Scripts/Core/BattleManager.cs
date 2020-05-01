using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleState;

/* 12/27/2019 12:47pm - Battle Manager
 * Handles game batle logic and all that shit
 * reconciles the differences between man and AI
 * 
 * 
 * 5/1/2020 1:11pm - Some refactoring going to be done,, really want to centralize the battle code onto the battle manager
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

    #region Awake
    void Awake() {
        Crafter.OnItemCrafted += UpdateAttackingItem;
        EnemyObject.OnEnemySelected += UpdateDefendingEnemy;
        OnBattlestateChanged += BStateChanged_BMListener;
        OnBattlestateFinished += BStateFinished_BMListener;
    }

    #endregion

    private void OnDestroy() {
        Crafter.OnItemCrafted -= UpdateAttackingItem;
        EnemyObject.OnEnemySelected -= UpdateDefendingEnemy;
        OnBattlestateChanged -= BStateChanged_BMListener;
        OnBattlestateFinished -= BStateFinished_BMListener;
    }

    private void Start() {
        _playerObj = FindObjectOfType<PlayerObject>();

        // Calls WaveManager.cs ( 5/1/2020 1:11pm )
        SetCurrentState(Bstate.game_LOADWAVE);
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

        // Current state should be Bstate.player_ENEMYSELECTION ( 5/1/2020 1:27pm )
        FinishCurrentState(Bstate.player_ENEMYSELECTION);
    }

    public void BStateFinished_BMListener(Bstate _state) {
        if (_state == Bstate.game_LOADWAVE || _state == Bstate.game_ROUNDRESET) {
            SetCurrentState(Bstate.player_CRAFT);   // Crafter.cs ( 5/1/2020 1:44pm )

        } else if (_state == Bstate.player_CRAFT) {
            SetCurrentState(Bstate.player_ENEMYSELECTION);  //EnemyObject.cs ( 5/1/2020 1:44pm )

        } else if (_state == Bstate.player_ENEMYSELECTION) {
            SetCurrentState(Bstate.player_ATTACK);  // BattleManager.cs ( 5/1/2020 1:44pm )

        } else if (_state == Bstate.player_ATTACK) {
            if (WaveManager.instance.enemyList.Count > 0) {
                SetCurrentState(Bstate.enemy_ATTACK);   // BattleManager.cs ( 5/1/2020 1:46pm )

            } else {
                // Fires if all enemies have been defeated ( 5/1/2020 5:29pm )
                Debug.Break();
            }
            
            //playerattack_animation
        /*
        } else if (_state == Bstate.playerattack_ANIMATE) {
            SetCurrentState(Bstate.enemy_ATTACK);*/

        } else if (_state == Bstate.enemy_ATTACK) {
            SetCurrentState(Bstate.game_ROUNDRESET);
        }
    }


    // Fires when the gamestate has been changed ( 12/27/2019 1:14pm )
    public void BStateChanged_BMListener(Bstate _state) {
        if (_state == Bstate.player_ATTACK) {
            float _vulModifier;

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


            int i = _rnd.Next(0, _count);

            Debug.Log(i);

            WaveManager.instance.enemyList[i].SetAttacking(true);
            OnEnemyAttack(CalculateAttackDamage(WaveManager.instance.enemyList[i].enemyType.baseAtk));

        // Reset all values ( 4/27/2020 3:07pm )
        } else if (_state == Bstate.game_ROUNDRESET) {
            _defendingEnemy = null;
            _attackingItem = null;

            FinishCurrentState(Bstate.game_ROUNDRESET);
            
        }
    }

    #endregion

}
