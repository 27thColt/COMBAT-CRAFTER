using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CancelCraftButton : MonoBehaviour {
    private Button _button;
    void Awake() {
        EventManager.StartListening("BStateChange", On_BStateChange);
    }

    void OnDestroy() {
        EventManager.StopListening("BStateChange", On_BStateChange);
    }

    void Start() {
        _button = GetComponent<Button>();
    }


    public void OnCancelCraftButton() {
        EventManager.TriggerEvent("CancelCraft", new EventParams());
        DeactivateButton();
    }

    private void ActivateButton() {
        GetComponent<Animator>().SetBool("PanelOpen", true);
        _button.interactable = true;
    }
    
    private void DeactivateButton() {
        GetComponent<Animator>().SetBool("PanelOpen", false);
        _button.interactable = false;
    }

    #region Event Listeners

    private void On_BStateChange(EventParams _eventParams) {
        if (BattleStateMachine.currentBState is EnemySelect) {
            ActivateButton();
        } else {
            try {
                DeactivateButton();
            } catch {
                
            }
        }
    }
    #endregion
}
