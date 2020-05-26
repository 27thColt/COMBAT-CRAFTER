using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BattleState;

/* 5/31/2019 3:52pm - Crafter Script
 * Manages crafting system
 */
public class Crafter : MonoBehaviour {
    #region Singleton
    private static Crafter _crafter;

    public static Crafter instance {
        get {
            if (!_crafter) {
                _crafter = FindObjectOfType(typeof(Crafter)) as Crafter;

                if (!_crafter) {
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                }
            }

            return _crafter;
        }
    }

    #endregion
    private Item _resultItem;
    public Recipe[] recipeList;
    public int itemAmt;

    private List<Item> craftingSlots = new List<Item>();

    private void Awake() {
        EventManager.StartListening("ResultUpdate", On_ResultUpdate);
        EventManager.StartListening("BStateChange", On_BStateChange);
        EventManager.StartListening("CancelCraft", On_CancelCraft);
    }

    private void OnDestroy() {
        EventManager.StopListening("ResultUpdate", On_ResultUpdate);
        EventManager.StopListening("BStateChange", On_BStateChange);
        EventManager.StopListening("CancelCraft", On_CancelCraft);
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
            if (CheckRecipe(craftingSlots[0].itemType, craftingSlots[1].itemType) != null) {
                Recipe _recipe = CheckRecipe(craftingSlots[0].itemType, craftingSlots[1].itemType);
                
                if (_recipe.result is WeaponType) {
                    EventManager.TriggerEvent("ResultUpdate", new EventParams(new Weapon(_recipe.result, Inventory.instance.itemInv.Count)));
                } else {
                    EventManager.TriggerEvent("ResultUpdate", new EventParams(new Item(_recipe.result, Inventory.instance.itemInv.Count)));
                }
                
            } else {
                EventManager.TriggerEvent("ResultUpdate", new EventParams());
            }

        } else if (itemAmt == 1) {
            EventManager.TriggerEvent("ResultUpdate", new EventParams(craftingSlots[0]));

        } else {
            EventManager.TriggerEvent("ResultUpdate", new EventParams());

        }
    }

    public void AddItem(Item _item) {
        craftingSlots.Add(_item);
        UpdateCraftingUI();
    }

    public void RemoveItem(Item _item) {
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

    public void ResetCrafter() {
        for (int i = 0; i < craftingSlots.Count; i++) {
            RemoveItem(craftingSlots[i]);
            i--;
        }

        _resultItem = null;
    }

    #endregion

    #region Button Listeners

    // Will be called uppon button press of the crafter button-- referenced through unity on the button component ( 12/27/2019 11:49am )
    public void OnCrafterButtonPressed() {
        EventManager.TriggerEvent("ItemCraft", new EventParams(_resultItem));

        foreach (Item _item in craftingSlots) {
            Inventory.instance.RemoveItem(_item);
        }

        print(_resultItem.itemType.itemName + " Crafted");
        FinishCurrentState(Bstate.player_CRAFT);
    }
    
    #endregion

    #region Event Listeners

    // When the Cancel Craft button is pressed ( 5/16/2020 3:48pm )
    private void On_CancelCraft(EventParams _eventParams) {
        if (currentState == Bstate.player_ENEMYSELECTION) {
            foreach (Item _item in craftingSlots) {
                Inventory.instance.AddItem(_item);
            }

            print(_resultItem.itemType.itemName + " uncrafted.");

            ResetCrafter();
        }
    }

    // Will set the result item whenever it is updated ( 12/27/2019 12:49pm )
    private void On_ResultUpdate(EventParams _eventParams) {
        if (_eventParams.itemParam != null) {
            _resultItem = _eventParams.itemParam;
        } else {
            _resultItem = null;
        }
    }

    private void On_BStateChange(EventParams _eventParams) {
        // Resets the crafter values ( 5/18/2020 9:39am )
        if (_eventParams.bstateParam == Bstate.game_ROUNDRESET) {
            ResetCrafter();

        // Does item On Craft action when player attacks ( 5/18/2020 9:50am )
        } else if (_eventParams.bstateParam == Bstate.player_ATTACK) {
            if (!craftingSlots.Contains(_resultItem)) {
                _resultItem.OnCraft();
            }
        }
    }

    #endregion
}
