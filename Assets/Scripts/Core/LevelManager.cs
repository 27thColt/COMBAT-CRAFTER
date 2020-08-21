﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LevelStateMachine;

public class LevelManager : MonoBehaviour {
    #region Singleton

    private static LevelManager _levelManager;

    // Ensures that there is an EventManager in the scene ( 5/6/2020 7:51pm )
    public static LevelManager instance {
        get {
            if (!_levelManager) {
                _levelManager = FindObjectOfType(typeof(LevelManager)) as LevelManager;

                if (!_levelManager) {
                    Debug.LogError("There needs to be one active LevelManager script on a GameObject in your scene.");
                }
            }
            return _levelManager;
        }
    }

    #endregion

    [SerializeField] private bool _levelGenerated = false; // Will be assigned true once the level has been generated. Workaround for Start() and OnSceneLoaded not being consistent)
    public Room currentRoom;
    public Level currentLevel;

    // THE FOLLOWING CODE WILL INITIATE THE ENTIRE GAME LOOP PROPER ( 7/31/2020 1:18pm )
    void Awake() {
        DontDestroyOnLoad(gameObject);

        EventManager.StartListening("RoomSelect", On_RoomSelect);
        SceneManager.sceneLoaded += On_SceneLoaded; 
    }


    void OnDestroy() {
        EventManager.StopListening("RoomSelect", On_RoomSelect);
        SceneManager.sceneLoaded -= On_SceneLoaded;
    }

    #region Functions

    public void Init() {
        LoadLevel();
        
        currentRoom = currentLevel.ReturnStartRoom();
        
        currentRoom.known = true;
        currentLevel.rooms[currentRoom.position.x, currentRoom.position.y].known = true;
    }

    private void LoadLevel() {
        Debug.Log("Generating Rooms.");
        currentLevel = new Level(10, 10, 7);
        currentLevel.GenerateRooms();
    }

    #endregion

    #region Event Listeners

    private void On_SceneLoaded(Scene scene, LoadSceneMode sceneMode) {
        if (currentLevel == null) return;

        MapDrawer.instance.DrawMap(currentLevel);
        EventManager.TriggerEvent("SetCurrentRoom", new EventParams(currentRoom)); 
    }

    private void On_RoomSelect(EventParams eventParams) {
        if (eventParams.roomParam == null) { Debug.LogError("Event Params of non-null room param expected."); return; }

        if (eventParams.roomParam != currentRoom && currentRoom.IsAdjacentTo(eventParams.roomParam.position)) { 
            SetCurrentRoom(eventParams.roomParam);
            currentLState.End(new EventParams(), "LevelExplore");
        }
    }

    public void SetCurrentRoom(Room room) {
        currentRoom = room;
        
        currentRoom.known = true;
        currentLevel.rooms[currentRoom.position.x, currentRoom.position.y].known = true;
        MapDrawer.instance.DrawMap(currentLevel);   // Redraws the map ( 5/29/2020 2:03pm )

        EventManager.TriggerEvent("SetCurrentRoom", new EventParams(currentRoom));
    }

    #endregion
}
