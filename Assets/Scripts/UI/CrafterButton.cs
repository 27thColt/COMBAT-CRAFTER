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

    [SerializeField]
    private Image resultSprite = null;

    private void Awake() {
        EventManager.StartListening("ResultUpdate", On_ResultUpdate);
        EventManager.StartListening("CancelCraft", On_CancelCraft);
    }

    private void OnDestroy() {
        EventManager.StopListening("ResultUpdate", On_ResultUpdate);
        EventManager.StopListening("CancelCraft", On_CancelCraft);
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

    public void ActivateButton(Sprite _sprite) {
        GetComponent<Animator>().SetBool("PanelOpen", true);
        _button.interactable = true;

        resultSprite.enabled = true;
        resultSprite.sprite = _sprite;
    }

    public void DeactivateButton() {
        GetComponent<Animator>().SetBool("PanelOpen", false);
        _button.interactable = false;

        resultSprite.enabled = false;
        resultSprite.sprite = null;
    }
    
    #region Event Listeners

    public void On_ResultUpdate(EventParams _eventParams) {
        if (_eventParams.itemParam != null) {
            ActivateButton(_eventParams.itemParam.itemType.sprite);
        } else {
            DeactivateButton();
        }
    }

    public void On_CancelCraft(EventParams _eventParams) {
        DeactivateButton();
    }

    #endregion
}
