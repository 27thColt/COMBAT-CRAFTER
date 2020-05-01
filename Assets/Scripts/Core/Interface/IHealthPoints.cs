using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

/* 4/26/2020 4:22pm - IHealthPoints Interface
 * 
 * For any object (i.e. player, enemy) that uses HealthPoints
 */ 
interface IHealthPoints {
    int MaxHP { get; set; }
    int CurrentHP { get; set; }

    IEnumerator TakeDamage(int _full, int _damage);

    void Die();
}
