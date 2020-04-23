using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static BattleStateManager;

/* 12/26/2019 10:48pm - Enemy Display
 * Attached onto the enemy prefab. Takes information from the item scriptable object to be used in the UI.
 * 
 * lmao just copied that from ItemDisplay script ^^. Also, now I'm listening to Pendulum's Witchcraft. Good shit.
 * 
 * Deals more than just UI,, deals with all the logic related to the enemy gameobject itself ( 12/27/2019 1:21pm )
 */
public class EnemyScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public EnemyType enemy;
    //public TextMeshProUGUI textMesh;
    public SpriteRenderer spriteRenderer;

    public int maxHP, currentHP;

    private bool _selected = false;

    // event for when an enemy has been selected ( 12/27/2019 11:38am )
    public delegate void enemySelect(EnemyType _enemy);
    public static event enemySelect OnEnemySelected = delegate {
        print("Enemy has been selected!");
    };

    // event for when enemy has been hovered over (shows tooltip) ( 2/25/2020 9:49pm )
    public delegate void enemyTooltip(EnemyScript _enemy, EnemyType _enemyType);
    public static event enemyTooltip OnEnemyHover;

    public delegate void exitHover();
    public static event exitHover OnEnemyExit;

    private void Awake() {
        BattleManager.OnDamagePerformed += DamageListener;
    }

    public void SetEnemy(EnemyType _enemy) {
        enemy = _enemy;
        maxHP = _enemy.baseHP;
        currentHP = _enemy.baseHP;
        spriteRenderer.sprite = _enemy.sprite;
    }

    public void TakeDamage(int _damage) {
        currentHP -= _damage;
    }

    // Will select an enemy WHEN it is enemy selection phase ( 2/29/2020 3:39pm )
    public void OnMouseDown() {
        if (currentState == Battlestate.player_ENEMYSELECTION) {
            _selected = true;

            OnEnemySelected(enemy);
        }
    }

    public void DamageListener(int _damage) {
        if (_selected) {
            TakeDamage(_damage);
            Debug.Log("Damage dealt: " + _damage + " | HP left: " + currentHP);
        }

        SetCurrentState(Battlestate.enemy_ATTACK);
    }

    #region OnPointerEnter & Exit
    public void OnPointerEnter(PointerEventData eventData) {
        OnEnemyHover(GetComponent<EnemyScript>(), enemy);
    }

    public void OnPointerExit(PointerEventData eventData) {
        OnEnemyExit();
    }

    #endregion

}
