using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static BattleState;

/* 5/31/2019 3:56pm - Crafting Pool Script
 * The 'collider' which detects when items have been dropped back again into the inventory
 * Attached to crafting panel game object
 * 
 * ^^ Copied that from the Inventory Pool Script, neat eh?
 * 
 * 
 * 
 * 
 * 
 * 5/4/2020 2:34pm - AS OF NOW, THIS SCRIPT WILL NOT BE USED (CrafterWindow removed)
 */
public class CrafterWindow : MonoBehaviour, IDropHandler, IItemWindow {

    #region IItemWindow

    public bool Interactable { get; set; } = false;
    public GameObject Pool { get; set; }

    #endregion

    private Crafter _crafter;

    public delegate void ClearWindow(List<GameObject> _objs);
    public static event ClearWindow OnClearCrafter;

    void Awake() {
        OnBattlestateChanged += CrfDHListener;
    }

    private void OnDestroy() {
        OnBattlestateChanged -= CrfDHListener;
    }


    public void Start() {
        Pool = GetComponentInChildren<GridLayoutGroup>().gameObject; // Assumes that the grid is the only one in the children that has this component ( 5/31/2019 3:32pm )
        _crafter = Crafter.instance;
    }

    #region IDropHandler

    // OnDrop() fires before OnEndDrag() ( 5/31/2019 3:29pm )
    public void OnDrop(PointerEventData eventData) {
        // Debug.Log("ON DROP");

        // Prevents anything from happening if there is no object OR if the number  of items in the crafter exceed the limit ( 5/31/2019 4:33pm )
        if (DragHandler.draggedObject == null || _crafter.itemAmt >= 2)
            return;

        DragHandler.draggedObject.transform.SetParent(Pool.transform);

        // Ensures item amount does not exceed limit ( 5/31/2019 4:10pm )
        if (_crafter.itemAmt >= 0 && _crafter.itemAmt <= 2)
            _crafter.itemAmt++;

        // Add the actual item to the crafting slot ( 6/3/2019 6:32pm )
        ItemType item = DragHandler.GetDraggedItem();
        _crafter.AddItem(item);
        _crafter.UpdateCraftingUI();
    }

    #endregion

    #region Event Listeners

    // Fires when the battlestate has been changed ( 12/27/2019 1:14pm )
    public void CrfDHListener(Bstate _state) {
        if (_state == Bstate.player_CRAFT) {
            Interactable = true;
        
        // Resets all items in the crafter ( 4/27/2020 3:17pm )
        } else if (_state == Bstate.game_ROUNDRESET) {
            List<GameObject> _items = new List<GameObject>();

            foreach (Transform _child in Pool.transform) {
                if (_child.GetComponent<ItemObject>() != null) {
                    _items.Add(_child.gameObject);
                }
                    
            }

            OnClearCrafter(_items);
        }

        if (lastState == Bstate.player_CRAFT) {
            Interactable = false;
        }
    }

    #endregion

}
