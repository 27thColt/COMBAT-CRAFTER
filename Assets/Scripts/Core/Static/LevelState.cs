using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelState {
    public static Lstate lastLState;
    public static Lstate currentLstate;

    public static void SetCurrentLState(Lstate state) {
        lastLState = currentLstate;
        currentLstate = state;

        Debug.Log("===== | LAST LEVEL STATE: " + lastLState + " | CURRENT LEVEL STATE: " + currentLstate + " | ====");


        EventManager.TriggerEvent("LStateChange", new EventParams(state));
    }

    // Called when a Bstate is finished ( 5/1/2020 1:17pm )
    public static void FinishCurrentLState(Lstate state) {
        EventManager.TriggerEvent("LStateFinish", new EventParams(state));
    }
}

[SerializeField]
public enum Lstate {
    none,
    BATTLE,
    EXPLORE
}
