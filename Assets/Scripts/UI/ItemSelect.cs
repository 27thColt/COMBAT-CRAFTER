using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/* 5/4/2020 2:42pm - ItemSelect.cs
 * Attached to ItemObject. Selects an item for craft usage when it is clicked
 * 
 */

public class ItemSelect : MonoBehaviour, IPointerClickHandler {

    #region IPointerClickHandler

    // Fires when the item is clicked ( 5/4/2020 2:47pm )
    public void OnPointerClick(PointerEventData eventData) {
        if (_crafter == null) return;

        if (!_crafter.craftingEnabled) return;
        
        Item item = GetComponent<ItemObject>().item;

        if (_crafter.itemAmt < 2 && !_selected) {
            _crafter.itemAmt++;

            UpdateSelection(true);
            // Add the actual item to the crafting slot ( 6/3/2019 6:32pm )
            
            _crafter.AddItem(item);
        } else if (_selected) {
            UpdateSelection(false);
            _crafter.RemoveItem(item);
        }
    }

    #endregion

    private Crafter _crafter;

    [SerializeField]
    private Image _selectedBG = null;

    private bool _selected;

    void Awake() {
        EventManager.StartListening("ResetCrafter", On_ResetCrafter);
    }

    void OnDestroy() {
        EventManager.StopListening("ResetCrafter", On_ResetCrafter);
    }

    void Start() {
        _selected = false;

        _crafter = FindObjectOfType<Crafter>();
    }

    // Updates the selected status of the item ( 5/4/2020 2:58pm )
    private void UpdateSelection(bool _selection) {
        _selected = _selection;
        _selectedBG.enabled = _selection;
    }

    // Resets the selection state of the item ( 5/4/2020 2:56pm )
    private void On_ResetCrafter(EventParams _eventParams) {
        if (_selected == true) {
            UpdateSelection(false);
        }
    }
}
