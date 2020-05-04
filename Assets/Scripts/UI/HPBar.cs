using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*  4/26/2020 4:53pm - HP Bar
 * Updates the HP Bar, want to design this so that any object with a health (implements IHealthPoints) may use one
 * 
 */
public class HPBar : MonoBehaviour {

    // The IHealthPoints object that the health bar will focus on ( 4/26/2020 4:55pm )
    private GameObject _object = null;

    [SerializeField]
    private Image _hpBar = null;

    private IHealthPoints _health;

    void Update() {
        // If theres ever a way to only update the screen when the bar needs to be changed, i would gladly accept that instead ( 4/27/2020 2:03pm )
        _hpBar.fillAmount = (float)_health.CurrentHP / (float)_health.MaxHP;

    }

    // Sets the focused object ( 4/27/2020 2:02pm )
    public void SetObject(GameObject _obj) {

        // Error if the object does not contain a component which implements IHealthPoints ( 4/27/2020 1:55pm )
        if (_obj.GetComponent(typeof(IHealthPoints)) != null) {
            _object = _obj;
            _health = _object.GetComponent(typeof(IHealthPoints)) as IHealthPoints;
        } else {
            Debug.Log(_obj.name + " does not implement IHealthPoints!");
        }
    }
}
