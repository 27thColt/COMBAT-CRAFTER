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

public class TooltipScript : MonoBehaviour {
    public TextMeshProUGUI displayText;
    public GameObject iconsGrid;
    public GameObject iconPrefab;

    void Awake() {
        // Focuses on enemy tooltips ( 4/23/2020 2:34pm )
        EnemyScript.OnEnemyHover += ShowEnemyTooltip;
        EnemyScript.OnEnemyExit += HideToolTip;
    }

    private void Start() {
        gameObject.SetActive(false);
        
    }

    private void Update() {

        /* Don't really wanna call the update function so if theres a better way to do this then I will replace it with that ( 4/23/2020 2:35pm )
         * 
         * Basically it just redraws the position of tooltip window
         */ 
        if (gameObject.activeSelf)
            transform.position = Input.mousePosition + (new Vector3 (0, 20, 0));
    }

    #region Event Listeners

    void ShowEnemyTooltip(Enemy _enemy) {
        iconsGrid.SetActive(true);
        displayText.text = _enemy.enemyName + "<br><size=18>Vulnerabilties:</size>";


        foreach (Item _item in _enemy.vulnerabilities) {
            GameObject icon = Instantiate(iconPrefab, iconsGrid.transform);

            if (EnemyInventory.instance.CheckInvFor(_enemy) != null) {
                if (EnemyInventory.instance.CheckInvFor(_enemy).vulnerabilities.Contains(_item.ID)) {
                    icon.GetComponent<Image>().sprite = _item.sprite;
                }
            }
        }

        gameObject.SetActive(true);
    }

    void HideToolTip() {

        // Deletes all icons upon disable ( 4/23/2020 2:45pm )
        foreach (Transform child in iconsGrid.transform) {
            Destroy(child.gameObject);
        }

        iconsGrid.SetActive(false);
        gameObject.SetActive(false);
    }

    #endregion
}
