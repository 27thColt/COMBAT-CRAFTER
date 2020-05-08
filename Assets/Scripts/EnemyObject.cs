using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static BattleState;


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

        Debug.Log("Damage dealt: " + _damage + " | HP left: " + CurrentHP);
    }

    public void Die() {
        Destroy(gameObject);
    }

    #endregion

    public EnemyType enemyType;

    // The Gameobject that the enemy is using (used to be a sprite but now we use character RIGZ) ( 4/30/2020 7:33pm )
    public GameObject rig = null;

    // Values for if the enemy object is attacking / defending ( 5/1/2020 7:11pm )
    private bool _isDefending = false;
    private bool _isAttacking = false;

    private GameObject _tooltip = null;

    // Will determine if the tooltip automatically show / hide itself ( 4/24/2020 7:32pm )
    private bool _autoTooltip = true;

    // boolean for whenever the mouse is hovering over the enemy ( 4/26/2020 1:04am )
    private bool _mouseOver = false;

    private void Awake() {
        EventManager.StartListening("PlayerAttack", On_PlayerAttack);
        EventManager.StartListening("EnemyAttack", On_EnemyAttack);
        EventManager.StartListening("EnemyAttackAnimEnd", On_EnemyAttackAnimEnd);
        EventManager.StartListening("EnemyDefendAnimEnd", On_EnemyDefendAnimEnd);
    }

    private void Start() {
        if (GetComponentInChildren(typeof(ObjectAnimator.IObjectAnimator)) == null) {
            Debug.LogError(gameObject.name + " has no IObjectAnimator component attached.");
        }
    }

    private void OnDestroy() {
        EventManager.StopListening("PlayerAttack", On_PlayerAttack);
        EventManager.StopListening("EnemyAttack", On_EnemyAttack);
        EventManager.StopListening("EnemyAttackAnimEnd", On_EnemyAttackAnimEnd);
    }

    #region Functions

    public void SetEnemy(EnemyType _enemy) {
        enemyType = _enemy;
        MaxHP = _enemy.baseHP;
        CurrentHP = MaxHP;

        rig = Instantiate(_enemy.rig);
        rig.transform.SetParent(transform);
        rig.transform.localScale = new Vector3(2, 2, 2);
        rig.transform.localPosition = new Vector3(0, 0, 0);
        rig.transform.localRotation = Quaternion.Euler(0, 0, 0);    
    }

    public void SetAttacking(bool _bool) {
        _isAttacking = _bool;
    }

    public bool GetAttacking() {
        return _isAttacking;
    }

    public bool GetDefending() {
        return _isDefending;
    }

    #endregion

    // Will select an enemy WHEN it is enemy selection phase ( 2/29/2020 3:39pm )
    public void OnMouseDown() {
        if (currentState == Bstate.player_ENEMYSELECTION) {
            _isDefending = true;

            EventManager.TriggerEvent("EnemySelect", new EventParams(enemyType));
        }
    }
    
    #region Event Listeners

    // Fires when the damage has been calculated, only the selected enemy will be operated on ( 4/24/2020 5:29pm )
    public void On_PlayerAttack(EventParams _eventParams) {
        if (_eventParams.intParam1 != 0) {
            int _damage = _eventParams.intParam1;

            if (_isDefending) {
                StartCoroutine(TakeDamage(CurrentHP, _damage));

                // Animations ( 5/1/2020 5:18pm )
                if (CurrentHP - _damage > 0) {
                    EventManager.TriggerEvent("EnemyDefendAnim", new EventParams("Damaged"));
                } else {
                    EventManager.TriggerEvent("EnemyDefendAnim", new EventParams("Died"));
                }

                // Disables the tooltip from following the mouse ( 4/26/2020 1:28am )
                _tooltip.GetComponent<EnemyTooltip>().SetMouseFollow(false);

                // There is no CreateTooltip function here because the logic is that a tooltip is already existing if they clicked on the enemy ( 4/26/2020 1:28am )

                _autoTooltip = false;
            }
        } else {
            Debug.LogError("EventParams with non-zero intParam1 expected.");
        }
        
    }   

    // Fires when the enemy attacks ( 5/7/2020 2:39pm )
    public void On_EnemyAttack(EventParams _eventParams) {
        if (_eventParams.intParam1 != 0) {
            if (_isAttacking) {
            EventParams _ep = new EventParams("Attack");
            _ep.intParam1 = _eventParams.intParam1; // Transfers damage done so that it can be called later ( 5/7/2020 5:20 )

            EventManager.TriggerEvent("EnemyAttackAnim", _ep);
            }
        } else {
            Debug.LogError("EventParams with non-zero intParam1 expected.");
        }
    }

    // Called by Enemy Object Animator, fires when the attack animation is over ( 5/7/2020 2:54pm )
    public void On_EnemyAttackAnimEnd(EventParams _eventParams) {
        if (_isAttacking) {
            SetAttacking(false);
            FinishCurrentState(Bstate.enemy_ATTACK);
        }
        
    }

    // Fires when the Defend animation is over ( 5/7/2020 2:54pm )
    public void On_EnemyDefendAnimEnd(EventParams _eventParams) {
        if (_isDefending) {
            // Following chunk of code refers to the tooltip ( 5/1/2020 1:04pm )
            if (_tooltip != null) {
                _autoTooltip = true;
                _tooltip.GetComponent<EnemyTooltip>().SetMouseFollow(true);

                if (!_mouseOver) {
                    Tooltip.DeleteTooltip(_tooltip);
                    _tooltip = null;
                }
            }
            

            // If the enemy fucking DIED ( 5/1/2020 1:59pm )
            if (CurrentHP <= 0) {
                WaveManager.instance.enemyList.Remove(GetComponent<EnemyObject>());
                Debug.Log("Enemy died at " + Time.time);
                Die();
            }

            _isDefending = false;

            FinishCurrentState(Bstate.player_ATTACK);
        }
    }

    #endregion

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
