using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro.EditorUtilities;
using UnityEngine;


/* 4/25/2020 4:51pm - Tooltip Class 
 * 
 * Deals with the creation of tooltips and the such
 * 
 */ 
public static class Tooltip {
    public static GameObject CreateTooltip(Component _component) {
        GameObject _object;

        if (_component is EnemyObject) {
            _object = Object.Instantiate(Resources.Load("Prefabs/Enemy Tooltip Panel") as GameObject);
        } else {
            _object = Object.Instantiate(new GameObject());
        }

        _object.transform.SetParent(GameObject.FindGameObjectWithTag("TooltipCanvas").transform);
        _object.transform.localScale = new UnityEngine.Vector3(1, 1, 1);

        return _object;
    }

    public static void DeleteTooltip(GameObject _tooltipObj) {
        if (_tooltipObj != null)
            Object.Destroy(_tooltipObj);
    }
}
