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
    }

    private void OnDestroy() {
        EventManager.StopListening("ResultUpdate", On_ResultUpdate);
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

    public void On_ResultUpdate(EventParams _eventParams) {
        if (_eventParams.itemParam != null) {
            GetComponent<Animator>().SetBool("PanelOpen", true);
            _button.interactable = true;

            resultSprite.enabled = true;
            resultSprite.sprite = _eventParams.itemParam.itemType.sprite;
        } else {
            GetComponent<Animator>().SetBool("PanelOpen", false);
            _button.interactable = false;

            resultSprite.enabled = false;
            resultSprite.sprite = null;

        }
    }

    #endregion
}
