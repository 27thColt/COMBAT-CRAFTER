using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 12/27/2019 3:46pm - BattleStateClass (static class)
 * Not sure if this is right implementation so bear with me
 * 
 * 7/30/2020 4:53pm - Deleted the enums that used to be here as part of refactoring
 */
public static class BattleStateMachine {
    public static BattleState currentBState;

    public static void SetCurrentBState(BattleState state, EventParams eventParams = null) {
        currentBState = state;

        currentBState?.Start(eventParams != null ? eventParams : new EventParams());

        EventManager.TriggerEvent("BStateChange", eventParams != null ? eventParams : new EventParams());
    }
}