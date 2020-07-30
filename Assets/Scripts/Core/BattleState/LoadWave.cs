using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    7/28/2020 3:55pm - Load Wave Battle State
    Loads the next wave
*/
public class LoadWave : BattleState {

    // WaveManager.cs ( 7/28/2020 9:28pm )
    override public void Start(EventParams eventParams) {
        Debug.Log("LOAD WAVE STATE INITIALIZED");

        // Following code is in WaveManager.cs ( 7/28/2020 8:35pm )
        WaveManager.instance.LoadWave();
        

        End(new EventParams(), "LoadWave");
    }

    override public void End(EventParams eventParams, string stateName) {
        BattleStateMachine.SetCurrentBState(new PlayerCraft());
    }
}
