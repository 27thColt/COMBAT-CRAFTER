using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleState;
using static LevelState;
using static BattleLogic;

/* 12/27/2019 12:47pm - Battle Manager
 * Handles game batle logic and all that shit
 * reconciles the differences between man and AI
 * 
 * 
 * 5/1/2020 1:11pm - Some refactoring going to be done,, really want to centralize the battle code onto the battle manager
 */
public class BattleManager : MonoBehaviour {
    // Item used to attack (comes from resultItem in the Crafter) ( 12/27/2019 12:50pm )
    private Item _attackingItem = null;
    private EnemyType _defendingEnemy = null;

    [SerializeField]
    private PlayerEntity _playerObj;

    /* A random object must be created because if new random objects are generated within too close of a time, the numbers may end up become the same ( 4/27/2020 2:51pm )
     * System.Random utilizes the system time in order to generate random numbers
     */ 
    private System.Random _rnd = new System.Random();

    #region Awake
    void Awake() {
        EventManager.StartListening("LStateChange", On_LStateChange);

        EventManager.StartListening("BStateFinish", On_BStateFinish);
        EventManager.StartListening("BStateChange", On_BStateChange);

        EventManager.StartListening("ItemCraft", On_ItemCraft);
        EventManager.StartListening("EnemySelect", On_EnemySelect);
        EventManager.StartListening("CancelCraft", On_CancelCraft);
    }

    #endregion

    private void OnDestroy() {
        EventManager.StopListening("LStateChange", On_LStateChange);

        EventManager.StopListening("BStateFinish", On_BStateFinish);
        EventManager.StopListening("BStateChange", On_BStateChange);

        EventManager.StopListening("ItemCraft", On_ItemCraft);
        EventManager.StopListening("EnemySelect", On_EnemySelect);
        EventManager.StopListening("CancelCraft", On_CancelCraft);
    }

    private void Start() {
        _playerObj = FindObjectOfType<PlayerEntity>();
    }

    #region Event Listeners

    #region Levelstate

    private void On_LStateChange(EventParams _eventParams) {
        switch(_eventParams.lstateParam) {
            case Lstate.EXPLORE:
                break;
            case Lstate.BATTLE:
                // Calls WaveManager.cs ( 5/1/2020 1:11pm )
                SetCurrentBState(Bstate.game_LOADWAVE);

                break;
            default:
                break;
        }
    }

    #endregion

    #region Battlestate
    // Dictionary Tag "BStateFinish" ( 5/6/2020 11:46pm )
    private void On_BStateFinish(EventParams eventParams) {
        switch (eventParams.bstateParam) {
            case Bstate.game_LOADWAVE:
                SetCurrentBState(Bstate.player_CRAFT);
                break;

            case Bstate.player_CRAFT:
                
                // In the case that the player crafted a potion ( 5/14/2020 1:58pm )
                if (_attackingItem is Potion) {
                   SetCurrentBState(Bstate.player_ATTACK);
                // In any other case
                } else {
                    SetCurrentBState(Bstate.player_ENEMYSELECTION);
                }
                break;

            case Bstate.player_ENEMYSELECTION:
                SetCurrentBState(Bstate.player_ATTACK);
                break;

            case Bstate.player_ATTACK:
                if (WaveManager.instance.enemyList.Count > 0) {
                    SetCurrentBState(Bstate.enemy_ATTACK);   // BattleManager.cs ( 5/1/2020 1:46pm )

                } else {
                    // Fires if all enemies in the wavee have been defeated ( 5/1/2020 5:29pm )
                    SetCurrentBState(Bstate.none);
                    FinishCurrentLState(Lstate.BATTLE);
                    Debug.Break();
                }
                break;

            case Bstate.enemy_ATTACK:
                SetCurrentBState(Bstate.game_ROUNDRESET);
                break;

            case Bstate.game_ROUNDRESET:
                SetCurrentBState(Bstate.player_CRAFT);
                break;

            default:
                break;
        }
    }

    private void On_BStateChange(EventParams _eventParams) {
        if (_eventParams.bstateParam != Bstate.none) {
            Bstate _state = _eventParams.bstateParam;
            if (_state == Bstate.player_ATTACK) {
                PlayerAttackState();

            // When the it is time for the enemies to attack the player ( 4/27/2020 2:29pm )
            }  else if (_state == Bstate.enemy_ATTACK) {
                int _count = WaveManager.instance.enemyList.Count;

                int i = _rnd.Next(0, _count);

                Debug.Log("PICKING ENEMY OF " + i);
                WaveManager.instance.enemyList[i].SetAttacking(true);
                EventManager.TriggerEvent("EnemyAttack", new EventParams(CalculateAttackDamage(WaveManager.instance.enemyList[i].enemyType.baseAtk, 1, true, _rnd)));

            // Reset all values ( 4/27/2020 3:07pm )
            } else if (_state == Bstate.game_ROUNDRESET) {
                _defendingEnemy = null;
                _attackingItem = null;

                FinishCurrentBState(Bstate.game_ROUNDRESET);
                
            }
        }
    }

    #endregion

    private void On_CancelCraft(EventParams _eventParams) {
        SetCurrentBState(Bstate.player_CRAFT);
    }

    // Updates the Attacking Item when it is crafted ( 5/7/2020 11:35pm )
    private void On_ItemCraft(EventParams _eventParams) {
        if (_eventParams.itemParam != null)
            _attackingItem = _eventParams.itemParam;
        else {
            Debug.LogError("EventParams with non-null itemParam expected.");
        }
    }

    // Will update the defending enemy once the enemy has been selected. Will also change the gamestate ( 12/27/2019 1:08pm )
    private void On_EnemySelect(EventParams _eventParams) {
        if (_eventParams.componentParams != null) {
            if (_eventParams.componentParams is EnemyEntity) {
                EnemyEntity _enemy = _eventParams.componentParams as EnemyEntity;

                _defendingEnemy = _enemy.enemyType;

                // Current state should be Bstate.player_ENEMYSELECTION ( 5/1/2020 1:27pm )
                FinishCurrentBState(Bstate.player_ENEMYSELECTION);
            }
            
        } else {
            Debug.LogError("EventParams with non-null componentParams expected.");
        }
        
    }

    // Fires when the gamestate has been changed ( 12/27/2019 1:14pm )
    #endregion

    private void PlayerAttackState() {
        if (_attackingItem is Potion) {
            // When the attacking item is a potion, the battle skips the enemy selection phase and goes straight to player_ATTACK ( 5/14/2020 2:03pm )
            Potion _activePotion = _attackingItem as Potion;

            EventManager.TriggerEvent("PlayerHeal", new EventParams(_activePotion.potionType.regen));
        } else {
            float _modifier;

            if (CheckVulnerabilities(_defendingEnemy, _attackingItem.itemType)) {
                // Add vulnerability to enemy inventory ( 4/24/2020 1:03am )
                EnemyInventory.instance.AddEnemyVul(_defendingEnemy, _attackingItem.itemType);

                _modifier = seModifier;
                print("It's Super Effective!");
            } else {
                
                if (_attackingItem is Weapon) {
                    _modifier = neModifier_Weapon;
                } else {
                    _modifier = neModifier;
                }
                
                print("Not Very Effective...");
            }

            EventManager.TriggerEvent("PlayerAttack", new EventParams(CalculateAttackDamage(_attackingItem.atk, _modifier, true, _rnd)));
        }      
    }
}
