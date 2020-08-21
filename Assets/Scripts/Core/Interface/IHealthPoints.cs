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
    int HPCache { get; set; } // A cache value which is used to hold temporary integer values for the HP ( 5/9/2020 3:36pm )
    
    void TakeDamage(int _damage);

    void Die();
}
