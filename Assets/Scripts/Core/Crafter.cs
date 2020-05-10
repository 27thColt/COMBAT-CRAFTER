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
    private ItemType _resultItem;
    public Recipe[] recipeList;
    //public List<Recipe> knownRecipes; // List of recipes known to the player ( 12/28/2019 10:38pm )

    public int itemAmt;

    private List<ItemType> craftingSlots = new List<ItemType>();

    private void Awake() {
        EventManager.StartListening("ResultUpdate", On_ResultUpdate);
        EventManager.StartListening("BStateChange", On_BStateChange);
    }


    private void OnDestroy() {
        EventManager.StopListening("ResultUpdate", On_ResultUpdate);
        EventManager.StopListening("BStateChange", On_BStateChange);
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
                
                EventManager.TriggerEvent("ResultUpdate", new EventParams(_recipe.result));
            } else {
                EventManager.TriggerEvent("ResultUpdate", new EventParams());
            }

        } else if (itemAmt == 1) {
            EventManager.TriggerEvent("ResultUpdate", new EventParams(craftingSlots[0]));

        } else {
            EventManager.TriggerEvent("ResultUpdate", new EventParams());

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
        EventManager.TriggerEvent("ItemCraft", new EventParams(_resultItem));

        foreach(ItemType _itemType in craftingSlots) {
            Inventory.instance.RemoveItem(_itemType);
        }

        print(_resultItem.itemName + " Crafted!");

        FinishCurrentState(Bstate.player_CRAFT);
    }

    #endregion

    #region Event Listeners

    // Will set the result item whenever it is updated ( 12/27/2019 12:49pm )
    public void On_ResultUpdate(EventParams _eventParams) {
        if (_eventParams.itemTypeParam1 != null) {
            _resultItem = _eventParams.itemTypeParam1;
        } else {
            _resultItem = null;
        }
    }

    public void On_BStateChange(EventParams _eventParams) {
        if (_eventParams.bstateParam == Bstate.game_ROUNDRESET) {
            for (int i = 0; i < craftingSlots.Count; i++) {
                RemoveItem(craftingSlots[i]);

                i--;
            }
            
        }
       
    }

    #endregion
}
