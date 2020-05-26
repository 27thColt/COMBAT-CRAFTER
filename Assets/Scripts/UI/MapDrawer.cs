using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*  5/19/2020 9:53pm - Map Drawer Script
    Monobehaviour attached to the map gameobject. Reads info from the Level Generator and draws the map.

*/
public class MapDrawer : MonoBehaviour {
    [SerializeField]
    private GameObject _minimapTile = null; // Temporary (?) prefab to draw each cell of the map ( 5/19/2020 11:09pm )

    [SerializeField]
    private Dictionary<string, Texture2D> _minimapTextures = null;

    void Start() {
        LevelGenerator.instance.GenerateRooms();
        
        // foreach(Room _room in LevelGenerator.instance.map) {
        //     if (_room != null) Debug.Log(_room.position + " " + _room.type);
        // }

        DrawMap();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            LevelGenerator.instance.GenerateRooms();
            DrawMap();
        }
    }

    // Draws the map taken straight from the LevelGenerator script ( 5/19/2020 11:31pm )
    private void DrawMap() {
        if (LevelGenerator.instance.map == null) return;

        DeleteMap();

        // Sets some temporary component variables for ease ( 5/19/2020 11:32pm )
        LevelGenerator LG = LevelGenerator.instance;
        RectTransform RT = GetComponent<RectTransform>();

        // Determines the size of each cell ( 5/19/2020 11:32pm )
        Vector2 cellSize = new Vector2(RT.sizeDelta.x / LG.map.GetLength(0), RT.sizeDelta.x / LG.map.GetLength(0));

        for (int i = 0; i < LG.map.GetLength(0); i++) {
            for (int j = 0; j < LG.map.GetLength(1); j++) {
                if (LG.map[i, j] != null) {
                    GameObject room = Instantiate(_minimapTile);
                    room.transform.SetParent(transform);
                    room.GetComponent<RectTransform>().sizeDelta = cellSize;
                    room.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.FloorToInt(i * cellSize.x + (cellSize.x / 2)), Mathf.FloorToInt(-(j * cellSize.x + (cellSize.x / 2)))); // Sets the position ( 5/19/2020 11:32pm )
                }

            }
        }

        // for (int i = 0; i < LG.map.GetLength(0); i++) {
        //     for (int j = 0; j < LG.map.GetLength(1); j++) {
        //         GameObject _cell = Instantiate(_panelObject);
        //         _cell.transform.SetParent(transform);
        //         _cell.GetComponent<RectTransform>().sizeDelta = cellSize;
        //         _cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * cellSize.x + (cellSize.x / 2), -(j * cellSize.y + (cellSize.y / 2))); // Sets the position ( 5/19/2020 11:32pm )

        //         // If the cell drawn contains a room ( 5/20/2020 10:36am )
        //         if (LG.map[i, j] != null) {
        //             GameObject _room = Instantiate(_panelObject);
        //             _room.name = "Room";
        //             _room.transform.SetParent(_cell.transform);
        //             _room.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.x / 1.4f, cellSize.y / 1.4f);
        //             _room.GetComponent<RectTransform>().anchoredPosition = new Vector2(cellSize.x / 2, -cellSize.y / 2); // Sets the position ( 5/19/2020 11:32pm )

        //             if (LG.map[i,j].type == RoomType.START) {
        //                 _room.GetComponent<Image>().color = Color.red;
        //             } else if (LG.map[i, j].type == RoomType.DEFAULT) {
        //                 _room.GetComponent<Image>().color = Color.yellow;
        //             } else if (LG.map[i, j].type == RoomType.TREASURE) {
        //                 _room.GetComponent<Image>().color = Color.cyan;
        //             } else if (LG.map[i, j].type == RoomType.EXIT) {
        //                 _room.GetComponent<Image>().color = Color.magenta;
        //             }

        //             // Draws the doors on each cell ( 5/20/2020 10:32am )
        //             for (int k = 0; k < LG.map[i, j].doors.Count; k++) {
        //                 if (LG.map[i, j].doors[k]) {
        //                     GameObject _door = Instantiate(_panelObject);
        //                     _door.transform.SetParent(_cell.transform);
        //                     _door.GetComponent<Image>().color = Color.yellow;

        //                     int _doorRatio = 4;
        //                     switch (k) {
        //                         case 0: // North ( 5/20/2020 10:32am )
                                    
        //                             _door.name = "North Door";
        //                             //_door.GetComponent<Image>().color = Color.magenta;
        //                             _door.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.x / _doorRatio, cellSize.y / _doorRatio);
        //                             _door.GetComponent<RectTransform>().anchoredPosition = new Vector2(cellSize.x / 2, -cellSize.y / _doorRatio / 2);  
        //                             break;
        //                         case 1: // East ( 5/20/2020 10:32am )
        //                             _door.name = "East Door";
        //                             float fuck = (cellSize.x / 2) + (cellSize.x / 2.5f);
        //                             //_door.GetComponent<Image>().color = Color.grey;
        //                             _door.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.y / (_doorRatio * 0.7f), cellSize.x / _doorRatio);
        //                             _door.GetComponent<RectTransform>().anchoredPosition = new Vector2((cellSize.x / 2) + (cellSize.x / 2.5f), -cellSize.y / 2);
        //                             break;
        //                         case 2: // South ( 5/20/2020 10:32am )
        //                             _door.name = "South Door";
        //                             //_door.GetComponent<Image>().color = Color.cyan;
        //                             _door.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.x / _doorRatio, cellSize.y / _doorRatio);
        //                             _door.GetComponent<RectTransform>().anchoredPosition = new Vector2(cellSize.x / 2, -cellSize.y + ( cellSize.y / _doorRatio / 2));
        //                             break;  
        //                         case 3: // West ( 5/20/2020 10:32am )
        //                             _door.name = "West Door";
        //                             //_door.GetComponent<Image>().color = Color.white;
        //                             _door.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.y / (_doorRatio * 0.6f), cellSize.x / _doorRatio);
        //                             _door.GetComponent<RectTransform>().anchoredPosition = new Vector2((cellSize.x / 2) - (cellSize.x / 2.5f), -cellSize.y / 2);
        //                             break;
        //                         default:
        //                             break;
        //                     }
        //                 }
        //             }
        //         }
        //     }
        // }
    }

    private void DeleteMap() {
        foreach(Transform _child in transform) {
            Destroy(_child.gameObject);
        }
    }
}
