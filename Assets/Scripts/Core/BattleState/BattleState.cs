using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    7/28/2020 2:18pm - Battle State abstract class
    Part of new FSM design,, refactoring everything to follow through lmao (hoping this goes over well)

    Class states will inherit this class. Each class will define it's own state's functionality
*/
[System.Serializable]
public abstract class BattleState {
    protected BattleManager _battleManager = null;
    protected WaveManager _waveManager = null;
    protected Crafter _crafter = null;

    public BattleState(BattleManager battleManager, WaveManager waveManager, Crafter crafter) {
        _battleManager = battleManager;
        _waveManager = waveManager;
        _crafter = crafter;

    }
    // Code for when the state starts ( 7/28/2020 2:18pm )
    virtual public void Start(EventParams eventParams) {

    }

    // Code for when the state ends ( 7/28/2020 2:18pm )
    virtual public void End(EventParams eventParams, string stateName) {
        // The second argument isn't actually needed, but I keep it there so state Ends can be easily identified. ( 7/29/2020 1:20pm )

        // It might be a horrible fix but I'm keeping it there for now
    }

}
