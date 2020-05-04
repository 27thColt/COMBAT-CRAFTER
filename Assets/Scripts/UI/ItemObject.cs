using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/* 5/30/2019 8:06pm - Item Display script
 * Attached onto the item prefab. Takes information from the item scriptable object to be used in the UI.
 * 
 * Gammer - Jaws 2016 is a nice song, actually Gammer is just plain awesome
 */ 
public class ItemObject : MonoBehaviour {

    public ItemType item; // Determines the item, duh ( 5/30/2019 8:09pm )
    public TextMeshProUGUI textMesh; // TextMesh Pro game object in the child ( 5/30/2019 8:51pm )
    public Image displayImage; // Image game object in the child ( 5/30/2019 8:57pm )

    public void SetItem(ItemType _item) {
        item = _item;
        textMesh.text = _item.itemName;
        displayImage.sprite = _item.sprite;
    }
}
