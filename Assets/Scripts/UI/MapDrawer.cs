using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*  5/19/2020 9:53pm - Map Drawer Script
    Monobehaviour attached to the map gameobject. Reads info from the Level Generator and draws the map.

*/
public class MapDrawer : MonoBehaviour {
    #region Singleton

    private static MapDrawer _mapDrawer;

    public static MapDrawer instance {
        get {
            if (!_mapDrawer) {
                _mapDrawer = FindObjectOfType(typeof(MapDrawer)) as MapDrawer;

                if (!_mapDrawer) {
                    Debug.LogError("There needs to be one active Map Drawer script on a GameObject in your scene.");
                }
            }
            return _mapDrawer;
        }
    }
    
    #endregion

    [SerializeField]
    private GameObject _minimapTile = null; // Temporary (?) prefab to draw each cell of the map ( 5/19/2020 11:09pm )
    private Sprite[] _minimapTextures = null;   // Used to grab all of the sprites, though not use for referencing. (arrays aren't easily readible) ( 5/26/2020 9:00pm )
    public string tilemapFilename = "Minimap Tilemap";
    private Dictionary<string, Sprite> _tiles = null; // Will be used to reference all of the sprites ( 5/26/2020 9:00pm )


    // _selectedVector  holds the position of the selected room, _tileHighlight is the actual gameobject ( 5/29/2020 9:00pm )
    private Vector2Int _selectedVector;
    [SerializeField]
    private GameObject _tileHighlight = null;

    void Awake() {
        EventManager.StartListening("SetCurrentRoom", On_SetCurentRoom);
        LoadTilemap();
    }

    void OnDestroy() {
        EventManager.StopListening("SetCurrentRoom", On_SetCurentRoom);
    }

    // Loads all the sprites onto a dictionary for easy access ( 5/26/2020 9:18pm )
    private void LoadTilemap() {
        try {
            Debug.Log("Loading Tilemap sprites from " + tilemapFilename);
            _minimapTextures = Resources.LoadAll<Sprite>("Minimap Tiles/" + tilemapFilename);
        } catch {
            Debug.LogError("MapDrawer could not find designated file.");
            return;
        }

        _tiles = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in _minimapTextures) {
            string tag = sprite.name.Remove(0, tilemapFilename.Length + 1);
            _tiles.Add(tag, sprite);

        }

