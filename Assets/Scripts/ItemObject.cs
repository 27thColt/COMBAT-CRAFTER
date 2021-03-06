﻿using System.Collections;
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

    #region UI Stuff
    [SerializeField]
    private TextMeshProUGUI _textMesh = null; // TextMesh Pro game object in the child ( 5/30/2019 8:51pm )
    [SerializeField]
    private Image _displayImage = null; // Image game object in the child ( 5/30/2019 8:57pm )
    [SerializeField]
    private Image _durabilityBar = null;
    [SerializeField]
    private Image _durabilityBG = null;
    [SerializeField]
    private TextMeshProUGUI _number = null;

    #endregion

    public void SetItem(Item item) {
        this.item = item;
        _textMesh.text = item.itemType.itemName;
        _displayImage.sprite = item.itemType.sprite;

        // Weapon Item Clause ( 5/11/2020 1:33pm )
        if (item is Weapon) {
            Weapon _weapon = item as Weapon;
            
            _number.gameObject.SetActive(false);

            _durabilityBar.gameObject.SetActive(true);
            _durabilityBG.gameObject.SetActive(true);

            _durabilityBar.fillAmount = (float) _weapon.currentDurability / (float)_weapon.maxDurability;
        // Generic Item Clause ( 5/11/2020 1:33pm )
        } else if (item is Potion) { 
            _number.gameObject.SetActive(false);

            _durabilityBar.gameObject.SetActive(false);
            _durabilityBG.gameObject.SetActive(false);
        } else {
            _number.text = item.number.ToString();

            _durabilityBar.gameObject.SetActive(false);
            _durabilityBG.gameObject.SetActive(false);
        }
        
    }

    public void UpdateItemInfo(Item item) {
        this.item = item;
        _number.text = item.number.ToString();

        if (item is Weapon) {
            Weapon weapon = item as Weapon;
            
            _durabilityBar.fillAmount = (float) weapon.currentDurability / (float)weapon.maxDurability;
        }
    }


}