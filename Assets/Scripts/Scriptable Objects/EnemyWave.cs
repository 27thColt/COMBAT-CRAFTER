using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* 10/24/2019 10:07pm - Enemy Wave Scriptable Object
 * Each battle will consist of a wave of enemies
 * Basically each wave has a list of enemies
 * 
 */
[CreateAssetMenu(fileName = "EnemyWave", menuName = "Enemy Wave", order = 3)]
public class EnemyWave : ScriptableObject {
    public int waveID;
    public EnemyType[] enemies;
}
