using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerScript : MonoBehaviour
{
    [SerializeField] UnityEvent<Collider> OnEntered;
    [SerializeField] UnityEvent<Collider> OnExited;
    [SerializeField] UnityEvent<Collider> OnStay;

    void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        OnEntered?.Invoke(other);
    }
    void OnTriggerExit(Collider other)
    {
        OnExited?.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        OnStay?.Invoke(other);
    }
}
