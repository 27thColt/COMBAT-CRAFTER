using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 12/27/2019 3:46pm - BattleStateClass (static class)
 * Not sure if this is right implementation so bear with me
 * 
 * Contains the last and current battlestate, as well as a function and event for when it is changed 
 */
public static class BattleState {
    public static Bstate lastBState;
    public static Bstate currentBState;

    public static void SetCurrentBState(Bstate state) {
        lastBState = currentBState;
        currentBState = state;

        Debug.Log("LAST BATTLE STATE: " + lastBState + " | CURRENT BATTLE STATE: " + currentBState);


        EventManager.TriggerEvent("BStateChange", new EventParams(state));
    }

    // Called when a Bstate is finished ( 5/1/2020 1:17pm )
    public static void FinishCurrentBState(Bstate state) {
        EventManager.TriggerEvent("BStateFinish", new EventParams(state));
    }
}

// The list of Battlestate that the game may choose from ( 10/24/2019 7:51pm )
// I just realzied I could've moved this out of the GSM class wtf I'm an idiot ( 12/27/2019 2:23pm )
[SerializeField]
public enum Bstate {
    none,                   // 0
    game_LOADWAVE,          // 1
    player_CRAFT,           // 2
    player_ENEMYSELECTION,  // 3
    player_ATTACK,          // 4
    enemy_ATTACK,           // 6
    game_ROUNDRESET         // 7
}