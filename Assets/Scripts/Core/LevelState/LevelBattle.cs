using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    7/30/2020 4:56pm - Battle Level State
    Level State for when the player is in the middle of a battle
*/
public class LevelBattle : LevelState {
    override public void Start(EventParams eventParams) {
        Debug.Log("LEVEL BATTLE STATE INITIALIZED");
    }

    override public void End(EventParams eventParams, string stateName) {
        LevelStateMachine.SetCurrentLState(new LevelExplore(), eventParams);
    }
}
