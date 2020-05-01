using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static BattleState;
using System;
using UnityEditor;
using System.Threading.Tasks;

/* 12/26/2019 10:48pm - Enemy Display
 * Attached onto the enemy prefab. Takes information from the item scriptable object to be used in the UI.
 * 
 * lmao just copied that from ItemDisplay script ^^. Also, now I'm listening to Pendulum's Witchcraft. Good shit.
 * 
 * Deals more than just UI,, deals with all the logic related to the enemy gameobject itself ( 12/27/2019 1:21pm )
 */
public class EnemyObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IHealthPoints {

    #region IHealthPoints
    public int MaxHP { get; set; }

    public int CurrentHP { get; set; }


    // This is a coroutine so that the HP can update in real time, and not just automatically ( 4/24/2020 5:25 pm)
    public IEnumerator TakeDamage(int _full, int _damage) {
        yield return new WaitForSeconds(0.4f);

        while (CurrentHP > _full - _damage) {
            CurrentHP -= 1;

            yield return new WaitForEndOfFrame();
        }

        // Following Code waits for the damaged/died animation to play out first ( 5/1/2020 5:09pm )
        yield return new WaitForSeconds(0.3f); // A small delay because the animation does not start immediately ( 5/1/2020 5:09pm )
        yield return new WaitForSeconds(_anim.GetCurrentAnimatorClipInfo(0)[0].clip.length - (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime * _anim.GetCurrentAnimatorClipInfo(0)[0].clip.length) - 0.3f);

        // Following chunk of code refers to the tooltip ( 5/1/2020 1:04pm )
        _autoTooltip = true;
        _tooltip.GetComponent<EnemyTooltip>().SetMouseFollow(true);

        if (!_mouseOver) {
            Tooltip.DeleteTooltip(_tooltip);
            _tooltip = null;
        }

        Debug.Log("Damage dealt: " + _damage + " | HP left: " + CurrentHP);

        // If the enemy fucking DIED ( 5/1/2020 1:59pm )
        if (CurrentHP <= 0) {
            WaveManager.instance.enemyList.Remove(GetComponent<EnemyObject>());
            Debug.Log("Enemy died at" + Time.time);
            Die();
        }

        FinishCurrentState(Bstate.player_ATTACK);
        // FinishCurrentState(Bstate.playerattack_ANIMATE);
    }

    public void Die() {
        Destroy(gameObject);
    }

    #endregion

    public EnemyType enemyType;

    // The Gameobject that the enemy is using (used to be a sprite but now we use character RIGZ) ( 4/30/2020 7:33pm )
    private GameObject _rig = null;
    private Animator _anim = null;

    // Values for if the enemy object is attacking / defending ( 5/1/2020 7:11pm )
    private bool _isDefending = false;
    private bool _isAttacking = false;

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
        BattleManager.OnPlayerAttack += DamageListener;
        BattleManager.OnEnemyAttack += AttackListener;
    }

    private void OnDestroy() {
        BattleManager.OnPlayerAttack -= DamageListener;
        BattleManager.OnEnemyAttack -= AttackListener;
    }

    // Coroutine for when the sprite needs to flash something (taking damage),, this takes into account that we are using RIGZ and not just stupid sprites ( 5/1/2020 1:34am )
    private IEnumerator SpriteFlash(int _loops, float _delay) {
        SpriteRenderer[] _sprites = _rig.GetComponentsInChildren<SpriteRenderer>();


        for (int i = 1; i <= _loops + 1; i++) {
            if (i % 2 == 0) {
                foreach (SpriteRenderer _sprite in _sprites) {
                    _sprite.color = new Color(1f, 1f, 1f, 0.7f); // White color, a bit transparent ( 5/1/2020 1:35am )
                }
            } else {
                foreach (SpriteRenderer _sprite in _sprites) {
                    _sprite.color = new Color(1f, 1f, 1f, 1f); // White color, opacity 100% ( 5/1/2020 1:35am )
                }
            }

            yield return new WaitForSeconds(_delay);
        }

        // Return to normal just in case ( 5/1/2020 1:35am )
        foreach (SpriteRenderer _sprite in _sprites) {
            _sprite.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    private IEnumerator AttackAnim() {
        _anim.SetTrigger("Attack");

        // Following Code waits for the damaged/died animation to play out first ( 5/1/2020 5:09pm )
        yield return new WaitForSeconds(0.3f); // A small delay because the animation does not start immediately ( 5/1/2020 5:09pm )
        yield return new WaitForSeconds(_anim.GetCurrentAnimatorClipInfo(0)[0].clip.length - (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime * _anim.GetCurrentAnimatorClipInfo(0)[0].clip.length) - 0.3f);


        FinishCurrentState(Bstate.enemy_ATTACK);
    }

    #region Functions

    public void SetEnemy(EnemyType _enemy) {
        enemyType = _enemy;
        MaxHP = _enemy.baseHP;
        CurrentHP = MaxHP;

        _rig = Instantiate(_enemy.rig);
        _rig.transform.SetParent(transform);
        _rig.transform.localScale = new Vector3(2, 2, 2);
        _rig.transform.localPosition = new Vector3(0, 0, 0);
        _rig.transform.localRotation = Quaternion.Euler(0, 0, 0);

        try {
            _anim = _rig.GetComponent<Animator>();
        } catch {
            Debug.Log(_rig.name + " does not have Animator Component!");
        }
        
    }

    public void SetAttacking(bool _bool) {
        _isAttacking = _bool;
    }

    #endregion

    // Will select an enemy WHEN it is enemy selection phase ( 2/29/2020 3:39pm )
    public void OnMouseDown() {
        if (currentState == Bstate.player_ENEMYSELECTION) {
            _isDefending = true;

            OnEnemySelected(enemyType);
        }
    }

    // Fires when the damage has been calculated, only the selected enemy will be operated on ( 4/24/2020 5:29pm )
    public void DamageListener(int _damage) {
        if (_isDefending) {
            StartCoroutine(TakeDamage(CurrentHP, _damage));
            StartCoroutine(SpriteFlash(3, 0.06f));

            // Animations ( 5/1/2020 5:18pm )
            if (CurrentHP - _damage > 0) {
                try {
                    _anim.SetTrigger("Damaged");
                } catch {
                    Debug.Log(_rig.name + " does not have Animator Component and/or cannot performed Damaged action!");
                }
            } else {
                try {
                    _anim.SetTrigger("Died");
                } catch {
                    Debug.Log(_rig.name + " does not have Animator Component and/or cannot performed Died action!");
                }
            }

            // Disables the tooltip from following the mouse ( 4/26/2020 1:28am )
            _tooltip.GetComponent<EnemyTooltip>().SetMouseFollow(false);

            // There is no CreateTooltip function here because the logic is that a tooltip is already existing if they clicked on the enemy ( 4/26/2020 1:28am )

            _autoTooltip = false;
            _isDefending = false;
        }
    }   

    // Fires when the enemy attacks
    public void AttackListener(int _damage) {
        if (_isAttacking) {
            StartCoroutine(AttackAnim());
        }
    }

    #region OnPointerEnter & Exit
    public void OnPointerEnter(PointerEventData eventData) {
        _mouseOver = true;

        if (_autoTooltip) {
            _tooltip = Tooltip.CreateTooltip(GetComponent<EnemyObject>());
            _tooltip.GetComponent<EnemyTooltip>().SetTooltip(GetComponent<EnemyObject>());
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
