using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    7/30/2020 4:48pm - Level State abstract class
    Follows the same design pattern mentioned in BattleState.cs

    Each level state is now it's own class, inherited from this one.
    And with that, all of the primary logic needed for each class will be contained in them
*/
[System.Serializable]
public abstract class LevelState {
    protected LevelManager _levelManager;

    public LevelState(LevelManager _levelManager = null) {
        this._levelManager = _levelManager;
    }

    virtual public void Start(EventParams eventParams) {

    }

    virtual public void End(EventParams eventParams, string stateName) {
        // Second argument is used for my personal identification ( 7/30/2020 4:50pm )
    }

}
