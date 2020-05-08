using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  5/8/2020 11"44am - Battle Logic Class
    Contains functions that all relate to battle logic, such as damage calculations
*/
public static class BattleLogic {

    // Damage modifier values ( 5/8/2020 2:50pm )
    public static float neModifier = 1.0f; // Not Effective Modifier ( 5/8/2020 2:56pm )
    public static float seModifier = 1.5f; // Super Effective Modifier ( 5/8/2020 2:56pm )

    // Percentages for upper and lower bounds for the variation modifier (randomizes attack damage) ( 5/8/2020 2:58pm )
    public static float varUpperBound = 0.1f;
    public static float varLowerBound = -varUpperBound; // Lower Bound is always the negative of the upper bound ( 5/8/2020 2:58pm )


    public static bool CheckVulnerabilities(EnemyType _defEnemy, ItemType _attItem) {
        for (int i = 0; i < _defEnemy.vulnerabilities.Length; i++) {
            if (_defEnemy.vulnerabilities[i] == _attItem)
                return true;
        }

        return false;
    }

    // This will be used to calculate the damage dealt ( 4/24/2020 1:04am )
    public static int CalculateAttackDamage(int _atk, float _modifier = 1, bool _varModifOn = false, System.Random _rnd = null) {
        int _original = Mathf.RoundToInt(4.5f * _atk);
        int _varModif = 0;
        // When Var modif on, the damage is given a bit of randomness, attack damage will never be constant ( 5/8/2020 3:01pm )
        if (_varModifOn) {
            _varModif = _rnd.Next(Mathf.RoundToInt(varLowerBound * _original) - 1, Mathf.RoundToInt(varUpperBound * _original) + 1);
        }
        
        
        return Mathf.RoundToInt(_original * _modifier + _varModif);
    }


    // Returns the range of values (accounting randomness) in the form of a list ( 5/8/2020, 3:15pm )
    public static List<int> CalculateMinMaxDamage(int _atk, float _modifier = 1) {
        int _original = CalculateAttackDamage(_atk, _modifier);

        int _lowerBound = _original + Mathf.RoundToInt(varLowerBound * _original);
        int _upperBound = _original + Mathf.RoundToInt(varUpperBound * _original);;
        
        List<int> _output = new List<int> {_lowerBound, _upperBound};

        return _output;
    }
    
}
