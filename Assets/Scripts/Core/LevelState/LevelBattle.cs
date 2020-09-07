using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    7/30/2020 4:56pm - Battle Level State
    Level State for when the player is in the middle of a battle
*/
public class LevelBattle : LevelState {

    public LevelBattle(LevelManager _levelManager = null) : base (_levelManager) {
        this._levelManager = _levelManager;
    }

    override public void Start(EventParams eventParams) {
        Debug.Log("LEVEL BATTLE STATE INITIALIZED");

        if (SceneManager.GetActiveScene().name != "BattleScene")
            SceneManager.LoadScene("BattleScene", LoadSceneMode.Single);
    }

    override public void End(EventParams eventParams, string stateName) {
        LevelStateMachine.SetCurrentLState(new LevelExplore(_levelManager), eventParams);
    }
}
