using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static BattleState;
using System;
using UnityEditor;

/* 12/26/2019 10:48pm - Enemy Display
 * Attached onto the enemy prefab. Takes information from the item scriptable object to be used in the UI.
 * 
 * lmao just copied that from ItemDisplay script ^^. Also, now I'm listening to Pendulum's Witchcraft. Good shit.
 * 
 * Deals more than just UI,, deals with all the logic related to the enemy gameobject itself ( 12/27/2019 1:21pm )
 */
public class EnemyScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public EnemyType enemyType;
    //public TextMeshProUGUI textMesh;
    public SpriteRenderer spriteRenderer;

    public int maxHP, currentHP;

    private bool _selected = false;

    private GameObject _tooltip = null;

    // Will determine if the tooltip automatically show / hide itself ( 4/24/2020 7:32pm )
    private bool _autoTooltip = true;

    // boolean for whenever the mouse is hovering over the enemy ( 4/26/2020 1:04am )
    private bool _mouseOver = false;

    #region Delegates

    // event for when an enemy has been selected ( 12/27/2019 11:38am )
    public delegate void enemySelect(EnemyType _enemy);
    public static event enemySelect OnEnemySelected;

    #endregion

    private void Awake() {
        BattleManager.OnDamagePerformed += DamageListener;
    }

    #region Coroutines

    // This is a coroutine so that the HP can update in real time, and not just automatically ( 4/24/2020 5:25 pm)
    private IEnumerator TakeDamage(int _full, int _damage) {
        yield return new WaitForSeconds(0.4f); 

        while (currentHP > _full - _damage) {
            currentHP -= 1;

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.4f);

        _autoTooltip = true;
        _tooltip.GetComponent<EnemyTooltip>().SetMouseFollow(true);

        if (!_mouseOver) {
            Tooltip.DeleteTooltip(_tooltip);
            _tooltip = null;
        }

        Debug.Log("Damage dealt: " + _damage + " | HP left: " + currentHP);
    }

    #endregion

    public void SetEnemy(EnemyType _enemy) {
        enemyType = _enemy;
        maxHP = _enemy.baseHP;
        currentHP = _enemy.baseHP;
        spriteRenderer.sprite = _enemy.sprite;
    }

    // Will select an enemy WHEN it is enemy selection phase ( 2/29/2020 3:39pm )
    public void OnMouseDown() {
        if (currentState == Bstate.player_ENEMYSELECTION) {
            _selected = true;

            OnEnemySelected(enemyType);
        }
    }

    // Fires when the damage has been calculated, only the selected enemy will be operated on ( 4/24/2020 5:29pm )
    public void DamageListener(int _damage) {
        if (_selected) {
            SetCurrentState(Bstate.playerattack_ANIMATE);

            StartCoroutine(TakeDamage(currentHP, _damage));

            // Disables the tooltip from following the mouse ( 4/26/2020 1:28am )
            _tooltip.GetComponent<EnemyTooltip>().SetMouseFollow(false);

            // There is no CreateTooltip function here because the logic is that a tooltip is already existing if they clicked on the enemy ( 4/26/2020 1:28am )

            _autoTooltip = false;
            _selected = false;
        }
    }

    #region OnPointerEnter & Exit
    public void OnPointerEnter(PointerEventData eventData) {
        _mouseOver = true;

        if (_autoTooltip) {
            _tooltip = Tooltip.CreateTooltip(GetComponent<EnemyScript>());
            _tooltip.GetComponent<EnemyTooltip>().SetTooltip(GetComponent<EnemyScript>());
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        _mouseOver = false;

        if (_autoTooltip) {
            Tooltip.DeleteTooltip(_tooltip);
            _tooltip = null;
        } 
    }

    #endregion

}
