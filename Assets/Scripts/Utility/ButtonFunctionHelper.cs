using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonFunctionHelper : MonoBehaviour
{
    [SerializeField] UnityEvent _EventToRun;

    public void InvokeEvents() => _EventToRun?.Invoke();
}
