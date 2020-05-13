﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* 5/6/2020 7:45pm - Event Manager

    First attempt at a PROPER event manager
    Thanks to this video: https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events

*/
public class EventManager : MonoBehaviour {
    #region Singleton

    private static EventManager _eventManager;

    // Ensures that there is an EventManager in the scene ( 5/6/2020 7:51pm )
    public static EventManager instance {
        get {
            if (!_eventManager) {
                _eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!_eventManager) {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                } else {
                    _eventManager.Init();
                }
            }
            return _eventManager;
        }
    }

    #endregion

    private Dictionary<string, Action<EventParams>> _eventDictionary;

    // Creates the event dictionary ( 5/6/2020 7:51pm )
    void Init() {
        if (_eventDictionary == null) {
            _eventDictionary = new Dictionary<string, Action<EventParams>>();
        }
    }

    public static void StartListening(string _eventName, Action<EventParams> _listener) {
        //Action<EventParams> _thisEvent;

        // Checks the key and event exists within the dictionary already ( 5/6/2020 7:54pm )
        if (instance._eventDictionary.ContainsKey(_eventName)) {
            //_thisEvent += _listener; // Adds the listener to the existing event ( 5/6/2020 7:54pm )
            instance._eventDictionary[_eventName] += _listener; // Updates the dictionary ( 5/6/2020 7:55pm )

        } else {
            //_thisEvent += _listener;
            instance._eventDictionary.Add(_eventName, _listener); // Adds to the dictionary for the first time ( 5/6/2020 7:56pm )
        }
    }

    public static void StopListening(string _eventName, Action<EventParams> _listener) {
        if (_eventManager == null) return; // Checks if the event manager had not been destroyed already ( 5/6/2020 7:58pm )
        //Action<EventParams> _thisEvent;
        if (instance._eventDictionary.ContainsKey(_eventName)) {
            //_thisEvent -= _listener;
            instance._eventDictionary[_eventName] -= _listener;
        }
    }

    public static void TriggerEvent(string _eventName, EventParams _eventParams) {
        Action<EventParams> _thisEvent = null;
        if (instance._eventDictionary.TryGetValue(_eventName, out _thisEvent)) {
            _thisEvent.Invoke(_eventParams);
        }
    }
}

/* 5/6/2020 7:49pm - EventParams Struct
    Following structure contains all parameters needed to pass

*/
public class EventParams {
    public EventParams() {
        // sets everything to default (null) ( 5/7/2020 10:56am )
    }
    public Bstate bstateParam = Bstate.none;

    public EventParams(Bstate _bstateParam) {
        bstateParam = _bstateParam;
    }

    public ItemType itemTypeParam1, itemTypeParam2 = null;

    public EventParams(ItemType _itemTypeParam1, ItemType _itemTypeParam2 = null) {
        itemTypeParam1 = _itemTypeParam1;
        itemTypeParam2 = _itemTypeParam2;
    }

    public Item itemParam = null;

    public EventParams(Item _itemParams) {
        itemParam = _itemParams;
    }

    public int intParam1 = 0;

    public EventParams(int _intParam1) {
        intParam1 = _intParam1;
    }

    public EnemyType enemyTypeParam1 = null;

    public EventParams(EnemyType _enemyTypeParam1) {
        enemyTypeParam1 = _enemyTypeParam1;
    }

    public String stringParam1 = null;

    public EventParams(String _stringParam1) {
        stringParam1 = _stringParam1;
    }

    public MonoBehaviour componentParams = null;

    public EventParams(MonoBehaviour _componentParams) {
        componentParams = _componentParams;
    }
} 
