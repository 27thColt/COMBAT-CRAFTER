using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


    [SerializeField]
    private PlayerEntity _playerObj;
    public Room currentRoom;
    public Level currentLevel;

    void Awake() {
        EventManager.StartListening("RoomSelect", On_RoomSelect);
    }

    void OnDestroy() {
        EventManager.StopListening("RoomSelect", On_RoomSelect);
    }

    void Start() {
        _playerObj = FindObjectOfType<PlayerEntity>();
        

        LoadLevel();
        SetCurrentLState(new LevelExplore());
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            currentLState.End(new EventParams(), "Explore or Battle");
        }
    }

    private void LoadLevel() {
        currentLevel = new Level(10, 10, 7);
        currentLevel.GenerateRooms();

        MapDrawer.instance.DrawMap(currentLevel);

        SetCurrentRoom(currentLevel.ReturnStartRoom());
    }

    #region Event Listeners

    private void On_RoomSelect(EventParams eventParams) {
        if (eventParams.roomParam != null) {
            if (eventParams.roomParam != currentRoom && currentRoom.IsAdjacentTo(eventParams.roomParam.position)) { 
                SetCurrentRoom(eventParams.roomParam);
            }

        } else {
            Debug.LogError("Event Params of non-null room param expected.");
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
