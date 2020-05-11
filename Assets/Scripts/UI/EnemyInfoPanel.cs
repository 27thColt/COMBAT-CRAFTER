using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*  5/10/2020 1:12pm - Enemy Info Panel
    I think I will ultimately be replacing the enemy tooltip with this panel.
    Just gonn copy and paste some code and all

*/
public class EnemyInfoPanel : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _nameText = null;

    [SerializeField]
    private GameObject _iconsGrid = null;

    [SerializeField]
    private GameObject _iconPrefab = null;

    public GameObject hpBar = null; // Referenced through Unity Editor ( 5/9/2020 3:58pm )

    private bool _autoDisable = true; // Will automatically disable the panel if set to true ( 5/10/2020 1:44pm )
    private bool _mouseOn = false;

    // Positions for when the enemy info panel is enabled and disabled ( 5/10/2020 5:02pm )
    private Vector2 _onPos = new Vector2(-140.5f, -77.29f);
    private Vector2 _offPos = new Vector2(142, -77.29f);

    void Awake() {
        EventManager.StartListening("EnemySelect", On_EnemySelect);
        EventManager.StartListening("EnemyHoverEnter", On_EnemyHoverEnter);
        EventManager.StartListening("EnemyHoverExit", On_EnemyHoverExit);
        EventManager.StartListening("EnemyDefendAnimEnd", On_EnemyDefendAnimEnd);

        gameObject.GetComponent<RectTransform>().anchoredPosition = _offPos;
    }

    void OnDestroy() {
        EventManager.StopListening("EnemySelect", On_EnemySelect);
        EventManager.StopListening("EnemyHoverEnter", On_EnemyHoverEnter);
        EventManager.StopListening("EnemyHoverExit", On_EnemyHoverExit);
        EventManager.StopListening("EnemyDefendAnimEnd", On_EnemyDefendAnimEnd);
    }

    
    private void SetInfo(EnemyObject _enemy) {
        _nameText.text = _enemy.enemyType.enemyName;
        hpBar = GetComponentInChildren<HPBar>().gameObject;
        hpBar.GetComponent<HPBar>().SetObject(_enemy.gameObject);

        foreach (ItemType _item in _enemy.enemyType.vulnerabilities) {
            GameObject icon = Instantiate(_iconPrefab, _iconsGrid.transform);

            if (EnemyInventory.instance.CheckInvFor(_enemy.enemyType) != null) {
                if (EnemyInventory.instance.CheckInvFor(_enemy.enemyType).vulnerabilities.Contains(_item.ID)) {
                    icon.GetComponent<Image>().sprite = _item.sprite;
                }
            }
        }
    }

    private void DisablePanel() {
        foreach (Transform _child in _iconsGrid.transform) {
            Destroy(_child.gameObject);
        }
            
        gameObject.GetComponent<RectTransform>().anchoredPosition = _offPos;
    }

    #region Event Listeners

    private void On_EnemySelect(EventParams _eventParams) {
        if (_eventParams.componentParams != null) {
            if (_eventParams.componentParams is EnemyObject) {
                if (!gameObject.activeSelf) {
                    SetInfo(_eventParams.componentParams as EnemyObject);
                    gameObject.GetComponent<RectTransform>().anchoredPosition = _onPos;
                }
                

                _autoDisable = false;
                
            }
        } else {
            Debug.LogError("Eventparams containing non-null componentParams expected!");
        }
    }

    private void On_EnemyDefendAnimEnd(EventParams _eventParams) {
        _autoDisable = true;

        if (!_mouseOn) {
            DisablePanel();
        }
    }

    // The following 2 functions are for when the mouse hovers over an enemy ( 5/10/2020 1:22pm )
    private void On_EnemyHoverEnter(EventParams _eventParams) {
        if (_eventParams.componentParams != null) {
            if (_eventParams.componentParams is EnemyObject) {
                SetInfo(_eventParams.componentParams as EnemyObject);
                gameObject.GetComponent<RectTransform>().anchoredPosition = _onPos;

                _mouseOn = true;
            }
        } else {
            Debug.LogError("Eventparams containing non-null componentParams expected!");
        }
    }

    private void On_EnemyHoverExit(EventParams _eventParams) {
        if (_autoDisable) {
            DisablePanel();
            
        }

        _mouseOn = false;
    }

    #endregion

}
