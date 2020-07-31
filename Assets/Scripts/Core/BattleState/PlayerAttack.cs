using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleLogic;

/*
    7/28/2020 9:22pm - Player Attack Battle State
    The state in which the player attacks (duh.. )
*/
public class PlayerAttack : BattleState {
    private System.Random _rnd = new System.Random();

    public PlayerAttack(BattleManager battleManager, WaveManager waveManager, Crafter crafter) : base (battleManager, waveManager, crafter) {
        if (_waveManager == null) Debug.Log("Wave Manager game object cannot be found");
    }

    override public void Start(EventParams eventParams) {
        Debug.Log("PLAYER ATTACK STATE INITIALIZED");

        if (eventParams.itemParam == null) { Debug.LogError("No ItemParam passed. Stopping Player Attack State."); return;}

        if (eventParams.enemyTypeParam1 == null) Debug.Log("No EnemyTypeParam passed, player must not be attacking.");

        PlayerAttackState(eventParams.itemParam, eventParams.enemyTypeParam1);
    }

    override public void End(EventParams eventParams, string stateName) {
        if (_waveManager.enemyList.Count > 0) {
            BattleStateMachine.SetCurrentBState(new EnemyAttack(_battleManager, _waveManager, _crafter));

        } else {
            // Fires if all enemies in the wavee have been defeated ( 5/1/2020 5:29pm )
            BattleStateMachine.SetCurrentBState(null);
        }
    }

    private void PlayerAttackState(Item attackingItem, EnemyType defendingEnemy = null) {
        if (attackingItem is Potion) {
            // When the attacking item is a potion, the battle skips the enemy selection phase and goes straight to player_ATTACK ( 5/14/2020 2:03pm )
            Potion activePotion = attackingItem as Potion;

            EventManager.TriggerEvent("PlayerHeal", new EventParams(activePotion.potionType.regen));
        } else {
            float modifier;

            if (CheckVulnerabilities(defendingEnemy, attackingItem.itemType)) {
                // Add vulnerability to enemy inventory ( 4/24/2020 1:03am )
                EnemyInventory.instance.AddEnemyVul(defendingEnemy, attackingItem.itemType);

                modifier = seModifier;
                Debug.Log("It's Super Effective!");
            } else {
                
                if (attackingItem is Weapon) {
                    modifier = neModifier_Weapon;
                } else {
                    modifier = neModifier;
                }
                
                Debug.Log("Not Very Effective...");
            }

            EventManager.TriggerEvent("PlayerAttack", new EventParams(CalculateAttackDamage(attackingItem.atk, modifier, true, _rnd)));
        }      
    }
}
