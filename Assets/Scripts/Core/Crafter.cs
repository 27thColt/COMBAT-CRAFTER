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

    public delegate void crafterUpdate(ItemType _item = null, ItemType _second = null);
    public static event crafterUpdate OnResultUpdate;

    // Event for when an item has been crafted ( 12/27/2019 11:49am )
    public delegate void ItemCraft(ItemType _item);
    public static event ItemCraft OnItemCrafted;

    #endregion

    #region Singleton and Awake

    public static Crafter instance;

    private void Awake() {
        instance = this;
        OnResultUpdate += UpdateResultItem;
        OnBattlestateChanged += Crf_StateListener;
    }

    #endregion

    private void OnDestroy() {
        OnResultUpdate -= UpdateResultItem;
        OnBattlestateChanged -= Crf_StateListener;
    }

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

    // Updates the crafting UI once an action is called -- Now this only refers to the Crafter Button ( 6/5/2019 6:21pm )
    public void UpdateCraftingUI() {
        if (itemAmt == 2) {
            // Crafting Button ( 6/5/2019 6:21pm )
            if (CheckRecipe(craftingSlots[0], craftingSlots[1]) != null) {
                Recipe _recipe = CheckRecipe(craftingSlots[0], craftingSlots[1]);

                OnResultUpdate(_recipe.result, _recipe.resultSecondary);
            } else {
                OnResultUpdate();
            }

        } else if (itemAmt == 1) {
            OnResultUpdate(craftingSlots[0]);

        } else {
            OnResultUpdate();

        }
    }

    public void AddItem(ItemType _item) {
        craftingSlots.Add(_item);
        UpdateCraftingUI();
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

        foreach(ItemType _itemType in craftingSlots) {
            Inventory.instance.RemoveItem(_itemType);
        }

        print(_resultItem.itemName + " Crafted!");

        if (_resultSecond != null && !Inventory.instance.HasItem(_resultSecond)) {
            Inventory.instance.AddItem(_resultSecond);
        }


        FinishCurrentState(Bstate.player_CRAFT);
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

    public void Crf_StateListener(Bstate _state) {
        if (_state == Bstate.game_ROUNDRESET) {
            for (int i = 0; i < craftingSlots.Count; i++) {
                RemoveItem(craftingSlots[i]);

                i--;
            }
            
        }
    }

    #endregion
}
