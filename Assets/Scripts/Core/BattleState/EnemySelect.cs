using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    7/28/2020 9:19pm - Enemy Select Battle State
    When the it is time for the player to select an enemy

*/

public class EnemySelect : BattleState {
    override public void Start(EventParams eventParams) {
        Debug.Log("ENEMY SELECT STATE INITIALIZED");
    }

    override public void End(EventParams eventParams, string stateName) {
        EnemyEntity enemy = eventParams.componentParams as EnemyEntity;
        BattleManager.instance.SetDefendingEnemy(enemy);
        
        BattleStateMachine.SetCurrentBState(new PlayerAttack(), new EventParams() {
            itemParam = BattleManager.instance.attackingItem,
            enemyTypeParam1 = enemy.enemyType
        });
    }
}   
