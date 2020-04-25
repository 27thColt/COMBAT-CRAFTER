using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/* 2/25/2020 9:51pm - Tooltip Script
 * All about the lil' tooltips that pop up above the mouse
 * 
 */

public class EnemyTooltip : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI nameText = null;

    [SerializeField]
    private GameObject iconsGrid = null;

    [SerializeField]
    private GameObject iconPrefab = null;

    [SerializeField]
    private GameObject hpBar = null;

    private EnemyScript _currentEnemy;

    // Will determine if the tooltip follows the mouse or not ( 4/24/2020 5:23pm )
    private bool _followMouse = true;

    private void Update() {

        /* Don't really wanna call the update function so if theres a better way to do this then I will replace it with that ( 4/23/2020 2:35pm )
         * 
         * Basically it just redraws the position of tooltip window
         */ 
        if (_followMouse) {
            transform.position = Input.mousePosition + (new Vector3(0, 20, 0));
        }
            

        // Updates healthbar if enemy is hovered over ( 4/24/2020 5:27pm )
        hpBar.GetComponent<Image>().fillAmount = (float)_currentEnemy.currentHP / (float)_currentEnemy.maxHP;
    }

    // Main Show and Hide functions ( 4/24/2020 5:33pm )

    #region Functions
    public void SetTooltip(EnemyScript _enemy) {
        _currentEnemy = _enemy;


        nameText.text = _enemy.enemyType.enemyName;
        hpBar.GetComponent<Image>().fillAmount = (float)_enemy.currentHP / (float)_enemy.maxHP;

        foreach (ItemType _item in _enemy.enemyType.vulnerabilities) {
            GameObject icon = Instantiate(iconPrefab, iconsGrid.transform);

            if (EnemyInventory.instance.CheckInvFor(_enemy.enemyType) != null) {
                if (EnemyInventory.instance.CheckInvFor(_enemy.enemyType).vulnerabilities.Contains(_item.ID)) {
                    icon.GetComponent<Image>().sprite = _item.sprite;
                }
            }
        }
    }

    public void SetMouseFollow(bool _x) {
        _followMouse = _x;
    }

    #endregion

    /*
     * DEPRECATED STUFF ( 4/25/2020 6:35pm )
     * 
     * 
    private void HideToolTip() {
        // Deletes all icons upon disable ( 4/23/2020 2:45pm )
        foreach (Transform child in iconsGrid.transform) {
            Destroy(child.gameObject);
        }

        iconsGrid.SetActive(false);

        _currentEnemy = null;

        gameObject.SetActive(false);
    }


    
    #region Event Listeners



    // forceHide will hide the tooltip no matter what ( 4/24/2020 7:42pm )
    private void TooltipHideListener(bool _forceHide) {
        if (!_forceHide) {
            if (_autoHide)
                HideToolTip();
        } else {
            HideToolTip();
        }
        
    }


    private void TooltipShowListener(EnemyScript _enemy, bool _mouseFollow) {
        if (!_mouseFollow) {
            _autoHide = false;
            _followMouse = false;

            SetTooltip(_enemy);
            transform.position = _camera.WorldToScreenPoint(_enemy.gameObject.transform.position);
        } else {
            if (!gameObject.activeSelf)
                SetTooltip(_enemy);
        }
    }

    #endregion*/
}
