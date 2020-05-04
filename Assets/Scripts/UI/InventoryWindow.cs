using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static BattleState;
using TMPro;

/* 5/31/2019 1:37pm - Inventory Pool Script
 * The 'collider' which detects when items have been dropped back again into the inventory
 * Attached to inventory panel game object
 * 
 * 
 * 
 * 
 * 5/4/2020 2:34pm - AS OF NOW, THIS SCRIPT WILL NOT BE USED (Item Drag functionality removed)
 */
public class InventoryWindow : MonoBehaviour, IDropHandler, IItemWindow {

    #region IItemWindow
    public bool Interactable { get; set; } = false;
    public GameObject Pool { get; set; }

    #endregion

    void Awake() {
        OnBattlestateChanged += InvDHListener;
        CrafterWindow.OnClearCrafter += ReturnToInventory;
    }

    private void OnDestroy() {
        OnBattlestateChanged -= InvDHListener;
        CrafterWindow.OnClearCrafter -= ReturnToInventory;
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
    public void InvDHListener(Bstate _state) {
        if (_state == Bstate.player_CRAFT) {
            Interactable = true;
        }

        if (lastState == Bstate.player_CRAFT) {

            //print(_state);
            //print("inventory will NOT be interactable");

            Interactable = false;
        }
    }

    private void ReturnToInventory(List<GameObject> _items) {
        foreach (GameObject _item in _items) {
            _item.transform.SetParent(Pool.transform);
        }
    }

    #endregion
}
