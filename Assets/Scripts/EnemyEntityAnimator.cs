using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityAnimator;

/*  5/6/2020 5:36pm - Enemy Object Animator
    Handles the logic side of the enemy animations

*/

[RequireComponent(typeof(Animator))]
public class EnemyEntityAnimator : MonoBehaviour, IEntityAnimator {
    private EnemyEntity _enemy = null;
    private Animator _anim = null;

    void Start() {
        _enemy = GetComponentInParent<EnemyEntity>();
        
        _anim = GetComponent<Animator>();
    }

    // Fires when the attack animation is done. THIS IS REFERENCED THROUGH AN ANIMATION EVENT ( 5/8/2020 3:52pm )
    public void AttackAnimEnd() {
        EventManager.TriggerEvent("EnemyAttackAnimEnd", new EventParams());
    }

    #region IObjectAnimator

    public void DoDefendAnim(string _animName) {
        try {
            StartCoroutine(SpriteFlash(_anim.gameObject, 3, 0.06f));
            StartCoroutine(DoAfterAnim(_animName, _anim, () => {
                EventManager.TriggerEvent("EnemyDefendAnimEnd", new EventParams());
            }));
        }  catch {
            Debug.LogError(_anim.gameObject.name + " does not have Animator Component and/or cannot perform action!");
        }
    }

    public void DoAnim(string _animName) {
        try {
            StartCoroutine(DoAfterAnim(_animName, _anim, () => {    }));
        }  catch {
            Debug.LogError(_anim.gameObject.name + " does not have Animator Component and/or cannot perform action!");
        }
    }

    #endregion
}
