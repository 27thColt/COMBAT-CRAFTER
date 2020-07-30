using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    7/28/2020 3:55pm - Player Craft Battle State
    State in which the player crafts an item to use in battle
*/

public class PlayerCraft : BattleState {
    
    // Crafter.cs ( 7/28/2020 9:28pm )

    override public void Start(EventParams eventParams) {
        Debug.Log("PLAYER CRAFT STATE INITIALIZED");

        if (eventParams.stringParam1 == "UNCRAFT") {
            Crafter.instance.UncraftItems();
        }

        Crafter.instance.ResetCrafter();

        Crafter.instance.craftingEnabled = true;
    }

    override public void End(EventParams eventParams, string stateName) {
        if (eventParams.itemParam == null) { Debug.LogError("No ItemParam passed. Pausing at PlayerCraft State."); return;}

        Crafter.instance.craftingEnabled = false;

        BattleManager.instance.SetAttackingItem(eventParams.itemParam);

        Debug.Log("PlayerCraft: " + eventParams.itemParam.itemType.itemName);

        if (eventParams.itemParam is Potion) {
            BattleStateMachine.SetCurrentBState(new PlayerAttack(), eventParams);

        // In any other case
        } else {
            BattleStateMachine.SetCurrentBState(new EnemySelect(), eventParams);
        }
    }
}
