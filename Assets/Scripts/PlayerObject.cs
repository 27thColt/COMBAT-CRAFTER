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

        SetCurrentState(Bstate.game_ROUNDRESET);

        Debug.Log("Damage dealt: " + _damage + " | HP left: " + CurrentHP);
    }

    #endregion

    [SerializeField]
    private GameObject _hpBar = null;

    void Awake() {
        BattleManager.OnEnemyAttack += DamageListener;
    }

    private void OnDestroy() {
        BattleManager.OnEnemyAttack -= DamageListener;
    }

    void Start() {
        // Following shit sets up the health and stuff ( 4/27/2020 2:05pm )
        CurrentHP = MaxHP;
        _hpBar.GetComponent<HPBar>().SetObject(gameObject);
    }

    private void DamageListener(int _damage) {
        StartCoroutine(TakeDamage(CurrentHP, _damage));
    }
}
