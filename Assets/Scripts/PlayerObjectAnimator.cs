using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectAnimator;

[RequireComponent(typeof(PlayerObject))]
public class PlayerObjectAnimator : MonoBehaviour, IObjectAnimator {
    private PlayerObject _playerObject = null;
    private Animator _anim = null;

    void Awake() {
        EventManager.StartListening("PlayerDefendAnim", On_DefendAnim);
    }

    void OnDestroy() {
        EventManager.StopListening("PlayerDefendAnim", On_DefendAnim);
    }

    void Start() {
        _playerObject = GetComponent<PlayerObject>();
        
        _anim = GetComponentInChildren<Animator>();
    }

    #region IObjectAnimator

    public void On_AttackAnim(EventParams _eventParams) {
        if (_eventParams.stringParam1 != null) {
            try {
                StartCoroutine(DoAfterAnim(_eventParams.stringParam1, _anim, () => {
                    EventManager.TriggerEvent("PlayerAttackAnimEnd", new EventParams());
                }));
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
