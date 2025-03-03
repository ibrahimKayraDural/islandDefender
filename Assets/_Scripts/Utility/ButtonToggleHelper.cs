using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonToggleHelper : MonoBehaviour
{
    public bool Status => _CurrentStatus;

    [SerializeField] bool _CurrentStatus = false;
    [SerializeField] UnityEvent OnToggleOn;
    [SerializeField] UnityEvent OnToggleOff;

    public void Toggle() => SetStatus(!_CurrentStatus);

    public void SetStatus(bool setTo)
    {
        _CurrentStatus = setTo;

        if (_CurrentStatus) OnToggleOn?.Invoke();
        else OnToggleOff?.Invoke();
    }
}
