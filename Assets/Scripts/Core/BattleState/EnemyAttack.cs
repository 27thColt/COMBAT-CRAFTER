using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    7/28/2020 10:29pm - Enemy Attack Battle State
    ... for when the enemy... attacks you? (obvious iNNIT)
*/
public class EnemyAttack : BattleState {
    private System.Random _rnd = new System.Random();

    override public void Start(EventParams _eventParams) {
        Debug.Log("ENEMY ATTACK STATE INITIALIZED");

        EnemyAttackState();
    }

    override public void End(EventParams _eventParams, string stateName) {
        BattleManager.instance.ResetValues();
    

        BattleStateMachine.SetCurrentBState(new PlayerCraft());
    }

    private void EnemyAttackState() {
        int _count = WaveManager.instance.enemyList.Count;

        int i = _rnd.Next(0, _count);

        Debug.Log("PICKING ENEMY OF " + i);
        WaveManager.instance.enemyList[i].SetAttacking(true);
        EventManager.TriggerEvent("EnemyAttack", new EventParams(BattleLogic.CalculateAttackDamage(WaveManager.instance.enemyList[i].enemyType.baseAtk, 1, true, _rnd)));
    }
}