        /*  5/26/2020 9:17pm - minimap texture cheatsheet
            N - North   NE - Northeast
            E - East    NW - Northwest
            S - South   SE - Southeast
            W - West    SW - Southwest
            T Junctions (3 doors)
            TN - T junction facing north
            TE - T junction facing east
            TS - T junction facing south
            TW - T junction facing west
            X - Fourway intersection
            O - Isolated room with no doors
            null - empty room
        */
    }   

    // Returns a tiles position on the map with ( 5/29/2020 1:21pm )
    private Vector2 ReturnPositionOnMap(int x, int y, Level level) {
        // Sets some temporary component variables for ease ( 5/19/2020 11:32pm )
        RectTransform RT = GetComponent<RectTransform>();
        RectTransform parentRT = transform.parent.gameObject.GetComponent<RectTransform>();

        // Determines the size of each cell ( 5/19/2020 11:32pm )
        Vector2 cellSize = new Vector2(parentRT.sizeDelta.x / level.rooms.GetLength(0), parentRT.sizeDelta.x / level.rooms.GetLength(0));

        // Following code sets the position of the middle cell and recenters it to fit the middle of the map ( 5/26/2020 9:58pm )
        Vector2 middlePos = new Vector2(RT.sizeDelta.x / 2, -RT.sizeDelta.y / 2);

        Vector2 outputVector = new Vector2(middlePos.x - ((level.rooms.GetLength(0) / 2) - x) * cellSize.x, middlePos.y + ((level.rooms.GetLength(0) / 2) - y) * cellSize.y);
        return outputVector;
    }

    // Draws one single Room ( 6/3/2020 9:52pm )
    private GameObject DrawRoom(Vector2Int coords, Vector2 cellSize, Level level) {
        Vector2 cellPosition = ReturnPositionOnMap(coords.x, coords.y, level);
        
        GameObject room = Instantiate(_minimapTile);

        room.AddComponent<RoomSelect>();
        room.GetComponent<RoomSelect>().containedRoom = level.rooms[coords.x, coords.y];

        room.name = "Room "  + (new Vector2Int(coords.x, coords.y));
        room.transform.SetParent(transform);
        room.GetComponent<RectTransform>().sizeDelta = cellSize;
        
        if (level.rooms[coords.x, coords.y].type == RoomType.START) 
            room.GetComponent<RectTransform>().anchoredPosition = cellPosition;
        else
            room.GetComponent<RectTransform>().anchoredPosition = cellPosition;

        // Sets the sprite ( 5/26/2020 9:31pm )
        #region sprite stuff
        // 1 door sprites ( 5/26/2020 9:33pm )
        if (level.rooms[coords.x, coords.y].doors["N"] && !level.rooms[coords.x, coords.y].doors["E"] && !level.rooms[coords.x, coords.y].doors["S"] && !level.rooms[coords.x, coords.y].doors["W"]) {
            room.GetComponent<Image>().sprite = _tiles["N"];

        } else if (level.rooms[coords.x, coords.y].doors["E"] && !level.rooms[coords.x, coords.y].doors["N"] && !level.rooms[coords.x, coords.y].doors["S"] && !level.rooms[coords.x, coords.y].doors["W"]) {
            room.GetComponent<Image>().sprite = _tiles["E"];

        } else if (level.rooms[coords.x, coords.y].doors["S"] && !level.rooms[coords.x, coords.y].doors["N"] && !level.rooms[coords.x, coords.y].doors["E"] && !level.rooms[coords.x, coords.y].doors["W"]) {
            room.GetComponent<Image>().sprite = _tiles["S"];

        } else if (level.rooms[coords.x, coords.y].doors["W"] && !level.rooms[coords.x, coords.y].doors["N"] && !level.rooms[coords.x, coords.y].doors["E"] && !level.rooms[coords.x, coords.y].doors["S"]) {
            room.GetComponent<Image>().sprite = _tiles["W"];

        }
        
        // 2 Door sprites ( 5/26/2020 9:33pm )
        if (level.rooms[coords.x, coords.y].doors["N"]) {
            if (level.rooms[coords.x, coords.y].doors["E"] && !level.rooms[coords.x, coords.y].doors["S"] && !level.rooms[coords.x, coords.y].doors["W"]) {
                room.GetComponent<Image>().sprite = _tiles["NE"];

            } else if (level.rooms[coords.x, coords.y].doors["W"] && !level.rooms[coords.x, coords.y].doors["E"] && !level.rooms[coords.x, coords.y].doors["S"]) {
                room.GetComponent<Image>().sprite = _tiles["NW"];

            }

        } else if (level.rooms[coords.x, coords.y].doors["S"]) {
            if (level.rooms[coords.x, coords.y].doors["E"] && !level.rooms[coords.x, coords.y].doors["N"] && !level.rooms[coords.x, coords.y].doors["W"]) {
                room.GetComponent<Image>().sprite = _tiles["SE"];

            } else if (level.rooms[coords.x, coords.y].doors["W"] && !level.rooms[coords.x, coords.y].doors["N"] && !level.rooms[coords.x, coords.y].doors["E"]) {
                room.GetComponent<Image>().sprite = _tiles["SW"];
            }
        }

        // 3 Door sprites ( 5/26/2020 9:33pm )
        if (level.rooms[coords.x, coords.y].doors["N"] && level.rooms[coords.x, coords.y].doors["E"] && level.rooms[coords.x, coords.y].doors["W"] && !level.rooms[coords.x, coords.y].doors["S"]) {
            room.GetComponent<Image>().sprite = _tiles["TN"];

        } else if (level.rooms[coords.x, coords.y].doors["E"] && level.rooms[coords.x, coords.y].doors["N"] && level.rooms[coords.x, coords.y].doors["S"] && !level.rooms[coords.x, coords.y].doors["W"]) {
            room.GetComponent<Image>().sprite = _tiles["TE"];

        } else if (level.rooms[coords.x, coords.y].doors["S"] && level.rooms[coords.x, coords.y].doors["E"] && level.rooms[coords.x, coords.y].doors["W"] && !level.rooms[coords.x, coords.y].doors["N"]) {
            room.GetComponent<Image>().sprite = _tiles["TS"];

        } else if (level.rooms[coords.x, coords.y].doors["W"] && level.rooms[coords.x, coords.y].doors["N"] && level.rooms[coords.x, coords.y].doors["S"] && !level.rooms[coords.x, coords.y].doors["E"]) {
            room.GetComponent<Image>().sprite = _tiles["TW"];

        }

        // Other cases ( 5/26/2020 9:38pm )
        if (level.rooms[coords.x, coords.y].doors["N"] && level.rooms[coords.x, coords.y].doors["E"] && level.rooms[coords.x, coords.y].doors["S"] && level.rooms[coords.x, coords.y].doors["W"]) {
            room.GetComponent<Image>().sprite = _tiles["X"];

        } else if (!level.rooms[coords.x, coords.y].doors["N"] && !level.rooms[coords.x, coords.y].doors["E"] && !level.rooms[coords.x, coords.y].doors["S"] && !level.rooms[coords.x, coords.y].doors["W"]) {
            room.GetComponent<Image>().sprite = _tiles["O"];
        }

        #endregion
    
        return room;
    }

    // Draws the map taken straight from the LevelGenerator script ( 5/19/2020 11:31pm )
    public void DrawMap(Level level) {
        // Prepared with some error messages ( 5/26/2020 8:53pm )
        if (level.rooms == null) {
            Debug.LogError("Map has not been generated. Cannot draw map.");
            return;
        }

        RectTransform RT = GetComponent<RectTransform>();
        RectTransform parentRT = transform.parent.gameObject.GetComponent<RectTransform>();

        DeleteMap();

        RT.sizeDelta = new Vector2(parentRT.sizeDelta.x * 2, parentRT.sizeDelta.y * 2);

        // Sets the size for the tile highlight ( 5/29/2020 1:06pm )
        _tileHighlight.GetComponent<RectTransform>().sizeDelta = new Vector2(parentRT.sizeDelta.x / level.rooms.GetLength(0), parentRT.sizeDelta.x / level.rooms.GetLength(0));

        // Determines the size of each cell ( 5/19/2020 11:32pm )
        Vector2 cellSize = new Vector2(parentRT.sizeDelta.x / level.rooms.GetLength(0), parentRT.sizeDelta.x / level.rooms.GetLength(0));

        for (int i = 0; i < level.rooms.GetLength(0); i++) {
            for (int j = 0; j < level.rooms.GetLength(1); j++) {
                if (level.rooms[i, j] != null && level.rooms[i, j].known) {
                    DrawRoom(new Vector2Int(i, j), cellSize, level);

                    List<Room> adjacentRooms = level.ReturnAdjacentRooms(new Vector2Int(i, j));

                    foreach (Room room in adjacentRooms) {  
                        if (!room.known) {
                            var cell = DrawRoom(room.position, cellSize, level);
                            cell.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                        }
                    }
                }
            }
        }

        _tileHighlight.transform.SetAsLastSibling();
    }

    private void DeleteMap() {
        foreach(Transform child in transform) {
            if (child.gameObject != _tileHighlight) {
                Destroy(child.gameObject);
            }
            
        }
    }

    private void On_SetCurentRoom(EventParams eventParams) {
        if (eventParams.roomParam != null) {

            _selectedVector = eventParams.roomParam.position;
            _tileHighlight.GetComponent<RectTransform>().anchoredPosition = ReturnPositionOnMap(_selectedVector.x, _selectedVector.y, LevelManager.instance.currentLevel);
        } else {
            Debug.LogError("Eventparams with non-null room param expected.");
        }
    }

}
