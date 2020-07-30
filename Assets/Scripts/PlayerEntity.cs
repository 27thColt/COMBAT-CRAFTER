using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleStateMachine;

/* 4/26/2020 1:43am - Player Object 
 * Oh my god I can already tell this will be good for me
 * 
 * Okay, this manages all player related stats-- as time goes on I will have to delegate any
 * abstract stat based stuff to another script and animations to this one
 * 
 * The term 'object' is used to refers to a physical gameobject that can be seen in the scene rather than the data type (at least the way I use it)
 */ 
public class PlayerEntity : MonoBehaviour, IHealthPoints {
    #region IHealthPoints

    public int MaxHP { get; set; } = 60;
    public int CurrentHP { get; set; }

    public int HPCache { get; set; } = 0;

    public void TakeDamage(int _damage) {
        HPCache = CurrentHP;
        CurrentHP -= _damage;   
    }
    
    public void Die() {
        return;
    }

    #endregion

    [SerializeField]
    private GameObject _hpBar = null;
    private EntityAnimator.IEntityAnimator _entityAnimator = null;

    void Awake() {
        EventManager.StartListening("PlayerAttack", On_PlayerAttack);
        EventManager.StartListening("EnemyAttackAnimEnd", On_EnemyAttackAnimEnd);
        EventManager.StartListening("EnemyAttack", On_EnemyAttack);
        EventManager.StartListening("PlayerHeal", On_PlayerHeal);
    }

    private void OnDestroy() {
        EventManager.StopListening("PlayerAttack", On_PlayerAttack);
        EventManager.StopListening("EnemyAttackAnimEnd", On_EnemyAttackAnimEnd);
        EventManager.StopListening("EnemyAttack", On_EnemyAttack);
        EventManager.StopListening("PlayerHeal", On_PlayerHeal);
    }

    void Start() {
        if (GetComponentInChildren(typeof(EntityAnimator.IEntityAnimator)) == null) {
            Debug.LogError(gameObject.name + " has no IObjectAnimator component attached.");
        } else {
            _entityAnimator = GetComponentInChildren<PlayerEntityAnimator>();
        }

        // Following shit sets up the health and stuff ( 4/27/2020 2:05pm )
        CurrentHP = MaxHP;
        _hpBar.GetComponent<HPBar>().SetObject(gameObject);
    }

    #region Event Listeners

    private void On_PlayerHeal(EventParams _eventParams) {
        if (_eventParams.intParam1 != 0) {
            int _regen = _eventParams.intParam1;

            if (CurrentHP + _regen > MaxHP) {
                HPCache = CurrentHP;
                CurrentHP = MaxHP;
            } else {
                TakeDamage(-_regen);
            }

            StartCoroutine(_hpBar.GetComponent<HPBar>().AnimateDamage(MaxHP, HPCache, CurrentHP));
            HPCache = 0;

            _entityAnimator.DoAnim("Heal");
        } else {
            Debug.LogError("EventParams with non-zero intParam1 expected.");
        }
    }

    private void On_PlayerAttack(EventParams _eventParams) {
        _entityAnimator.DoAnim("Attack");
    }

    private void On_EnemyAttack(EventParams _eventParams) {
        if (_eventParams.intParam1 != 0) {
            TakeDamage(_eventParams.intParam1);
        } else {
            Debug.LogError("EventParams with non-zero intParam1 expected.");
        }
    }

    // Is performed after the enemy attack animation ends ( 5/7/2020 5:22pm )
    private void On_EnemyAttackAnimEnd(EventParams _eventParams) {
        StartCoroutine(_hpBar.GetComponent<HPBar>().AnimateDamage(MaxHP, HPCache, CurrentHP));
        HPCache = 0;

        if (CurrentHP > 0) {
            _entityAnimator.DoDefendAnim("Damaged");
        } else {
            _entityAnimator.DoDefendAnim("Died");
        }
    }

    #endregion
}
