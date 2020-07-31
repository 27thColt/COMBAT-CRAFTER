using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    7/28/2020 10:29pm - Enemy Attack Battle State
    ... for when the enemy... attacks you? (obvious iNNIT)
*/
public class EnemyAttack : BattleState {
    private System.Random _rnd = new System.Random();

    public EnemyAttack(BattleManager battleManager, WaveManager waveManager, Crafter crafter) : base (battleManager, waveManager, crafter) {
        if (_waveManager == null) Debug.LogError("Wave Manager object cannot be found.");
        if (_battleManager == null) Debug.LogError("Battle Manager object cannot be found.");
    }

    override public void Start(EventParams _eventParams) {
        Debug.Log("ENEMY ATTACK STATE INITIALIZED");

        EnemyAttackState();
    }

    override public void End(EventParams _eventParams, string stateName) {
        _battleManager.ResetValues();
    

        BattleStateMachine.SetCurrentBState(new PlayerCraft(_battleManager, _waveManager, _crafter));
    }

    private void EnemyAttackState() {
        int _count = _waveManager.enemyList.Count;

        int i = _rnd.Next(0, _count);

        Debug.Log("PICKING ENEMY OF " + i);
        _waveManager.enemyList[i].SetAttacking(true);
        EventManager.TriggerEvent("EnemyAttack", new EventParams(BattleLogic.CalculateAttackDamage(_waveManager.enemyList[i].enemyType.baseAtk, 1, true, _rnd)));
    }
}
