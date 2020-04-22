using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static BattleStateClass;


/* 5/31/2019 1:37pm - Inventory Pool Script
 * The 'collider' which detects when items have been dropped back again into the inventory
 * Attached to inventory panel game object
 */
public class InventoryDropHandler : MonoBehaviour, IDropHandler {
    private GameObject _pool;

    // This is changed between game states and other things. ( 10/30/2019 1:46pm )
    public bool interactable = false;

    void Awake() {
        OnBattlestateChanged += InvDropHandListener;
    }

    void Start() {
        _pool = GetComponentInChildren<GridLayoutGroup>().gameObject; // Assumes that the grid is the only one in the children that has this component ( 5/31/2019 3:32pm )
    }

    #region IDropHandler

    // OnDrop() fires before OnEndDrag() ( 5/31/2019 3:29pm )
    public void OnDrop(PointerEventData eventData) {
        //Debug.Log("ON DROP");

        if (DragHandler.draggedObject == null)
            return;

        DragHandler.draggedObject.transform.SetParent(_pool.transform);

        // Add the actual item to the inventory (
        Item item = DragHandler.GetDraggedItem();
        if (!Inventory.instance.HasItem(item))
            Inventory.instance.AddItem(item);
        
    }

    #endregion

    #region Event Listeners

    // Fires when the gamestate has been changed ( 12/27/2019 1:14pm )
    public void InvDropHandListener(Battlestate _state) {
        if (_state == Battlestate.player_CRAFT) {
            interactable = true;
            print("inventory will be interactable");
        }

        if (lastState == Battlestate.player_CRAFT) {

            //print(_state);
            //print("inventory will NOT be interactable");

            interactable = false;
        }
    }

    #endregion
}
