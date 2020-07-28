using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  5/19/2020 11:28am - Level Generator script
    Script which handles the level generation.
    This will take rooms and procedurally generate them or something


    First time doing this so this will be exciting.
*/
public class Level {
    private System.Random _rnd = new System.Random();
    public Room[,] rooms;
    private List<Vector2Int> _takenCoords;
    
    // Maximum width and length for the maxe ( 5/19/2020 11:41am )
    private int _mapSizeX;
    private int _mapSizeY;
    private int _numOfRooms;
    private int _numOfTreasure;

    public Level(int mapSizeX, int mapSizeY, int numOfRooms) {
        _mapSizeX = mapSizeX;
        _mapSizeY = mapSizeY;
        _numOfRooms = numOfRooms;

        _numOfTreasure = Mathf.RoundToInt(0.33f * _numOfRooms); // Mathf.RoundToInt(Mathf.Log(_numOfRooms, 2)) <-- Original formula (changed because growth was too slow) ( 5/24/2020 10:06pm )
    }

    public void GenerateRooms() {
        if (_numOfRooms > _mapSizeX * _mapSizeY) {
            Debug.LogError("Cannot generate map, too many rooms of " + _numOfRooms + " does not fit map size of " + _mapSizeX + " by " + _mapSizeY);
            return;
        }
        
        // Initialize some values ( 5/19/2020 1:29pm )
        rooms = new Room[_mapSizeX, _mapSizeY];
        _takenCoords = new List<Vector2Int>();
        

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
                if (rooms[_coords.x, _coords.y].displacement > _displacement) {
                    _farthestCoords = _coords;
                    _displacement = rooms[_coords.x, _coords.y].displacement;
                }
            }

            rooms[_farthestCoords.x, _farthestCoords.y].type = RoomType.EXIT;
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
                    foreach(Room _room in rooms) {
                        if (_room != null) {
                            if (Vector2Int.Distance(_room.position, _cachedVector) == 1 && _room.type == RoomType.TREASURE) { 
                                _redoCheck = true;
                            }
                        }
                    }
                    // Debug.Log("TREASURE #" + i + " | AVAILABLE VECTORS LEFT: " + _availableVectors.Count + " | NUM OF TREASURE LEFT" + (_numOfTreasure - i) + " | REDO CHECK? " + _redoCheck);
                } while (_redoCheck && _availableVectors.Count > _numOfTreasure - i);
            } while (rooms[_cachedVector.x, _cachedVector.y].type != RoomType.DEFAULT);

            rooms[_cachedVector.x, _cachedVector.y].type = RoomType.TREASURE;      
        }
    }

    private void AddRoomToMap(RoomType _roomType, Vector2Int _coords, int _displacement = 0) {
        rooms[_coords.x, _coords.y] = new Room(_roomType, _coords, _displacement);
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
                        if (_numOfRooms > 7 && (rooms[_cachedVector.x, _cachedVector.y].displacement >= _numOfRooms - 4)) _redoCheck = true;
                    } while (_redoCheck);
                } while ((_numOfAdjacent >= 3 && _rnd.Next(0, _numOfRooms) < rooms[_cachedVector.x, _cachedVector.y].displacement - 1)
                        && i > 3);

                int _numOfDoors = 0;

                foreach (bool _bool in rooms[_cachedVector.x, _cachedVector.y].doors.Values)
                    if (_bool) _numOfDoors++;


                /*
                    This will keep looping until it finds a valid room adjacent to the cached room ( 5/22/2020 2:12pm )
                */
                do {
                    /*
                        The following two if statements try to prevent long straight chains of rooms ( 5/22/2020 2:24pm )
                    */
                    // If the cached room already contains a door to its east or west ( 5/22/2020 2:20pm )
                    if (_numOfDoors == 1 && (rooms[_cachedVector.x, _cachedVector.y].doors["E"] || rooms[_cachedVector.x, _cachedVector.y].doors["W"])) {
                        _roomCoords.x = _cachedVector.x;
                        _roomCoords.y = _cachedVector.y + _rnd.Next(-1, 2);

                    // If the cached room already contains a door to its north or south ( 5/22/2020 2:21pm )
                    } else if (_numOfDoors == 1 && (rooms[_cachedVector.x, _cachedVector.y].doors["N"] || rooms[_cachedVector.x, _cachedVector.y].doors["S"])) {
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

            AddRoomToMap(RoomType.DEFAULT, _roomCoords, rooms[_cachedVector.x, _cachedVector.y].displacement + 1);

            /* 
                Sets door values ( 5/20/2020 10:56 am )
            */

            // if new room is to the north of cached room ( 5/20/2020 10:24am )
            if (_roomCoords.y == _cachedVector.y - 1) {
                rooms[_cachedVector.x, _cachedVector.y].doors["N"] = true;
                rooms[_roomCoords.x, _roomCoords.y].doors["S"] = true;

            // If new room is to the east of cached room ( 5/20/2020 10:24am )
            } else if (_roomCoords.x == _cachedVector.x - 1) {
                rooms[_cachedVector.x, _cachedVector.y].doors["W"] = true;
                rooms[_roomCoords.x, _roomCoords.y].doors["E"] = true;
            
            // if new room is to the south of cached room ( 5/20/2020 10:24am )
            } else if (_roomCoords.y == _cachedVector.y + 1) {
                rooms[_cachedVector.x, _cachedVector.y].doors["S"] = true;
                rooms[_roomCoords.x, _roomCoords.y].doors["N"] = true;

            // If new room is to the west of cached room ( 5/20/2020 10:24am )
            } else if (_roomCoords.x == _cachedVector.x + 1) {
                rooms[_cachedVector.x, _cachedVector.y].doors["E"] = true;
                rooms[_roomCoords.x, _roomCoords.y].doors["W"] = true;

            }
        }
    }

    public Room ReturnStartRoom() {
        for (int i = 0; i < rooms.GetLength(0); i++) {
            for (int j = 0; j < rooms.GetLength(1); j++) {
                if (rooms[i, j] != null) {
                    if (rooms[i, j].type == RoomType.START) {
                        return rooms[i, j];
                    } 
                }
            }
        }

        return null;
    }

    public List<Room> ReturnAdjacentRooms(Vector2Int coords) {
        List<Room> outputList = new List<Room>();

        if (rooms[coords.x, coords.y].doors["N"]) outputList.Add(rooms[coords.x, coords.y - 1]);
        if (rooms[coords.x, coords.y].doors["E"]) outputList.Add(rooms[coords.x + 1, coords.y]);
        if (rooms[coords.x, coords.y].doors["S"]) outputList.Add(rooms[coords.x, coords.y + 1]);
        if (rooms[coords.x, coords.y].doors["W"]) outputList.Add(rooms[coords.x - 1, coords.y]);

        return outputList;
    }
}