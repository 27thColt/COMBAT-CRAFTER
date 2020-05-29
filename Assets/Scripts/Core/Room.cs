using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  5/18/2020 9:00pm - Room Class
    Holds information for each room generated
*/
[System.Serializable]
public class Room {
    public Vector2Int position;    // Position for where on the map grid the room is located on ( 5/19/2020 11:26am )
    public RoomType type { get; set; }
    // booleans for whether or not a room has an exit leading to either of the cardinal directions ( 5/19/2020 11:27am )
    public Dictionary<string, bool> doors = new Dictionary<string, bool> {
        ["N"] = false, 
        ["E"] = false, 
        ["S"] = false, 
        ["W"] = false
    };   

    public int displacement = 0; // Distance from the center of the map; Used in map generation ( 5/20/2020 1:46pm )

    public Room(RoomType _type, Vector2Int _position, int _displacement = 0) {
        type = _type;
        position = _position;
        displacement = _displacement;
    }
}

public enum RoomType {
    DEFAULT,
    START,
    TREASURE,
    EXIT
}
