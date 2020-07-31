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
    // Item used to attack (comes from resultItem in the Crafter) ( 12/27/2019 12:50pm )
    public Item attackingItem = null;
    public EnemyType defendingEnemy = null;

    // Referenced values in editor.
    [SerializeField]
    private WaveManager _waveManager = null;
    [SerializeField]
    private Crafter _crafter = null;

    #region Awake
    void Awake() {
        EventManager.StartListening("LStateChange", On_LStateChange);
        EventManager.StartListening("BStateChange", On_BStateChange);
        EventManager.StartListening("CancelCraft", On_CancelCraft);
    }

    #endregion

    private void OnDestroy() {
        EventManager.StopListening("LStateChange", On_LStateChange);
        EventManager.StopListening("BStateChange", On_BStateChange);
        EventManager.StopListening("CancelCraft", On_CancelCraft);
    }

    private void Start() {

    }

    #region Functions

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

    #endregion
    
    #region Event Listeners

    private void On_LStateChange(EventParams eventParams) {
        if (currentLState is LevelBattle)
            SetCurrentBState(new LoadWave(GetComponent<BattleManager>(), _waveManager, _crafter));
    }

    private void On_BStateChange(EventParams eventParams) {
        if (currentBState is null)
            currentLState.End(new EventParams(), "LevelBattle");
    }

    private void On_CancelCraft(EventParams eventParams) {
        BattleStateMachine.SetCurrentBState(new PlayerCraft(GetComponent<BattleManager>(), _waveManager, _crafter), new EventParams("UNCRAFT"));
    }
    
    #endregion
}
