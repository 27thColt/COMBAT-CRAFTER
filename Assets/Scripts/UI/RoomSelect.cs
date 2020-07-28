using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
    R6/3/2020 9:32pm - RoomSelect.cs
    Attached onto any room prefab
*/
public class RoomSelect : MonoBehaviour, IPointerClickHandler {
    #region IPointerClickHandler

    public void OnPointerClick(PointerEventData eventData) {
        if (LevelState.currentLstate == Lstate.EXPLORE) {
            EventManager.TriggerEvent("RoomSelect", new EventParams(containedRoom));
        }
    }

    #endregion

    public Room containedRoom = null;
}
