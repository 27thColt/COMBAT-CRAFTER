using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    7/31/2020 1:15pm - Room Action Level State
    This might be a bit more complicated so this might take a bit of explaining,, but

    This level stated after the explore state (player can choose a room to explore) and will determine probabilities of events happening to the player

    This will include, encounters, random item pickups, or anything else
*/
public class RoomAction : LevelState {
    private System.Random _rnd = new System.Random();
    public RoomAction(LevelManager _levelManager = null) : base (_levelManager) {
        this._levelManager = _levelManager;
    }

    override public void Start(EventParams eventParams) {
        Debug.Log("ROOM ACTION STATE INITIALIZED");

        LevelStateMachine.currentLState.End(new EventParams(), "RoomAction");
    }

    override public void End(EventParams eventParams, string stateName) {
        int i = _rnd.Next(0, 101);

        if (i > 49) {
            LevelStateMachine.SetCurrentLState(new LevelBattle()); 
        } else {
            LevelStateMachine.SetCurrentLState(new LevelExplore());
        }
    }
}
