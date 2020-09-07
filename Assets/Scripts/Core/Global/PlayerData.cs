using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    8/2/2020 12:48pm - Player Data Class
    This class holds all global information about the player
    Stats, and other data

    Okay for this part of the refactoring, I'm going to move all of the health related information
    that was previously in the Player Entity class now over here. Because it is player Data
    that needs to be made universal 
*/
public class PlayerData : MonoBehaviour, IHealthPoints {
    #region IHealthPoints

    public int MaxHP { get; set; } = 60;
    public int CurrentHP { get; set; }
    public int HPCache { get; set; } = 0;

    public void TakeDamage(int _damage) {
        HPCache = CurrentHP;
        CurrentHP -= _damage;
    }
    
    public void Die() {
        return;
    }

    #endregion

    #region Unity Functions

    void Awake() {
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += On_SceneLoaded;
        SceneManager.sceneUnloaded += On_SceneUnloaded;
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= On_SceneLoaded;
        SceneManager.sceneUnloaded -= On_SceneUnloaded;
    }
    

    #endregion

    #region Functions

    // Initializes everything. Called in LevelLoad ( 8/5/2020 7:50pm )
    public void Init() {
        CurrentHP = MaxHP;
    }

    #endregion

    #region Event Listeners

    private void On_SceneLoaded(Scene scene, LoadSceneMode sceneMode) {
        EventManager.StartListening("EnemyAttackAnimEnd", On_EnemyAttackAnimEnd);
        EventManager.StartListening("EnemyAttack", On_EnemyAttack);
        EventManager.StartListening("PlayerHeal", On_PlayerHeal);       
    }

    private void On_SceneUnloaded(Scene scene) {
        EventManager.StopListening("EnemyAttackAnimEnd", On_EnemyAttackAnimEnd);
        EventManager.StopListening("EnemyAttack", On_EnemyAttack);
        EventManager.StopListening("PlayerHeal", On_PlayerHeal);
    }

    private void On_PlayerHeal(EventParams eventParams) {
        if (eventParams.intParam1 == 0) { Debug.LogError("EventParams with non-zero intParam1 expected."); return ;}
        
        int regen = eventParams.intParam1;

        if (CurrentHP + regen > MaxHP) {
            HPCache = CurrentHP;
            CurrentHP = MaxHP;
        } else {
            TakeDamage(-regen);
        }

        EventManager.TriggerEvent("PlayerDamaged", new EventParams(MaxHP, HPCache, CurrentHP)); 

        HPCache = 0;
    }

    private void On_EnemyAttack(EventParams eventParams) {
        if (eventParams.intParam1 == 0) { Debug.LogError("EventParams with non-zero intParam1 expected."); return; }

        TakeDamage(eventParams.intParam1);
    }

    private void On_EnemyAttackAnimEnd(EventParams eventParams) {
        EventManager.TriggerEvent("PlayerDamaged", new EventParams(MaxHP, HPCache, CurrentHP)); 
        
        HPCache = 0;

        if (CurrentHP > 0)
            EventManager.TriggerEvent("PlayerDamagedAnim", new EventParams("Damaged"));
        else
            EventManager.TriggerEvent("PlayerDamagedAnim", new EventParams("Died"));
    }

    #endregion
}
