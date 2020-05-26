using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityAnimator;

[RequireComponent(typeof(Animator))]
public class PlayerEntityAnimator : MonoBehaviour, IEntityAnimator {
    private Animator _anim = null;

    void Start() {
        _anim = GetComponent<Animator>();
    }

    // Fires when the attack animation is done. THIS IS REFERENCED THROUGH AN ANIMATION EVENT ( 5/8/2020 7:438pm )
    public void ActionAnimEnd() {
        EventManager.TriggerEvent("PlayerAttackAnimEnd", new EventParams());
    }

    #region IObjectAnimator

    public void DoDefendAnim(string _animName) {
        try {
            StartCoroutine(SpriteFlash(_anim.gameObject, 3, 0.06f));
            StartCoroutine(DoAfterAnim(_animName, _anim, () => {
                EventManager.TriggerEvent("PlayerDefendAnimEnd", new EventParams());
            }));
        }  catch {
            Debug.LogError(_anim.gameObject.name + " does not have Animator Component and/or cannot perform action!");
        }
    }

    public void DoAnim(string _animName) {
        if (_animName == "Attack") {
            StartCoroutine(DoAfterAnim(_animName, _anim, () => {    }));
        } else if (_animName == "Heal") {
            StartCoroutine(DoAfterAnim(_animName, _anim, () => {
                BattleState.FinishCurrentState(Bstate.player_ATTACK);
            }));
        }
    }

    #endregion
}
