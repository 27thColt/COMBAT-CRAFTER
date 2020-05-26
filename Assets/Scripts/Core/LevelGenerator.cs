using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  5/19/2020 11:28am - Level Generator script
    Script which handles the level generation.
    This will take rooms and procedurally generate them or something


    First time doing this so this will be exciting.
*/
public class LevelGenerator : MonoBehaviour {
    #region Singleton

    private static LevelGenerator _levelGenerator;

    public static LevelGenerator instance {
        get {
            if (!_levelGenerator) {
                _levelGenerator = FindObjectOfType(typeof(LevelGenerator)) as LevelGenerator;

                if (!_levelGenerator) {
                    Debug.LogError("There needs to be one active Level Generator script on a GameObject in your scene.");
                }
            }
            return _levelGenerator;
        }
    }
    
    #endregion


    private System.Random _rnd = new System.Random();

    public Room[,] map;
    private List<Vector2Int> _takenCoords;
    [SerializeField]
    private int _numOfRooms = 10;
    private int _numOfTreasure;
    // Maximum width and length for the maxe ( 5/19/2020 11:41am )
    [SerializeField]
    private int _mapSizeX = 10;
    [SerializeField]
    private int _mapSizeY = 10;

    public void GenerateRooms() {
        if (_numOfRooms > _mapSizeX * _mapSizeY) {
            Debug.LogError("Cannot generate map, too many rooms of " + _numOfRooms + " does not fit map size of " + _mapSizeX + " by " + _mapSizeY);
            return;
        }
        
        // Initialize some values ( 5/19/2020 1:29pm )
        map = new Room[_mapSizeX, _mapSizeY];
        _takenCoords = new List<Vector2Int>();
        _numOfTreasure = Mathf.RoundToInt(0.33f * _numOfRooms); // Mathf.RoundToInt(Mathf.Log(_numOfRooms, 2)) <-- Original formula (changed because growth was too slow) ( 5/24/2020 10:06pm )

         // Creates the starting Room at the center of the map ( 5/19/2020 1:28pm )
        Vector2Int _startCoords = new Vector2Int(Mathf.RoundToInt(_mapSizeX / 2), Mathf.RoundToInt(_mapSizeY / 2)); // Temporary value to hold the coords of the current room being created ( 5/20/2020 12:08am )
        AddRoomToMap(RoomType.START, _startCoords);

        GenerateMainLayout();

        /*
            Exit Room ( 5/24/2020 11:39am )
        */
        for (int i = 0; i < _numOfRooms; i++) {
            Vector2Int _farthestCoords = new Vector2Int();
            int _displacement = 0;

            foreach(Vector2Int _coords in _takenCoords) {
                if (map[_coords.x, _coords.y].displacement > _displacement) {
                    /*
                    int _numOfDoors = 0;
                    foreach (bool _door in map[_coords.x, _coords.y].doors) {
                        if (_door) _numOfDoors++;
                    }

                    if (_numOfDoors == 1) {*/
                        _farthestCoords = _coords;
                        _displacement = map[_coords.x, _coords.y].displacement;
                    //}
                }
            }

            map[_farthestCoords.x, _farthestCoords.y].type = RoomType.EXIT;
        }

        List<Vector2Int> _availableVectors = _takenCoords;
        /*
            Sets treasure Rooms ( 5/24/2020 11:39am )
        */
        for (int i = 0; i < _numOfTreasure; i++) { 
            Vector2Int _cachedVector;  
            do {
                bool _redoCheck;
                do {
                    _cachedVector = _availableVectors[_rnd.Next(0, _availableVectors.Count)];
                    _availableVectors.Remove(_cachedVector);    // This will prevent the system from chosing from the same vector again ( 5/24/2020 10:00pm )
 
                    _redoCheck = false;

                    /* 
                        Keep looping if any of the adjacent rooms are treasure rooms ( 5/24/2020 11:17am )
                    */
                    foreach(Room _room in map) {
                        if (_room != null) {
                            if (Vector2Int.Distance(_room.position, _cachedVector) == 1 && _room.type == RoomType.TREASURE) { 
                                _redoCheck = true;
                            }
                        }
                    }
                    // Debug.Log("TREASURE #" + i + " | AVAILABLE VECTORS LEFT: " + _availableVectors.Count + " | NUM OF TREASURE LEFT" + (_numOfTreasure - i) + " | REDO CHECK? " + _redoCheck);
                } while (_redoCheck && _availableVectors.Count > _numOfTreasure - i);
            } while (map[_cachedVector.x, _cachedVector.y].type != RoomType.DEFAULT);

            map[_cachedVector.x, _cachedVector.y].type = RoomType.TREASURE;      
        }
    }

    private void AddRoomToMap(RoomType _roomType, Vector2Int _coords, int _displacement = 0) {
        map[_coords.x, _coords.y] = new Room(_roomType, _coords, _displacement);
        if (!_takenCoords.Contains(_coords)) _takenCoords.Add(_coords);
    }

