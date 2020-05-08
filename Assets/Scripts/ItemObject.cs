using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 5/30/2019 8:06pm - Item Display script
    * Attached onto the item prefab. Takes information from the item scriptable object & class to be used in the UI.
    * 
    * Gammer - Jaws 2016 is a nice song, actually Gammer is just plain awesome
    */
public class ItemObject : MonoBehaviour {

    public Item item; // Determines the item, duh ( 5/30/2019 8:09pm )

    [SerializeField]
    private TextMeshProUGUI _textMesh = null; // TextMesh Pro game object in the child ( 5/30/2019 8:51pm )

    [SerializeField]
    private Image _displayImage = null; // Image game object in the child ( 5/30/2019 8:57pm )

    [SerializeField]
    private TextMeshProUGUI _number = null;

    public void SetItem(Item _item) {
        item = _item;
        _textMesh.text = _item.itemType.itemName;
        _displayImage.sprite = _item.itemType.sprite;
        _number.text = _item.number.ToString();
    }

    public void UpdateItemInfo(Item _item) {
        _number.text = _item.number.ToString();
    }


}