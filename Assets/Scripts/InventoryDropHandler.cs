using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static BattleStateManager;


/* 5/31/2019 1:37pm - Inventory Pool Script
 * The 'collider' which detects when items have been dropped back again into the inventory
 * Attached to inventory panel game object
 */
public class InventoryDropHandler : MonoBehaviour, IDropHandler, ItemWindow {
    public bool Interactable { get; set; } = false;
    public GameObject Pool { get; set; }

    void Awake() {
        OnBattlestateChanged += InvDHListener;
    }

    void Start() {
        Pool = GetComponentInChildren<GridLayoutGroup>().gameObject; // Assumes that the grid is the only one in the children that has this component ( 5/31/2019 3:32pm )
    }

    #region IDropHandler

    // OnDrop() fires before OnEndDrag() ( 5/31/2019 3:29pm )
    public void OnDrop(PointerEventData eventData) {
        //Debug.Log("ON DROP");

        if (DragHandler.draggedObject == null)
            return;

        DragHandler.draggedObject.transform.SetParent(Pool.transform);

        // Add the actual item to the inventory (
        ItemType item = DragHandler.GetDraggedItem();
        if (!Inventory.instance.HasItem(item))
            Inventory.instance.AddItem(item);
        
    }

    #endregion

    #region Event Listeners

    // Fires when the gamestate has been changed ( 12/27/2019 1:14pm )
    public void InvDHListener(Battlestate _state) {
        if (_state == Battlestate.player_CRAFT) {
            Interactable = true;
        }

        if (lastState == Battlestate.player_CRAFT) {

            //print(_state);
            //print("inventory will NOT be interactable");

            Interactable = false;
        }
    }

    #endregion
}
