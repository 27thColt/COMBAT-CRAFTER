using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    7/28/2020 3:55pm - Player Craft Battle State
    State in which the player crafts an item to use in battle
*/

public class PlayerCraft : BattleState {
    public PlayerCraft(BattleManager battleManager, WaveManager waveManager, Crafter crafter) : base (battleManager, waveManager, crafter) {
        if (_crafter == null) Debug.LogError("Crafter object cannot be found.");
        if (_battleManager == null) Debug.LogError("Battle Manager object cannot be found.");
    }

    override public void Start(EventParams eventParams) { 
        Debug.Log("PLAYER CRAFT STATE INITIALIZED");

        if (_crafter == null) { Debug.Log("Crafter Gameobject cannot be found. Stoppping at PlayerCraft state."); return; }

        if (eventParams.stringParam1 == "UNCRAFT") {
            _crafter.UncraftItems();
        }

        _crafter.ResetCrafter();

        _crafter.craftingEnabled = true;
    }

    override public void End(EventParams eventParams, string stateName) {
        if (eventParams.itemParam == null) { Debug.LogError("No ItemParam passed. Pausing at PlayerCraft State."); return;}

        _crafter.craftingEnabled = false;

        _battleManager.SetAttackingItem(eventParams.itemParam);

        Debug.Log("PlayerCraft: " + eventParams.itemParam.itemType.itemName);

        if (eventParams.itemParam is Potion) {
            BattleStateMachine.SetCurrentBState(new PlayerAttack(_battleManager, _waveManager, _crafter), eventParams);

        // In any other case
        } else {
            BattleStateMachine.SetCurrentBState(new EnemySelect(_battleManager, _waveManager, _crafter), eventParams);
        }
    }
}
