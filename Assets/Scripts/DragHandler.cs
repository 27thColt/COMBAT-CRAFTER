using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* 5/30/2019 9:47pm - DragHandler Script
 * Manages the item drag and drop with the mouse
 * Tho im kinda lazy so imma just do it tomorrow something
 * 
 * 5/31/2019 11:30am - WHY AM I WATCHING VGHS WHILE PROGRAMMING I CANT EVEN GET ANY WORK DONE
 * So yeah I took a lot from Brackey's "Crafty" game, thanks! Learning a lot as well.
 * 
 * 
 * 10/30/2019 4:45am - important to note that this is only being attached to draggable objects (aka the items themselves)
 */
public class DragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    #region Private Values

    private GameObject _itemDraggerObject; // Items will be stored as a child of this object when being dragged ( 5/31/2019 1:32pm )

    private GameObject _inventoryPool;
    private GameObject _inventoryPoolScript; // Holds InventoryPool script; not actually the inventory pool parent. Confusing right? ( 10/30/2019 4:42am)

    private GameObject _startingPool = null; // Where the item was first in (either inventory or crafter) ( 5/31/2019 2:36pm )

    #endregion

    public static GameObject draggedObject;

    public delegate void itemEndDrag(string _tag, ItemType _item);
    public static event itemEndDrag OnItemEndDrag;

    /* 5/31/2019 10:52am - I might get confused in the future by this so imma just put this here
     * Awake() calls before Start() ; Start() are for inits that depend on other components, while Awake() is for this component
     */
    private void Awake() {

    }

    private void Start() {
        _itemDraggerObject = GameObject.FindGameObjectWithTag("ItemDraggerObject");
    }

    #region IDragHandler

    public void OnBeginDrag(PointerEventData eventData) {
        _startingPool = transform.parent.gameObject;

        // Will not do anything if the inventory pool is not interactrable ( 10/30/2019 1:45pm )
        ItemWindow _itemPool = _startingPool.GetComponentInParent(typeof(ItemWindow)) as ItemWindow;
        if (_itemPool.Interactable == false)
            return;

        //Debug.Log("ON BEGIN DRAG"); // Oh my god these are fucking annoying,, appearing in the console log ( 12/26/2019 10:23pm )

        draggedObject = gameObject;

        transform.SetParent(_itemDraggerObject.transform);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        if (draggedObject != null)
            transform.position = eventData.position;
    }

    /* OnEndDrag() triggers AFTER OnDrop() ( 5/31/2019 3:16pm )
     * Thank you quill18creates!! https://www.youtube.com/watch?v=P66SSOzCqFU
     */
    public void OnEndDrag(PointerEventData eventData) {
        
        //Debug.Log("ON END DRAG");

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // Ensures item amount does not exceed limit ( 5/31/2019 4:10pm )
        if (transform.parent != _itemDraggerObject.transform) {

            OnItemEndDrag(_startingPool.tag, GetDraggedItem());

            // If the object has not already been set as the parent of another pool ( 5/31/2019 3:17pm )
        } else {
            transform.SetParent(_startingPool.transform);
        }

        draggedObject = null;
    }

    #endregion

    #region Functions
    public static ItemType GetDraggedItem() {
        try {
            return draggedObject.GetComponent<ItemObject>().item;
        } catch {
            Debug.Log("ItemWindow is Currently Uninteractable");
            return null;
        }
        
    }

    #endregion
}
