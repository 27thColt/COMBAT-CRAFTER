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
public class EnemyEntity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IHealthPoints {

    #region IHealthPoints
    public int MaxHP { get; set; }

    public int CurrentHP { get; set; }

    public int HPCache { get; set; } = 0;

    public void TakeDamage(int _damage) {
        HPCache = CurrentHP;
        CurrentHP -= _damage;

        Debug.Log("Damage Dealt: " + _damage + " | HP Left: " + CurrentHP);
    }

    public void Die() {
        Destroy(gameObject);
    }

    #endregion

    public EnemyType enemyType;

    // The Gameobject that the enemy is using (used to be a sprite but now we use character RIGZ) ( 4/30/2020 7:33pm )
    public GameObject rig = null;
    private EntityAnimator.IEntityAnimator _entityAnimator = null;

    private EnemyInfoPanel _infoPanel = null;

    // Values for if the enemy object is attacking / defending ( 5/1/2020 7:11pm )
    private bool _isDefending = false;
    private bool _isAttacking = false;

    void Awake() {
        EventManager.StartListening("PlayerAttack", On_PlayerAttack);
        EventManager.StartListening("PlayerAttackAnimEnd", On_PlayerAttackAnimEnd);
        EventManager.StartListening("EnemyAttack", On_EnemyAttack);
        EventManager.StartListening("EnemyAttackAnimEnd", On_EnemyAttackAnimEnd);
        EventManager.StartListening("EnemyDefendAnimEnd", On_EnemyDefendAnimEnd);
    }

    void OnDestroy() {
        EventManager.StopListening("PlayerAttack", On_PlayerAttack);
        EventManager.StopListening("PlayerAttackAnimEnd", On_PlayerAttackAnimEnd);
        EventManager.StopListening("EnemyAttack", On_EnemyAttack);
        EventManager.StopListening("EnemyAttackAnimEnd", On_EnemyAttackAnimEnd);
        EventManager.StopListening("EnemyDefendAnimEnd", On_EnemyDefendAnimEnd);
    }

    void Start() {
        try {
            _infoPanel = FindObjectOfType<EnemyInfoPanel>();
        } catch {
            Debug.LogError("Enemy Info Panel cannot be found!");
        }
        
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

        _entityAnimator = rig.gameObject.GetComponent<EnemyEntityAnimator>();
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

            EventManager.TriggerEvent("EnemySelect", new EventParams(GetComponent<EnemyEntity>()));
        }
    }
    
    #region Event Listeners

    private void On_PlayerAttack(EventParams _eventParams) {
        if (_eventParams.intParam1 != 0) {
            if (_isDefending) TakeDamage(_eventParams.intParam1);
        } else {
            Debug.LogError("EventParams with non-zero intParam1 expected.");
        }
    }

    // Fires when the damage has been calculated, only the selected enemy will be operated on ( 4/24/2020 5:29pm )
    private void On_PlayerAttackAnimEnd(EventParams _eventParams) {
        int _damage = _eventParams.intParam1;

        if (_isDefending) {
            StartCoroutine(_infoPanel.GetComponent<EnemyInfoPanel>().hpBar.GetComponent<HPBar>().AnimateDamage(MaxHP, HPCache, CurrentHP));
                
            HPCache = 0;

            // Animations ( 5/1/2020 5:18pm )
            if (CurrentHP > 0) {
                _entityAnimator.DoDefendAnim("Damaged");
            } else {
                _entityAnimator.DoDefendAnim("Died");
            }
        }
        
    }   

    // Fires when the enemy attacks ( 5/7/2020 2:39pm )
    private void On_EnemyAttack(EventParams _eventParams) {
        if (_isAttacking) {
            _entityAnimator.DoAnim("Attack");
        }
    }

    // Called by Enemy Object Animator, fires when the attack animation is over ( 5/7/2020 2:54pm )
    private void On_EnemyAttackAnimEnd(EventParams _eventParams) {
        if (_isAttacking) {
            SetAttacking(false);
            FinishCurrentState(Bstate.enemy_ATTACK);
        }
        
    }

    // Fires when the Defend animation is over ( 5/7/2020 2:54pm )
    private void On_EnemyDefendAnimEnd(EventParams _eventParams) {
        if (_isDefending) {
            // If the enemy fucking DIED ( 5/1/2020 1:59pm )
            if (CurrentHP <= 0) {
                WaveManager.instance.enemyList.Remove(GetComponent<EnemyEntity>());
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
        EventManager.TriggerEvent("EnemyHoverEnter", new EventParams(GetComponent<EnemyEntity>()));
    }

    public void OnPointerExit(PointerEventData eventData) {
        EventManager.TriggerEvent("EnemyHoverExit", new EventParams(GetComponent<EnemyEntity>()));
    }

    #endregion

}
