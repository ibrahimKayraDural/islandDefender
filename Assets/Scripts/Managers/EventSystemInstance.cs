using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemInstance : MonoBehaviour
{
    public static EventSystemInstance Instance { get; private set; } = null;
    public EventSystem @EventSystem => _eventSystem;
    EventSystem _eventSystem;

    void Awake()
    {
        if (TryGetComponent(out _eventSystem) == false)
        {
            Destroy(this);
            return;
        }

        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }
}
