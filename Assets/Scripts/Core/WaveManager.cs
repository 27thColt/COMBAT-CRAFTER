using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleState;

/* 10/24/2019 9:57pm - Wave Manager
 * Handles everything to do with enemy wave logic and all that
 * 
 * 2 or so hours of understanding what delegates and events so here we goo
 * 
 * 
 * This script initialize everything; ever since the fall of Mr. GameStateManager (rip) ( 12/27/2019 4:00pm )
 */ 
public class WaveManager : MonoBehaviour {
    #region Singleton

    private static WaveManager _waveManager;

    public static WaveManager instance {
        get {
            if (!_waveManager) {
                _waveManager = FindObjectOfType(typeof(WaveManager)) as WaveManager;

                if (!_waveManager) {
                    Debug.LogError("There needs to be one active WaveManager script on a GameObject in your scene.");
                }
            }

            return _waveManager;
        }
    }
    
    #endregion

    public EnemyWave[] waveList;
    public int currentWave;
    public EnemyWave loadedWave; // loadedWave is the enemy wave itself of ID currentWave ( 10/28/2019 2:06pm )

    public List<EnemyEntity> enemyList = new List<EnemyEntity>();

    [SerializeField]
    private GameObject enemyPrefab = null;

    [SerializeField]
    private GameObject[] spawnpoints = null;

    // NOTE: Maximum number of spawnpoints should be 5, thus 5 enemies should only appear at any given moment ( 12/27/2019 10:52am )
    
    private void Awake() {
        EventManager.StartListening("BStateChange", On_BStateChange);

        waveList = Resources.LoadAll<EnemyWave>("Enemy Waves");

        waveList = SortWaveList(waveList);
        currentWave = 0;
    }


    private void OnDestroy() {
        EventManager.StopListening("BStateChange", On_BStateChange);
    }

    #region Functions

    // Creates/Displays an enemy in the world ( 12/26/2019 11:15pm )
    public void AddEnemy(EnemyType _enemy, GameObject _parent) {
        Debug.Log("Adding " + _enemy.enemyName + " to game world");

        GameObject _enemyObj = Instantiate(enemyPrefab, _parent.transform);
        _enemyObj.GetComponent<EnemyEntity>().SetEnemy(_enemy);

        enemyList.Add(_enemyObj.GetComponent<EnemyEntity>());
    }

    // Creates/Displays all of the enemies of a specific wave in the game world ( 12/26/2019 11:25pm )
    public void AddAllEnemiesInWave(EnemyWave _wave) {
        for (int i = 0; i < _wave.enemies.Length; i++) {
            AddEnemy(_wave.enemies[i], spawnpoints[i]);

            // Adds each enemy to enemy inventory ( 4/21/2020 5:26pm )
            EnemyInventory.instance.AddEnemyDef(_wave.enemies[i]);
        }
    }

    // Something to sort the wave list that gets loaded. Sorted by ID number. Bubble sort. ( 10/27/2019 9:59am )
    public EnemyWave[] SortWaveList(EnemyWave[] _list) {
        EnemyWave _reserve;
        for (int i = 0; i < _list.Length; i++) {
            for (int j = 0; j < _list.Length; j++) {
                if (j + 1 < _list.Length && _list[j].waveID > _list[j + 1].waveID) {
                    _reserve = _list[j + 1];

                    _list[j + 1] = _list[j];
                    _list[j] = _reserve;
                }
            }
        }

        return _list;
    }

    #endregion

    #region Event Listeners

    // Fires when the gamestate has been changed ( 12/27/2019 1:14pm )
    public void On_BStateChange(EventParams _eventParams) {
        if (_eventParams.bstateParam == Bstate.game_LOADWAVE) {
            loadedWave = waveList[currentWave];
            EnemyInventory.instance.LoadEnemyDefs();

            AddAllEnemiesInWave(waveList[currentWave]);


            EventManager.TriggerEvent("BStateFinish", new EventParams(Bstate.game_LOADWAVE));
        }
    }

    #endregion
}
