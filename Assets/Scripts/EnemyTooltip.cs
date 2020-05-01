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
    private TextMeshProUGUI _nameText = null;

    [SerializeField]
    private GameObject _iconsGrid = null;

    [SerializeField]
    private GameObject _iconPrefab = null;

    [SerializeField]
    private GameObject _hpBar = null;

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
    }

    // Main Show and Hide functions ( 4/24/2020 5:33pm )

    #region Functions
    public void SetTooltip(EnemyObject _enemy) {
        _nameText.text = _enemy.enemyType.enemyName;
        _hpBar.GetComponent<HPBar>().SetObject(_enemy.gameObject);

        foreach (ItemType _item in _enemy.enemyType.vulnerabilities) {
            GameObject icon = Instantiate(_iconPrefab, _iconsGrid.transform);

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
}
