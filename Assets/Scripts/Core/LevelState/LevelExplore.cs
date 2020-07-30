using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    7/30/2020 4:56pm - Explore Level State
    Level State for when the player is exploring the level
*/
public class LevelExplore : LevelState {
    override public void Start(EventParams eventParams) {
        Debug.Log("LEVEL EXPLORE STATE INITIALIZED");
    }

    override public void End(EventParams eventParams, string stateName) {
        LevelStateMachine.SetCurrentLState(new LevelBattle(), eventParams);
    }
}
     