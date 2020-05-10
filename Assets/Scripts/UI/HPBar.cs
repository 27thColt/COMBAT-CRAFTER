using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*  4/26/2020 4:53pm - HP Bar
 * Updates the HP Bar, want to design this so that any object with a health (implements IHealthPoints) may use one
 * 
 */
public class HPBar : MonoBehaviour {
    [SerializeField]
    private Image _hpBar = null;

    // Sets the focused object ( 4/27/2020 2:02pm )
    public void SetObject(GameObject _obj) {
        // Error if the object does not contain a component which implements IHealthPoints ( 4/27/2020 1:55pm )
        if (_obj.GetComponent(typeof(IHealthPoints)) != null) {
            IHealthPoints _health = _obj.GetComponent(typeof(IHealthPoints)) as IHealthPoints;

             _hpBar.fillAmount = (float)_health.CurrentHP / (float)_health.MaxHP;
        } else {
            Debug.LogError(_obj.name + " does not implement IHealthPoints!");
        }
    }

    // Coroutine which animates the HP Bar ( 5/9/2020 3:31pm )
    public IEnumerator AnimateDamage(int _maxHP, int _startHP, int _endHP) {
        int _currentHP = _startHP;

        yield return new WaitForSeconds(0.4f);

        while (_currentHP > _endHP) {
            
            _currentHP -= 1;
            _hpBar.fillAmount = (float)_currentHP / _maxHP;

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.4f);

        // Debug.Log("Damage dealt: " + _damage + " | HP left: " + CurrentHP);
    }
}
