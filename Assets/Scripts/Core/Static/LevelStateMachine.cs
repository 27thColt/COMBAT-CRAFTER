using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelStateMachine {
    public static LevelState currentLState;

    public static void SetCurrentLState(LevelState state, EventParams eventParams = null) {
        currentLState = state;

        eventParams = (eventParams != null) ? eventParams : new EventParams();

        currentLState?.Start(eventParams);

        EventManager.TriggerEvent("LStateChange", eventParams);
    }
}

[SerializeField]
public enum Lstate {
    none,
    BATTLE,
    EXPLORE
}
