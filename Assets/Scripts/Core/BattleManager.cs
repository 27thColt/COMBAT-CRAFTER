using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleStateMachine;
using static LevelStateMachine;
using static BattleLogic;

/*  12/27/2019 12:47pm - Battle Manager
 *  Handles game batle logic and all that shit
 *  reconciles the differences between man and AI
 * 
 * 
 *  5/1/2020 1:11pm - Some refactoring going to be done,, really want to centralize the battle code onto the battle manager

    7/30/2020 4:38pm - Did some more refactoring, decentralized the battle code onto separate battlestate classes. Decentralized while still centralized lmao
    Less of an eyesore on this file now.

    Now all this file is good for is holding the attackingItem and defendingItem values

    It will also initiate the battle once called from the LevelManager (that's probably... more important lmao)
 */
public class BattleManager : MonoBehaviour {
    #region Singleton

    private static BattleManager _battleManager;

    public static BattleManager instance {
        get {
            if (!_battleManager) {
                _battleManager = FindObjectOfType(typeof(BattleManager)) as BattleManager;

                if (!_battleManager) {
                    Debug.LogError("There needs to be one active WaveBattleManagerManager script on a GameObject in your scene.");
                }
            }

            return _battleManager;
        }
    }
    
    #endregion

    // Item used to attack (comes from resultItem in the Crafter) ( 12/27/2019 12:50pm )
    public Item attackingItem = null;
    public EnemyType defendingEnemy = null;

    // [SerializeField]
    // private PlayerEntity _playerObj;

    #region Awake
    void Awake() {
        EventManager.StartListening("LStateChange", On_LStateChange);
        EventManager.StartListening("BStateChange", On_BStateChange);
    }

    #endregion

    private void OnDestroy() {
        EventManager.StopListening("LStateChange", On_LStateChange);
        EventManager.StopListening("BStateChange", On_BStateChange);
    }

    private void Start() {
        // _playerObj = FindObjectOfType<PlayerEntity>();
    }

    #region Event Listeners

    private void On_LStateChange(EventParams eventParams) {
        if (currentLState is LevelBattle)
            SetCurrentBState(new LoadWave());
    }

    private void On_BStateChange(EventParams eventParams) {
        if (currentBState is null)
            currentLState.End(new EventParams(), "LevelBattle");
    }
    
    #endregion

    public void SetAttackingItem(Item item) {
        attackingItem = item;
    }

    public void SetDefendingEnemy(EnemyEntity enemy) {
        defendingEnemy = enemy.enemyType;
    }

    public void ResetValues() {
        attackingItem = null;
        defendingEnemy = null;
    }
}
