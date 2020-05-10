using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectAnimator;

/*  5/6/2020 5:36pm - Enemy Object Animator
    Handles the logic side of the enemy animations

*/

[RequireComponent(typeof(Animator))]
public class EnemyObjectAnimator : MonoBehaviour, IObjectAnimator {
    private EnemyObject _enemyObject = null;
    private Animator _anim = null;

    void Awake() {
        EventManager.StartListening("EnemyDefendAnim", On_DefendAnim);
        EventManager.StartListening("EnemyAttackAnim", On_AttackAnim);
    }

    void OnDestroy() {
        EventManager.StopListening("EnemyDefendAnim", On_DefendAnim);
        EventManager.StopListening("EnemyAttackAnim", On_AttackAnim);
    }

    void Start() {
        _enemyObject = GetComponentInParent<EnemyObject>();
        
        _anim = GetComponent<Animator>();
    }

    // Fires when the attack animation is done. THIS IS REFERENCED THROUGH AN ANIMATION EVENT ( 5/8/2020 3:52pm )
    public void AttackAnimEnd() {
        EventManager.TriggerEvent("EnemyAttackAnimEnd", new EventParams());
    }

    #region IObjectAnimator

    public void On_DefendAnim(EventParams _eventParams) {
        if (_eventParams.stringParam1 != null) {
            if (_enemyObject.GetDefending()) {
                try {
                    StartCoroutine(SpriteFlash(_anim.gameObject, 3, 0.06f));
                    StartCoroutine(DoAfterAnim(_eventParams.stringParam1, _anim, () => {
                        EventManager.TriggerEvent("EnemyDefendAnimEnd", new EventParams());
                    }));
                }  catch {
                    Debug.LogError(_anim.gameObject.name + " does not have Animator Component and/or cannot performed Damaged action!");
                }
            }
        } else {
            Debug.LogError("EventParams with non-null stringParam1 expected.");
        }
    }

    public void On_AttackAnim(EventParams _eventParams) {
        if (_eventParams.stringParam1 != null) {
            if (_enemyObject.GetAttacking()) {
                // _eventParams.intParam1 carries the calculated damage from BattleManager. This is then passed on to PlayerObject ( 5/7/2020 5:23pm )
                try {
                    StartCoroutine(DoAfterAnim(_eventParams.stringParam1, _anim, () => {    }));
                }  catch {
                    Debug.LogError(_anim.gameObject.name + " does not have Animator Component and/or cannot performed Damaged action!");
                }
            }
        } else {
            Debug.LogError("EventParams with non-null stringParam1 expected.");
        }
    }

    #endregion
}
