using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventTrigger : MonoBehaviour, IInteractable
{
    public string InteractDescription { get => _InteractDescription; set { } }
    [SerializeField] string _InteractDescription = GLOBAL.UnassignedString;
    [SerializeField] GameObject[] _InstantiateOnActivation;

    [Space(25)]

    [SerializeField] UnityEvent e_OnInteracted;
    [SerializeField] UnityEvent<GameObject> e_OnInteractedWithValue;

    public void OnInteracted(GameObject interactor)
    {
        e_OnInteracted?.Invoke();
        e_OnInteractedWithValue?.Invoke(interactor);

        foreach (var item in _InstantiateOnActivation)
        {
            Instantiate(item, transform.position, transform.rotation);
        }
    }
}
