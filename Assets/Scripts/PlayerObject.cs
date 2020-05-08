using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleState;

/* 4/26/2020 1:43am - Player Object 
 * Oh my god I can already tell this will be good for me
 * 
 * Okay, this manages all player related stats-- as time goes on I will have to delegate any
 * abstract stat based stuff to another script and animations to this one
 * 
 * The term 'object' is used to refers to a physical gameobject that can be seen in the scene rather than the data type (at least the way I use it)
 */ 
public class PlayerObject : MonoBehaviour, IHealthPoints {
    #region IHealthPoints

    public int MaxHP { get; set; } = 60;
    public int CurrentHP { get; set; }

    public IEnumerator TakeDamage(int _full, int _damage) {
        yield return new WaitForSeconds(0.4f);

        while (CurrentHP > _full - _damage) {
            CurrentHP -= 1;

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.4f);

        Debug.Log("Damage dealt: " + _damage + " | HP left: " + CurrentHP);
    }

    
    public void Die() {
        return;
    }

    #endregion

    [SerializeField]
    private GameObject _hpBar = null;

    void Awake() {
        EventManager.StartListening("EnemyAttackAnimEnd", On_EnemyAttackAnimEnd);
    }

    private void OnDestroy() {
        EventManager.StopListening("EnemyAttackAnimEnd", On_EnemyAttackAnimEnd);
    }

    void Start() {
        if (GetComponent(typeof(ObjectAnimator.IObjectAnimator)) == null) {
            Debug.LogError(gameObject.name + " has no IObjectAnimator component attached.");
        }


        // Following shit sets up the health and stuff ( 4/27/2020 2:05pm )
        CurrentHP = MaxHP;
        _hpBar.GetComponent<HPBar>().SetObject(gameObject);
    }

    // Is performed after the enemy attack animation ends ( 5/7/2020 5:22pm )
    private void On_EnemyAttackAnimEnd(EventParams _eventParams) {
        if (_eventParams.intParam1 != 0) {
            int _damage = _eventParams.intParam1;


            StartCoroutine(TakeDamage(CurrentHP, _damage));


            if (CurrentHP - _damage > 0) {
                    try {
                        EventManager.TriggerEvent("PlayerDefendAnim", new EventParams("Damaged"));
                    } catch {
                        Debug.Log(name + " does not have Animator Component and/or cannot performed Damaged action!");
                    }
                } else {
                    try {
                        EventManager.TriggerEvent("PlayerDefendAnim", new EventParams("Died"));
                    } catch {
                        Debug.Log(name + " does not have Animator Component and/or cannot performed Died action!");
                    }
            }
        }
        
    }
}
