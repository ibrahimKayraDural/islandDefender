using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonToggleHelper : MonoBehaviour
{
    [SerializeField] bool _CurrentStatus = false;
    [SerializeField] UnityEvent OnToggleOn;
    [SerializeField] UnityEvent OnToggleOff;

    public void Toggle()
    {
        _CurrentStatus = !_CurrentStatus;

        if (_CurrentStatus) OnToggleOn?.Invoke();
        else OnToggleOff?.Invoke();
    }
}
