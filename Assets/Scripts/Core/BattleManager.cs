using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleState;
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
    private PlayerObject _playerObj;

    /* A random object must be created because if new random objects are generated within too close of a time, the numbers may end up become the same ( 4/27/2020 2:51pm )
     * System.Random utilizes the system time in order to generate random numbers
     */ 
    private System.Random _rnd = new System.Random();

    #region Awake
    void Awake() {
        EventManager.StartListening("BStateFinish", On_BstateFinish);
        EventManager.StartListening("BStateChange", On_BStateChange);
        EventManager.StartListening("ItemCraft", On_ItemCraft);
        EventManager.StartListening("EnemySelect", On_EnemySelect);
    }

    #endregion

    private void OnDestroy() {
        EventManager.StopListening("BStateFinish", On_BstateFinish);
        EventManager.StopListening("BStateChange", On_BStateChange);
        EventManager.StopListening("ItemCraft", On_ItemCraft);
        EventManager.StopListening("EnemySelect", On_EnemySelect);
    }

    private void Start() {
        _playerObj = FindObjectOfType<PlayerObject>();

        // Calls WaveManager.cs ( 5/1/2020 1:11pm )
        SetCurrentState(Bstate.game_LOADWAVE);
    }

    #region Event Listeners

    // Dictionary Tag "BStateFinish" ( 5/6/2020 11:46pm )
    private void On_BstateFinish(EventParams _eventParams) {
        switch (_eventParams.bstateParam) {
            case Bstate.game_LOADWAVE:
                SetCurrentState(Bstate.player_CRAFT);
                break;

            case Bstate.player_CRAFT:
                SetCurrentState(Bstate.player_ENEMYSELECTION);
                if (_attackingItem is Weapon) {
                    
                }
                break;

            case Bstate.player_ENEMYSELECTION:
                SetCurrentState(Bstate.player_ATTACK);
                break;

            case Bstate.player_ATTACK:
                if (WaveManager.instance.enemyList.Count > 0) {
                    SetCurrentState(Bstate.enemy_ATTACK);   // BattleManager.cs ( 5/1/2020 1:46pm )

                } else {
                    // Fires if all enemies in the wavee have been defeated ( 5/1/2020 5:29pm )
                    Debug.Break();
                }
                break;

            case Bstate.enemy_ATTACK:
                SetCurrentState(Bstate.game_ROUNDRESET);
                break;

            case Bstate.game_ROUNDRESET:
                SetCurrentState(Bstate.player_CRAFT);
                break;

            default:
                Debug.LogError("EventParams with non-default bstateParam expected.");
                break;
        }
    }

    // Updates the Attacking Item when it is crafted ( 5/7/2020 11:35pm )
    public void On_ItemCraft(EventParams _eventParams) {
        if (_eventParams.itemParam != null)
            _attackingItem = _eventParams.itemParam;
        else {
            Debug.LogError("EventParams with non-null itemParam expected.");
        }
    }

    // Will update the defending enemy once the enemy has been selected. Will also change the gamestate ( 12/27/2019 1:08pm )
    public void On_EnemySelect(EventParams _eventParams) {
        if (_eventParams.componentParams != null) {
            if (_eventParams.componentParams is EnemyObject) {
                EnemyObject _enemy = _eventParams.componentParams as EnemyObject;

                _defendingEnemy = _enemy.enemyType;

                // Current state should be Bstate.player_ENEMYSELECTION ( 5/1/2020 1:27pm )
                FinishCurrentState(Bstate.player_ENEMYSELECTION);
            }
            
        } else {
            Debug.LogError("EventParams with non-null componentParams expected.");
        }
        
    }

    // Fires when the gamestate has been changed ( 12/27/2019 1:14pm )
    public void On_BStateChange(EventParams _eventParams) {
        if (_eventParams.bstateParam != Bstate.none) {
            Bstate _state = _eventParams.bstateParam;

            if (_state == Bstate.player_ATTACK) {
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
            
            // When the it is time for the enemies to attack the player ( 4/27/2020 2:29pm )
            }  else if (_state == Bstate.enemy_ATTACK) {
                int _count = WaveManager.instance.enemyList.Count;

                int i = _rnd.Next(0, _count);

                WaveManager.instance.enemyList[i].SetAttacking(true);
                EventManager.TriggerEvent("EnemyAttack", new EventParams(CalculateAttackDamage(WaveManager.instance.enemyList[i].enemyType.baseAtk, 1, true, _rnd)));

            // Reset all values ( 4/27/2020 3:07pm )
            } else if (_state == Bstate.game_ROUNDRESET) {
                _defendingEnemy = null;
                _attackingItem = null;

                FinishCurrentState(Bstate.game_ROUNDRESET);
                
            }
        } else {
            Debug.LogError("EventParams with non-default bstateParam expected.");
        }
        
        
    }

    #endregion

}
