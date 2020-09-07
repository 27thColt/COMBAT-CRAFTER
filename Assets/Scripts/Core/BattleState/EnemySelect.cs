using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    7/28/2020 9:19pm - Enemy Select Battle State
    When the it is time for the player to select an enemy

*/

public class EnemySelect : BattleState {
    public EnemySelect(BattleManager battleManager, WaveManager waveManager, Crafter crafter) : base (battleManager, waveManager, crafter) {
        if (_battleManager == null) Debug.LogError("Battle Manager object cannot be found.");
    }

    override public void Start(EventParams eventParams) {
        Debug.Log("ENEMY SELECT STATE INITIALIZED");
    }

    override public void End(EventParams eventParams, string stateName) {
        EnemyEntity enemy = eventParams.componentParams as EnemyEntity;
        _battleManager.SetDefendingEnemy(enemy);
        
        EventManager.TriggerMessage(Message.ItemCraft(_battleManager.attackingItem.itemType.itemName));

        BattleStateMachine.SetCurrentBState(new PlayerAttack(_battleManager, _waveManager, _crafter), new EventParams() {
            itemParam = _battleManager.attackingItem,
            enemyTypeParam1 = enemy.enemyType
        });
    }
}   
