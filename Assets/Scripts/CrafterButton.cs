using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/* 10/30/2019 1:47pm - Crafter Button
 * Script specifically made for the crafting
 * 
 */ 
public class CrafterButton : MonoBehaviour {
    private Button _button;

    public Image resultSprite;

    

    private void Awake() {
       Crafter.OnResultUpdate += UpdateResultButton;
    }

    void Start() {
        _button = GetComponent<Button>();
    }

    #region Functions

    // Will be called uppon button press of the crafter button-- referenced through unity on the button component ( 12/26/2019 10:18pm )
    public void OnCrafterButtonPressed() {
        _button.interactable = false;
    }

    #endregion

    #region Event Listeners

    public void UpdateResultButton(Item _item = null, Item _second = null) {
        if (_item != null) {
            _button.interactable = true;

            resultSprite.enabled = true;
            resultSprite.sprite = _item.sprite;

        } else {
            _button.interactable = false;

            resultSprite.enabled = false;
            resultSprite.sprite = null;

        }
    }

    #endregion
}
