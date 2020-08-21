using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityAnimator;

[RequireComponent(typeof(Animator))]
public class PlayerEntityAnimator : MonoBehaviour, IEntityAnimator {
    private Animator _anim = null;

    #region IEntityAnimator

    public void DoDefendAnim(string animName) {
        try {
            StartCoroutine(SpriteFlash(_anim.gameObject, 3, 0.06f));
            StartCoroutine(DoAfterAnim(animName, _anim, () => {
                EventManager.TriggerEvent("PlayerDefendAnimEnd", new EventParams());
            }));
        }  catch {
            Debug.LogError(_anim.gameObject.name + " does not have Animator Component and/or cannot perform action!");
        }
    }

    public void DoAnim(string animName) {
        switch(animName) {
            case "Attack":
                StartCoroutine(DoAfterAnim(animName, _anim, () => {    }));

                break;
            case "Heal":
                StartCoroutine(DoAfterAnim(animName, _anim, () => {
                    // End of Player Attack Battle State if the player HEALED ( 7/28/2020 10:23pm )
                    BattleStateMachine.currentBState.End(new EventParams(), "PlayerAttack");
                }));

                break;
            default:

                Debug.LogError("No animation exists for " + animName);
                break;
        }
    }

    #endregion

    #region Unity Functions

    void Awake() {
        EventManager.StartListening("PlayerAttack", On_PlayerAttack);
        EventManager.StartListening("PlayerHeal", On_PlayerHeal);
        EventManager.StartListening("PlayerDamagedAnim", On_PlayerDamagedAnim);
    }

    private void OnDestroy() {
        EventManager.StopListening("PlayerAttack", On_PlayerAttack);
        EventManager.StopListening("PlayerHeal", On_PlayerHeal);
        EventManager.StopListening("PlayerDamagedAnim", On_PlayerDamagedAnim);
    }

    void Start() {
        _anim = GetComponent<Animator>();
    }

    #endregion

    #region Event Listeners

    private void On_PlayerHeal(EventParams eventParams) {
        DoAnim("Heal");
    }

    private void On_PlayerAttack(EventParams eventParams) {
        DoAnim("Attack");
    }

    // Is performed after the enemy attack animation ends ( 5/7/2020 5:22pm )
    private void On_PlayerDamagedAnim(EventParams eventParams) {
        if (eventParams.stringParam == null) { Debug.LogError("Expected non-null string parameter."); return; }

        DoDefendAnim(eventParams.stringParam);

    }

    #endregion

    // Fires when the attack animation is done. THIS IS REFERENCED THROUGH AN ANIMATION EVENT ( 5/8/2020 7:438pm )
    public void ActionAnimEnd() {
        EventManager.TriggerEvent("PlayerAttackAnimEnd", new EventParams());
    }
}
