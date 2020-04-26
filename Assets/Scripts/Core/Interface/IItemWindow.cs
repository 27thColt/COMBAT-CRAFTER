using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 4/23/2020 7:18pm - Item Drop Handler Interface
 * 
 * interface including values and functions that are used in UI windows that feature item interaction
 */ 
interface IItemWindow {
    bool Interactable { get; set; } // enables interaction when true, disables it when false ( 4/23/2020 7:19pm )
    GameObject Pool { get; set; } // Child object that actually holds the items ( 4/23/2020 7:40pm )
}
