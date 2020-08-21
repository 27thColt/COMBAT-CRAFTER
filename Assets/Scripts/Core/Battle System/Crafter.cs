using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BattleStateMachine;

/* 5/31/2019 3:52pm - Crafter Script
 * Manages crafting system
 */
public class Crafter : MonoBehaviour {
    public bool craftingEnabled = false;
    
    private Item _resultItem;
    public Recipe[] recipeList;
    public int itemAmt;

    private List<Item> craftingSlots = new List<Item>();

    private void Awake() {
        EventManager.StartListening("ResultUpdate", On_ResultUpdate);
    }

    private void OnDestroy() {
        EventManager.StopListening("ResultUpdate", On_ResultUpdate);
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

        EventManager.TriggerEvent("ResetCrafter", new EventParams());
    }

    public void UncraftItems() {
        foreach (Item _item in craftingSlots) {
            if (Inventory.instance.HasItem(_item)) {   
                Inventory.instance.AddItem(_item);
            } else {
                if (_item is Weapon)
                    Inventory.instance.AddItem(new Weapon(_item.itemType, _item.UID, 1, 1));
                else
                    Inventory.instance.AddItem(new Item(_item.itemType, _item.UID));    
            }
        }

        Inventory.instance.RenderInventory();
        print(_resultItem.itemType.itemName + " uncrafted.");
    }

    // If an item may be added to the inventory (aka, added to the inventory, not consumed immediately), it will ( 7/28/2020 10:20pm )
    public void PerformItemFunction() {
        if (!craftingSlots.Contains(_resultItem)) {
            _resultItem.OnCraft();
        }
    }

    #endregion

    #region Button Listeners

    // Will be called uppon button press of the crafter button-- referenced through unity on the button component ( 12/27/2019 11:49am )
    public void OnCrafterButtonPressed() {

        foreach (Item _item in craftingSlots) {
            Inventory.instance.RemoveItem(_item);
        }

        print(_resultItem.itemType.itemName + " Crafted");
        PerformItemFunction();
        Inventory.instance.RenderInventory();

        craftingEnabled = false;
        

        // END OF PlayerCraft Battle State ( 7/28/2020 9:24pm )
        BattleStateMachine.currentBState.End(new EventParams(_resultItem), "PlayerCraft");
    }
    
    #endregion

    #region Event Listeners

    // Will set the result item whenever it is updated ( 12/27/2019 12:49pm )
    private void On_ResultUpdate(EventParams eventParams) {
        if (eventParams.itemParam != null)
            _resultItem = eventParams.itemParam;
        else
            _resultItem = null;
    }

    #endregion
}
