using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    7/30/2020 4:56pm - Explore Level State
    Level State for when the player is exploring the level
*/
public class LevelExplore : LevelState {

    public LevelExplore(LevelManager _levelManager = null) : base (_levelManager) {
        this._levelManager = _levelManager;
    }

    override public void Start(EventParams eventParams) {
        Debug.Log("LEVEL EXPLORE STATE INITIALIZED");

        if (SceneManager.GetActiveScene().name != "DungeonScene") 
            SceneManager.LoadScene("DungeonScene", LoadSceneMode.Single);

    }

    override public void End(EventParams eventParams, string stateName) {
        LevelStateMachine.SetCurrentLState(new RoomAction(), eventParams);
    }
}
     