using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 6/3/2019 7:46pm - Recipe scriptable object
 * dictates which items can be crafted into what material
 */
[CreateAssetMenu(fileName = "Recipe", menuName = "Recipe", order = 1)]
[System.Serializable]
public class Recipe : ScriptableObject {
    public Item material1;
    public Item material2;

    public Item result;

    // Some recipes will result a second item, typically the first will be a "weapon", while secondaries are not ( 6/3/2019 8:20pm )
    public Item resultSecondary; 
    
}
