using System;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    GAME_INITIALIZED,
    GAME_STARTED,
    GAME_OVER,
    SCORE_UPDATED,
    LIVES_UPDATED,
    PLAYER_RESPAWN,
    PLAYER_DIED,
    ASTEROID_DESTROYED,
    PLAYER_SHOOT,
    ENEMY_DESTROYED,
    ENEMY_SPAWNED,
    ENEMY_SHOOT,
}

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    private Dictionary<EventType, Action<object>> _eventDictionary;

    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EventManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("EventManager");
                    _instance = obj.AddComponent<EventManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        _eventDictionary = new Dictionary<EventType, Action<object>>();
    }

    public void AddListener(EventType eventType, Action<object> listener)
    {
        if (_eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
        {
            thisEvent += listener;
            _eventDictionary[eventType] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            _eventDictionary.Add(eventType, thisEvent);
        }
    }

    public void RemoveListener(EventType eventType, Action<object> listener)
    {
        if (_eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
        {
            thisEvent -= listener;
            _eventDictionary[eventType] = thisEvent;
        }
    }

    public void TriggerEvent(EventType eventType, object parameter = null)
    {
        if (_eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
        {
            thisEvent?.Invoke(parameter);
        }
    }
}