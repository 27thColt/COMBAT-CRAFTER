using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*  8/5/2020 7:16pm - Level Load Class
    This script loads all the global gameobjects required for the level as well as other important information

    This script should only be called in the Level Load scene and the game will not work without it
*/
public class LevelLoad : MonoBehaviour {
    [SerializeField] private EnemyInventory _enemyInventory = null;
    [SerializeField] private Inventory _inventory = null;
    [SerializeField] private PlayerData _playerData = null;
    [SerializeField] private LevelManager _levelManager = null;
    void Start() {
        _inventory.Init();
        _playerData.Init();
        _levelManager.Init();
        
        LevelStateMachine.SetCurrentLState(new LevelExplore(_levelManager)); 
    }
}
