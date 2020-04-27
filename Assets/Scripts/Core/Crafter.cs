using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BattleState;

/* 5/31/2019 3:52pm - Crafter Script
 * Manages crafting system
 */
public class Crafter : MonoBehaviour {
    private ItemType _resultItem;
    private ItemType _resultSecond;

    // Drag&Dropped values ( 12/28/2019 10:38pm )
    [SerializeField]
    private GameObject craftingSlot1 = null;

    [SerializeField]
    private GameObject craftingSlot2 = null;

    public Recipe[] recipeList;
    //public List<Recipe> knownRecipes; // List of recipes known to the player ( 12/28/2019 10:38pm )

    public int itemAmt;

    private List<ItemType> craftingSlots = new List<ItemType>();
    
    #region Events

    public delegate void crafterPoolUpdate(ItemType _item = null, ItemType _second = null);
    public static event crafterPoolUpdate OnResultUpdate;

    // Event for when an item has been crafted ( 12/27/2019 11:49am )
    public delegate void ItemCraft(ItemType _item);
    public static event ItemCraft OnItemCrafted;

    #endregion

    #region Singleton and Awake

    public static Crafter instance;

    private void Awake() {
        instance = this;
        OnResultUpdate += UpdateResultItem;
        DragHandler.OnItemEndDrag += RemoveItemFromCrafter;
        OnBattlestateChanged += CrfStateListener;
    }

    #endregion

    void Start() {
        /* The following is how I plan to do this shit ( 6/3/2019 6:04pm )
         * 
         * For once I won't be using Brackeys as a basis for programming.
         * 
         * Combine two items, make a another thing.
         * 
         * > Take in the two items as input
         * > Read the first item
         * > Read the second item
         * > Does the two items match an existing recipe?
         * > If yes, then make an available item
         */

        recipeList = Resources.LoadAll<Recipe>("Recipes");
    }

    #region Functions

    // Updates the crafting UI once an action is called (Drag or Drop) ( 6/5/2019 6:21pm )
    public void UpdateCraftingUI() {
        if (itemAmt == 2) {
            Inventory.instance.RemoveAllGhostItems();

            // Crafting Button ( 6/5/2019 6:21pm )
            if (CheckRecipe(craftingSlots[0], craftingSlots[1]) != null) {
                Recipe _recipe = CheckRecipe(craftingSlots[0], craftingSlots[1]);

                OnResultUpdate(_recipe.result, _recipe.resultSecondary);

                if (_recipe.resultSecondary != null && !Inventory.instance.HasItem(_recipe.resultSecondary))
                    Inventory.instance.CreateGhostItem(_recipe.resultSecondary);

            } else {
                OnResultUpdate();
            }

            // Background Sprites ( 6/5/2019 6:21pm )
            craftingSlot1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-75, 0);
            craftingSlot2.GetComponent<Image>().enabled = true;

        } else if (itemAmt == 1) {
            Inventory.instance.RemoveAllGhostItems();

            OnResultUpdate(craftingSlots[0]);

            // Background Sprites ( 6/5/2019 6:46pm )
            craftingSlot1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            craftingSlot2.GetComponent<Image>().enabled = false;

        } else {
            Inventory.instance.RemoveAllGhostItems();

            OnResultUpdate();

            // Background Sprites ( 6/5/2019 6:21pm )
            craftingSlot1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-75, 0);
            craftingSlot2.GetComponent<Image>().enabled = true;
        }
    }

    // Updates the result button and places the sprite of the resulting item ( 6/12/2019 11:00am )
    /* Moved to CrafterButton.cs ( 10/30/2019 2:00pm )
     * 
     * Just a note: Was replaced by 'OnResultUpdate()' event ( 12/26/2019 10:46pm )
    public void UpdateResultSprite(Item _item = null) {
        if (_item != null) {
            resultSprite.enabled = true;
            resultSprite.sprite = _item.sprite;

        } else {
            resultSprite.enabled = false;
            resultSprite.sprite = null;

        }
    } */

    public void AddItem(ItemType _item) {
        craftingSlots.Add(_item);
    }

    public void RemoveItem(ItemType _item) {
        craftingSlots.Remove(_item);

        if (itemAmt >= 1 && itemAmt <= 2) {
            itemAmt--;


            UpdateCraftingUI();
        }
        
    }



    // Checks if two items match a recipe from the recipe list ( 6/3/2019 10:04pm )
    public Recipe CheckRecipe(ItemType _item1, ItemType _item2) {
        for (int i = 0; i < recipeList.Length; i++) {
            if ((_item1 == recipeList[i].material1 && _item2 == recipeList[i].material2) || (_item2 == recipeList[i].material1 && _item1 == recipeList[i].material2))
                return recipeList[i];
        }
        return null;
    }

    // Will be called uppon button press of the crafter button-- referenced through unity on the button component ( 12/27/2019 11:49am )
    public void OnCrafterButtonPressed() {
        OnItemCrafted(_resultItem);
        print(_resultItem.itemName + " Crafted!");

        if (_resultSecond != null && !Inventory.instance.HasItem(_resultSecond)) {
            Inventory.instance.AddItem(_resultSecond);
            Inventory.instance.RemoveAllGhostItems();
        }

        SetCurrentState(Bstate.player_ENEMYSELECTION);

    }

    #endregion

    #region Event Listeners

    // Will set the result item whenever it is updated ( 12/27/2019 12:49pm )
    public void UpdateResultItem(ItemType _item = null, ItemType _second = null) {
        if (_item != null) {
            _resultItem = _item;

            if (_second != null) {
                _resultSecond = _second;
            } else {
                _resultSecond = null;
            }
        } else {
            _resultItem = null;
        }
    }

    // Event listener which will fire once an item has been taken out of crafter, OnEndDrag() in DragHandler ( 12/28/2019 10:04pm )
    public void RemoveItemFromCrafter(GameObject _window, ItemType _item) {
        // This function used to be in DragHandler, but I just moved it to here to allow itemAmt to be privatized and for better modularization ( 12/28/2019 10:11pm )

        // Item removing process if the item was taken out of the crafter ( 6/3/2019 6:45pm )
        if (_window.GetComponent<CrafterDropHandler>() != null && _item != null) {
            RemoveItem(_item);
        }
    }

    public void CrfStateListener(Bstate _state) {
        if (_state == Bstate.game_ROUNDRESET) {
            for (int i = 0; i < craftingSlots.Count; i++) {
                RemoveItem(craftingSlots[i]);
            }
            
        }
    }

    #endregion
}
