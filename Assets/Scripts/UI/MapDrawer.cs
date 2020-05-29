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
    private Vector2 ReturnPositionOnMap(int x, int y) {
        // Sets some temporary component variables for ease ( 5/19/2020 11:32pm )
        LevelGenerator LG = LevelGenerator.instance;
        RectTransform RT = GetComponent<RectTransform>();
        RectTransform parentRT = transform.parent.gameObject.GetComponent<RectTransform>();

        // Determines the size of each cell ( 5/19/2020 11:32pm )
        Vector2 cellSize = new Vector2(parentRT.sizeDelta.x / LG.map.GetLength(0), parentRT.sizeDelta.x / LG.map.GetLength(0));

        // Following code sets the position of the middle cell and recenters it to fit the middle of the map ( 5/26/2020 9:58pm )
        Vector2 middlePos = new Vector2(RT.sizeDelta.x / 2, -RT.sizeDelta.y / 2);

        Vector2 outputVector = new Vector2(middlePos.x - ((LG.map.GetLength(0) / 2) - x) * cellSize.x, middlePos.y + ((LG.map.GetLength(0) / 2) - y) * cellSize.y);
        return outputVector;
    }

    // Draws the map taken straight from the LevelGenerator script ( 5/19/2020 11:31pm )
    public void DrawMap() {
        // Prepared with some error messages ( 5/26/2020 8:53pm )
        if (LevelGenerator.instance.map == null) {
            Debug.LogError("Map has not been generated. Cannot draw map.");
            return;
        }

        // Sets some temporary component variables for ease ( 5/19/2020 11:32pm )
        LevelGenerator LG = LevelGenerator.instance;
        RectTransform RT = GetComponent<RectTransform>();
        RectTransform parentRT = transform.parent.gameObject.GetComponent<RectTransform>();

        LoadTilemap();
        DeleteMap();

        RT.sizeDelta = new Vector2(parentRT.sizeDelta.x * 2, parentRT.sizeDelta.y * 2);

        // Sets the size for the tile highlight ( 5/29/2020 1:06pm )
        _tileHighlight.GetComponent<RectTransform>().sizeDelta = new Vector2(parentRT.sizeDelta.x / LevelGenerator.instance.map.GetLength(0), parentRT.sizeDelta.x / LevelGenerator.instance.map.GetLength(0));

        // Determines the size of each cell ( 5/19/2020 11:32pm )
        Vector2 cellSize = new Vector2(parentRT.sizeDelta.x / LG.map.GetLength(0), parentRT.sizeDelta.x / LG.map.GetLength(0));

        for (int i = 0; i < LG.map.GetLength(0); i++) {
            for (int j = 0; j < LG.map.GetLength(1); j++) {
                if (LG.map[i, j] != null) {
                    Vector2 cellPosition = ReturnPositionOnMap(i, j);
                    
                    GameObject room = Instantiate(_minimapTile);
                    room.name = "Room "  + (new Vector2Int(i, j));
                    room.transform.SetParent(transform);
                    room.GetComponent<RectTransform>().sizeDelta = cellSize;
                    
                    if (LG.map[i, j].type == RoomType.START) 
                        room.GetComponent<RectTransform>().anchoredPosition = cellPosition;
                    else
                        room.GetComponent<RectTransform>().anchoredPosition = cellPosition;

                    // Sets the sprite ( 5/26/2020 9:31pm )
                    #region sprite stuff
                    // 1 door sprites ( 5/26/2020 9:33pm )
                    if (LG.map[i, j].doors["N"] && !LG.map[i, j].doors["E"] && !LG.map[i, j].doors["S"] && !LG.map[i, j].doors["W"]) {
                        room.GetComponent<Image>().sprite = _tiles["N"];

                    } else if (LG.map[i, j].doors["E"] && !LG.map[i, j].doors["N"] && !LG.map[i, j].doors["S"] && !LG.map[i, j].doors["W"]) {
                        room.GetComponent<Image>().sprite = _tiles["E"];

                    } else if (LG.map[i, j].doors["S"] && !LG.map[i, j].doors["N"] && !LG.map[i, j].doors["E"] && !LG.map[i, j].doors["W"]) {
                        room.GetComponent<Image>().sprite = _tiles["S"];

                    } else if (LG.map[i, j].doors["W"] && !LG.map[i, j].doors["N"] && !LG.map[i, j].doors["E"] && !LG.map[i, j].doors["S"]) {
                        room.GetComponent<Image>().sprite = _tiles["W"];

                    }
                    
                    // 2 Door sprites ( 5/26/2020 9:33pm )
                    if (LG.map[i, j].doors["N"]) {
                        if (LG.map[i, j].doors["E"] && !LG.map[i, j].doors["S"] && !LG.map[i, j].doors["W"]) {
                            room.GetComponent<Image>().sprite = _tiles["NE"];

                        } else if (LG.map[i, j].doors["W"] && !LG.map[i, j].doors["E"] && !LG.map[i, j].doors["S"]) {
                            room.GetComponent<Image>().sprite = _tiles["NW"];

                        }

                    } else if (LG.map[i, j].doors["S"]) {
                        if (LG.map[i, j].doors["E"] && !LG.map[i, j].doors["N"] && !LG.map[i, j].doors["W"]) {
                            room.GetComponent<Image>().sprite = _tiles["SE"];

                        } else if (LG.map[i, j].doors["W"] && !LG.map[i, j].doors["N"] && !LG.map[i, j].doors["E"]) {
                            room.GetComponent<Image>().sprite = _tiles["SW"];
                        }
                    }

                    // 3 Door sprites ( 5/26/2020 9:33pm )
                    if (LG.map[i, j].doors["N"] && LG.map[i, j].doors["E"] && LG.map[i, j].doors["W"] && !LG.map[i, j].doors["S"]) {
                        room.GetComponent<Image>().sprite = _tiles["TN"];

                    } else if (LG.map[i, j].doors["E"] && LG.map[i, j].doors["N"] && LG.map[i, j].doors["S"] && !LG.map[i, j].doors["W"]) {
                        room.GetComponent<Image>().sprite = _tiles["TE"];

                    } else if (LG.map[i, j].doors["S"] && LG.map[i, j].doors["E"] && LG.map[i, j].doors["W"] && !LG.map[i, j].doors["N"]) {
                        room.GetComponent<Image>().sprite = _tiles["TS"];

                    } else if (LG.map[i, j].doors["W"] && LG.map[i, j].doors["N"] && LG.map[i, j].doors["S"] && !LG.map[i, j].doors["E"]) {
                        room.GetComponent<Image>().sprite = _tiles["TW"];

                    }

                    // Other cases ( 5/26/2020 9:38pm )
                    if (LG.map[i, j].doors["N"] && LG.map[i, j].doors["E"] && LG.map[i, j].doors["S"] && LG.map[i, j].doors["W"]) {
                        room.GetComponent<Image>().sprite = _tiles["X"];

                    } else if (!LG.map[i, j].doors["N"] && !LG.map[i, j].doors["E"] && !LG.map[i, j].doors["S"] && !LG.map[i, j].doors["W"]) {
                        room.GetComponent<Image>().sprite = _tiles["O"];
                    }

                    #endregion
                
                }
            }
        }

        _tileHighlight.transform.SetAsLastSibling();
    }

    public void On_SetCurentRoom(EventParams eventParams) {
        if (eventParams.roomParam != null) {

            _selectedVector = eventParams.roomParam.position;
            _tileHighlight.GetComponent<RectTransform>().anchoredPosition = ReturnPositionOnMap(_selectedVector.x, _selectedVector.y);
        } else {
            Debug.LogError("Eventparams with non-null room param expected.");
        }
    }

    private void DeleteMap() {
        foreach(Transform child in transform) {
            if (child.gameObject != _tileHighlight) {
                Destroy(child.gameObject);
            }
            
        }
    }
}
