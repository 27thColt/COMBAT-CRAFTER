using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  5/7/2020 4:46pm - ObjectAnimator Static Class
    This class holds object animator specific functions

*/
public static class ObjectAnimator {
    // Coroutine that plays an animation after a specified animation ( 5/7/2020 2:45pm )
    public static IEnumerator DoAfterAnim(string _animName = null, Animator _anim = null, Action _action = null) {
        if (_animName != null && _anim != null) 
            _anim.SetTrigger(_animName);
        
        // Following Code waits for the damaged/died animation to play out first ( 5/1/2020 5:09pm )
        yield return new WaitForSeconds(0.4f); // A small delay because the animation does not start immediately ( 5/1/2020 5:09pm )
        float _time = _anim.GetCurrentAnimatorClipInfo(0)[0].clip.length - (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime * _anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        
        if (_time < 0) {
            _time = 1.0f;
        }

        Debug.Log("Finishing " + _anim.GetCurrentAnimatorClipInfo(0)[0].clip.name + " animation. Waiting for " + _time + " seconds.");
        yield return new WaitForSeconds(_time - 0.4f);

        _action?.Invoke();
    }

    // Coroutine for when the sprite needs to flash something (taking damage),, this takes into account that we are using RIGZ and not just stupid sprites ( 5/1/2020 1:34am )
    public static IEnumerator SpriteFlash(GameObject _rig, int _loops, float _delay) {
        SpriteRenderer[] _sprites = _rig.GetComponentsInChildren<SpriteRenderer>();


        for (int i = 1; i <= _loops + 1; i++) {
            if (i % 2 == 0) {
                foreach (SpriteRenderer _sprite in _sprites) {
                    _sprite.color = new Color(1f, 1f, 1f, 0.7f); // White color, a bit transparent ( 5/1/2020 1:35am )
                }
            } else {
                foreach (SpriteRenderer _sprite in _sprites) {
                    _sprite.color = new Color(1f, 1f, 1f, 1f); // White color, opacity 100% ( 5/1/2020 1:35am )
                }
            }

            yield return new WaitForSeconds(_delay);
        }

        // Return to normal just in case ( 5/1/2020 1:35am )
        foreach (SpriteRenderer _sprite in _sprites) {
            _sprite.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    /* 5/7/2020 3:01pm - IObjectAnimator
        Interface that contains functions for an animator controller
        (EnemyObjectAnimator, PlayerObjectAnimator)
    */
    public interface IObjectAnimator {
        void On_DefendAnim(EventParams _eventParams);
        void On_AttackAnim(EventParams _eventParams);
    }

}
