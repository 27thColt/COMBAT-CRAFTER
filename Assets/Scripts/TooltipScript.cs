using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 2/25/2020 9:51pm - Tooltip Script
 * All about the lil' tooltips that pop up above the mouse
 * 
 */
 
public class TooltipScript : MonoBehaviour {
    public TextMeshProUGUI displayText;
    public GameObject iconsGrid;

    void Awake() {
        EnemyScript.OnEnemyHover += ShowEnemyTooltip;
        EnemyScript.OnEnemyExit += HideToolTip;
    }

    private void Start() {
        gameObject.SetActive(false);
        
    }

    private void Update() {
        if (gameObject.activeSelf)
            transform.position = Input.mousePosition + (new Vector3 (0, 20, 0));
    }

    #region Event Listeners

    void ShowEnemyTooltip(Enemy _enemy) {
        iconsGrid.SetActive(true);
        displayText.text = _enemy.enemyName + "<br><size=18>Vulnerabilties:</size>";

        gameObject.SetActive(true);
    }

    void HideToolTip() {
        iconsGrid.SetActive(false);
        gameObject.SetActive(false);
    }

    #endregion
}
