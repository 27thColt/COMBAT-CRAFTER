using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelState;

public class LevelManager : MonoBehaviour {

    [SerializeField]
    private PlayerEntity _playerObj;
    public Room currentRoom;

    void Awake() {
        EventManager.StartListening("LStateFinish", On_LStateFinish);
    }

    void OnDestroy() {
        EventManager.StopListening("LStateFinish", On_LStateFinish);
    }

    void Start() {
        _playerObj = FindObjectOfType<PlayerEntity>();

        LoadLevel();
        SetCurrentLState(Lstate.BATTLE);
    }

    private void LoadLevel() {
        LevelGenerator.instance.GenerateRooms();
        MapDrawer.instance.DrawMap();

        Room start = LevelGenerator.instance.ReturnStartRoom();
        SetCurrentRoom(start);
    }

    private void On_LStateFinish(EventParams eventParams) {
        switch (eventParams.lstateParam) {
            case Lstate.BATTLE:
                SetCurrentLState(Lstate.EXPLORE);
                break;

            case Lstate.EXPLORE:
                SetCurrentLState(Lstate.BATTLE);    // Calls BattleManager.cs ( 5/29/2020 1:29pm )
                break;

            default:  
                break;
        }
    }

    public void SetCurrentRoom(Room room) {
        currentRoom = room;
        EventManager.TriggerEvent("SetCurrentRoom", new EventParams(currentRoom));
    }
}
