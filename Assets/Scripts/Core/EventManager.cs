using System;
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

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    #region Functions

    // Creates the event dictionary ( 5/6/2020 7:51pm )
    void Init() {
        if (_eventDictionary == null) {
            _eventDictionary = new Dictionary<string, Action<EventParams>>();
        }
    }

    public static void StartListening(string eventName, Action<EventParams> listener) {
        if (instance._eventDictionary.ContainsKey(eventName)) {
            instance._eventDictionary[eventName] += listener; // Updates the dictionary ( 5/6/2020 7:55pm )

        } else {
            instance._eventDictionary.Add(eventName, listener); // Adds to the dictionary for the first time ( 5/6/2020 7:56pm )
        }
    }

    public static void StopListening(string eventName, Action<EventParams> listener) {
        if (_eventManager == null) return; // Checks if the event manager had not been destroyed already ( 5/6/2020 7:58pm )
        if (instance._eventDictionary.ContainsKey(eventName)) {
            instance._eventDictionary[eventName] -= listener;
        }
    }

    public static void TriggerEvent(string eventName, EventParams eventParams) {
        Action<EventParams> _thisEvent = null;
        if (instance._eventDictionary.TryGetValue(eventName, out _thisEvent)) {
            _thisEvent.Invoke(eventParams);
        }
    }
    
    #endregion
}

/* 5/6/2020 7:49pm - EventParams Struct
    Following structure contains all parameters needed to pass

*/
public class EventParams {
    public EventParams() {
        // sets everything to default (null) ( 5/7/2020 10:56am )
    }

    #region Special Objects

    public Room roomParam = null;

    public EventParams(Room roomParam) {
        this.roomParam = roomParam;
    }

    public ItemType itemTypeParam1, itemTypeParam2 = null;

    public EventParams(ItemType itemTypeParam1, ItemType itemTypeParam2 = null) {
        this.itemTypeParam1 = itemTypeParam1;
        this.itemTypeParam2 = itemTypeParam2;
    }

    public Item itemParam = null;

    public EventParams(Item itemParam) {
        this.itemParam = itemParam;
    }

    public EnemyType enemyTypeParam1 = null;

    public EventParams(EnemyType enemyTypeParam1) {
        this.enemyTypeParam1 = enemyTypeParam1;
    }

    #endregion

    #region Basic Variable Types

    public int intParam1 = 0;
    public int intParam2 = 0;
    public int intParam3 = 0;
 
    public EventParams(int intParam1, int intParam2 = 0, int intParam3 = 0) {
        this.intParam1 = intParam1;
        this.intParam2 = intParam2;
        this.intParam3 = intParam3;
    }

    public String stringParam = null;

    public EventParams(String stringParam) {
        this.stringParam = stringParam;
    }
    
    public bool boolParam = false;

    public EventParams(bool boolParam) {
        this.boolParam = boolParam;
    }
    public MonoBehaviour componentParams = null;

    public EventParams(MonoBehaviour componentParams) {
        this.componentParams = componentParams;
    }

    #endregion
} 
