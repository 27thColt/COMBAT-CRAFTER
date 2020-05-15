using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityAnimator;

[RequireComponent(typeof(Animator))]
public class PlayerEntityAnimator : MonoBehaviour, IEntityAnimator {
    private PlayerEntity _playerObject = null;
    private Animator _anim = null;

    void Awake() {
        EventManager.StartListening("PlayerDefendAnim", On_DefendAnim);
        EventManager.StartListening("PlayerAttackAnim", On_AttackAnim);
    }

    void OnDestroy() {
        EventManager.StopListening("PlayerDefendAnim", On_DefendAnim);
        EventManager.StopListening("PlayerAttackAnim", On_AttackAnim);
    }

    void Start() {
        _playerObject = GetComponent<PlayerEntity>();
        
        _anim = GetComponentInChildren<Animator>();
    }

    // Fires when the attack animation is done. THIS IS REFERENCED THROUGH AN ANIMATION EVENT ( 5/8/2020 7:438pm )
    public void AttackAnimEnd() {
        EventManager.TriggerEvent("PlayerAttackAnimEnd", new EventParams());
    }

    #region IObjectAnimator

    public void On_AttackAnim(EventParams _eventParams) {
        if (_eventParams.stringParam1 != null) {
            try {
                StartCoroutine(DoAfterAnim(_eventParams.stringParam1, _anim, () => {    }));
            }  catch {
                Debug.LogError(_anim.gameObject.name + " does not have Animator Component and/or cannot performed Damaged action!");
            }
        } else {
            Debug.LogError("EventParams with non-null stringParam1 expected.");
        }
    }

    public void On_DefendAnim(EventParams _eventParams) {
        if (_eventParams.stringParam1 != null) {
            try {
                StartCoroutine(SpriteFlash(_anim.gameObject, 3, 0.06f));
                StartCoroutine(DoAfterAnim(_eventParams.stringParam1, _anim, () => {
                    EventManager.TriggerEvent("PlayerDefendAnimEnd", new EventParams());
                }));
            }  catch {
                Debug.LogError(_anim.gameObject.name + " does not have Animator Component and/or cannot performed Damaged action!");
            }
        } else {
            Debug.LogError("EventParams with non-null stringParam1 expected.");
        }
    }

    #endregion
}
