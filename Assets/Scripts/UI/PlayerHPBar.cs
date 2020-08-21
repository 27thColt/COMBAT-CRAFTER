using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    8/2/2020 1:09pm - Player HP Bar Monobehaviour
    the Bridge between the Player Entity object and the HP Bar

    Takes in signals from the Player Entity and feeds them into the HP Bar
    This way, the HPBar can stay universal for any and all HPBars with different "controller" scripts depending on the context
*/
[RequireComponent(typeof(HPBar))]
public class PlayerHPBar : MonoBehaviour {
    private HPBar _hpBar;
    void Awake() {
        EventManager.StartListening("PlayerDamaged", On_PlayerDamaged);
    }

    void Start() {
        _hpBar = GetComponent<HPBar>();
        GameObject player = FindObjectOfType<PlayerData>().gameObject;
        _hpBar.SetObject(player);
    }

    void OnDestroy() {
        EventManager.StopListening("PlayerDamaged", On_PlayerDamaged);
    }

    private void On_PlayerDamaged(EventParams eventParams) {
        int MaxHP = eventParams.intParam1;
        int HPCache = eventParams.intParam2;
        int CurrentHP = eventParams.intParam3;

        StartCoroutine(_hpBar.AnimateDamage(MaxHP, HPCache, CurrentHP));
    }
}
