using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerScript : MonoBehaviour
{
    [SerializeField] List <string> Objects2Look4Tag = new List<string>();

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
        if (Objects2Look4Tag.Contains(other.gameObject.tag) == false && Objects2Look4Tag.Count > 0) return;

        OnEntered?.Invoke(other);
    }
    void OnTriggerExit(Collider other)
    {
        if (Objects2Look4Tag.Contains(other.gameObject.tag) == false && Objects2Look4Tag.Count > 0) return;

        OnExited?.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (Objects2Look4Tag.Contains(other.gameObject.tag) == false && Objects2Look4Tag.Count > 0) return;

        OnStay?.Invoke(other);
    }
}
