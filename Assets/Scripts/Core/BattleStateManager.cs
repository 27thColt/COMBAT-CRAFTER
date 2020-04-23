using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 12/27/2019 3:46pm - BattleStateClass (static class)
 * Not sure if this is right implementation so bear with me
 * 
 * Contains the last and current battlestate, as well as a function and event for when it is changed 
 */
public static class BattleStateManager {
    public static Battlestate lastState;
    public static Battlestate currentState;

    // Good Delegates and Events video -- https://www.youtube.com/watch?v=qwQ16sS8FSs ( 10/24/2019 9:34pm )
    public delegate void BattlestateChange(Battlestate _state);
    public static event BattlestateChange OnBattlestateChanged;

    public static void SetCurrentState(Battlestate _state) {
        lastState = currentState;
        currentState = _state;

        Debug.Log("LAST STATE: " + lastState + " | CURRENT STATE: " + currentState);

        OnBattlestateChanged?.Invoke(_state); // This is new so yeah-- "Null Conditional Operator ?." ( 12/28/2019 5:59pm )

        /* More on this operator. Given a function a?.x, the function will return a non-null value if a also returns a non-null value,
         * if null, then nothing will happen (will also return a null value)
         * 
         * Moreover, Invoke() is used to call a function (whichever precedes the the '.'
         * 
         * Meaning, OnBattlestateChanged will only fire if it is not equal to a null value (meaning there are event listeners present)
         *  
         *  ( 12/28/2019 6:06pm )
         */
    }
}

// The list of Battlestate that the game may choose from ( 10/24/2019 7:51pm )
// I just realzied I could've moved this out of the GSM class wtf I'm an idiot ( 12/27/2019 2:23pm )
[SerializeField]
public enum Battlestate {
    none,                   // 0
    game_LOADWAVE,          // 1
    player_CRAFT,           // 2
    player_ENEMYSELECTION,  // 3
    game_CALCULATE,         // 4
    enemy_ATTACK            // 5
}

/* DEPRECATED SHIT:
 * 
 * 10/24/2019 7:47pm - Game State Manager
 * I'm back :) Miss me? Okay, so after awhile of not working on this (most of 1st term, gr12), I am finally getting back to this.
 * 
 * Game State Manager (GSM, for short) handles all game state changes
 * 
 * Making an effort to better contain each component into themselves while keeping this a center for everything ( 12/27/2019 2:24pm )
 * 
 * 
 * So everything has been moved to a static class, leaving this part empty. ( 12/27/2019 4:00pm )
 */

