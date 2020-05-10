using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 6/3/2019 7:46pm - Recipe scriptable object
 * dictates which items can be crafted into what material
 */
[CreateAssetMenu(fileName = "Recipe", menuName = "Recipe", order = 1)]
[System.Serializable]
public class Recipe : ScriptableObject {
    public ItemType material1;
    public ItemType material2;

    public ItemType result;
    
}
