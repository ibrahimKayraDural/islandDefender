using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnablityChanged : MonoBehaviour
{
    [SerializeField] UnityEvent OnEnabled;
    [SerializeField] UnityEvent OnDisabled;

    void OnEnable()
    {
        OnEnabled?.Invoke();
    }
    void OnDisable()
    {
        OnDisabled?.Invoke();
    }
}