    /*
        Room Generation Main Loop ( 5/20/2020 10:57am )
    */
    private void GenerateMainLayout() {
        for (int i = 1; i < _numOfRooms; i++) {
            Vector2Int _roomCoords = new Vector2Int();
            Vector2Int _cachedVector;
            
            /* 
                This will keep looping until it has found a valid set of coordinates to create from
            */
            do {
                /*
                    This will keep lopoing until it finds an appropriate cached room (room to build off of) ( 5/22/2020 2:12pm )

                */
                int _numOfAdjacent;
                
                do {
                    bool _redoCheck; // If set to true, the check will loop again ( 5/24/2020 10:42am )
                    do {
                        _redoCheck = false;

                        _cachedVector = _takenCoords[_rnd.Next(0, _takenCoords.Count)];
                        _numOfAdjacent = 0;
                        
                        if (_takenCoords.Contains(new Vector2Int(_cachedVector.x, _cachedVector.y + 1))) _numOfAdjacent++;
                        if (_takenCoords.Contains(new Vector2Int(_cachedVector.x, _cachedVector.y - 1))) _numOfAdjacent++;
                        if (_takenCoords.Contains(new Vector2Int(_cachedVector.x + 1, _cachedVector.y))) _numOfAdjacent++;
                        if (_takenCoords.Contains(new Vector2Int(_cachedVector.x - 1, _cachedVector.y))) _numOfAdjacent++;

                        /*  Automatically break the loop if the cached room isnt too far away from the origin ( 5/22/2020 2:13pm )
                            This is in an effort to prevent too long of a chain
                        */
                        if (_numOfRooms > 7 && (map[_cachedVector.x, _cachedVector.y].displacement >= _numOfRooms - 4)) _redoCheck = true;
                    } while (_redoCheck);
                } while ((_numOfAdjacent >= 3 && _rnd.Next(0, _numOfRooms) < map[_cachedVector.x, _cachedVector.y].displacement - 1)
                        && i > 3);

                int _numOfDoors = 0;

                foreach (bool _bool in map[_cachedVector.x, _cachedVector.y].doors)
                    if (_bool) _numOfDoors++;


                /*
                    This will keep looping until it finds a valid room adjacent to the cached room ( 5/22/2020 2:12pm )
                */
                do {
                    /*
                        The following two if statements try to prevent long straight chains of rooms ( 5/22/2020 2:24pm )
                    */
                    // If the cached room already contains a door to its east or west ( 5/22/2020 2:20pm )
                    if (_numOfDoors == 1 && (map[_cachedVector.x, _cachedVector.y].doors[1] || map[_cachedVector.x, _cachedVector.y].doors[3])) {
                        _roomCoords.x = _cachedVector.x;
                        _roomCoords.y = _cachedVector.y + _rnd.Next(-1, 2);

                    // If the cached room already contains a door to its north or south ( 5/22/2020 2:21pm )
                    } else if (_numOfDoors == 1 && (map[_cachedVector.x, _cachedVector.y].doors[0] || map[_cachedVector.x, _cachedVector.y].doors[2])) {
                        _roomCoords.x = _cachedVector.x + _rnd.Next(-1, 2);
                        _roomCoords.y = _cachedVector.y;

                    } else {
                        if (_rnd.Next(0, 2) == 0) {
                            _roomCoords.x = _cachedVector.x + _rnd.Next(-1, 2);
                            _roomCoords.y = _cachedVector.y;
                        } else {    
                            _roomCoords.x = _cachedVector.x;
                            _roomCoords.y = _cachedVector.y + _rnd.Next(-1, 2);
                        }
                    }


                    

                // Makes sure the room isnt outside of the bounds ( 5/22/2020 2:14pm )
                } while (!(_roomCoords.x >= 0 && _roomCoords.x < _mapSizeX && _roomCoords.y >= 0 && _roomCoords.y < _mapSizeY));
            } while (_takenCoords.Contains(_roomCoords));

            AddRoomToMap(RoomType.DEFAULT, _roomCoords, map[_cachedVector.x, _cachedVector.y].displacement + 1);

            /* 
                Sets door values ( 5/20/2020 10:56 am )
            */

            // if new room is to the north of cached room ( 5/20/2020 10:24am )
            if (_roomCoords.y == _cachedVector.y - 1) {
                map[_cachedVector.x, _cachedVector.y].doors[0] = true;
                map[_roomCoords.x, _roomCoords.y].doors[2] = true;

            // If new room is to the east of cached room ( 5/20/2020 10:24am )
            } else if (_roomCoords.x == _cachedVector.x - 1) {
                map[_cachedVector.x, _cachedVector.y].doors[3] = true;
                map[_roomCoords.x, _roomCoords.y].doors[1] = true;
            
            // if new room is to the south of cached room ( 5/20/2020 10:24am )
            } else if (_roomCoords.y == _cachedVector.y + 1) {
                map[_cachedVector.x, _cachedVector.y].doors[2] = true;
                map[_roomCoords.x, _roomCoords.y].doors[0] = true;

            // If new room is to the west of cached room ( 5/20/2020 10:24am )
            } else if (_roomCoords.x == _cachedVector.x + 1) {
                map[_cachedVector.x, _cachedVector.y].doors[1] = true;
                map[_roomCoords.x, _roomCoords.y].doors[3] = true;

            }
        }
    }
}
