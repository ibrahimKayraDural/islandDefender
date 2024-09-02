using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProximityInteractable : MonoBehaviour, IInteractable
{
    abstract public string InteractDescription { get; set; }

    [SerializeField] internal Animator b_Animator;
    [SerializeField] internal float b_ForgetDistance = 1;

    internal Transform b_currentInteractor = null;
    internal bool b_isOpen = false;

    virtual public void OnInteracted(GameObject interactor)
    {
        b_currentInteractor = interactor.transform;
        SetOpennes(true);
    }
    virtual public void SetOpennes(bool setTo)
    {
        if (b_isOpen == setTo) return;

        b_isOpen = setTo;

        if (b_Animator == false) Debug.LogError("an Animator is not assigned");
        else b_Animator.SetBool("IsOpen", setTo);

        if (setTo) OnOpened();
        else OnClosed();
    }

    virtual internal void OnOpened()
    {
        StartCoroutine(nameof(CheckForget));
    }
    virtual internal void OnClosed()
    {
        StopCoroutine(nameof(CheckForget));
        b_currentInteractor = null;
    }
    virtual internal IEnumerator CheckForget()
    {
        Vector3 pos = transform.position;
        while (true)
        {
            if (Vector3.Distance(b_currentInteractor.position, pos) >= b_ForgetDistance)
            {
                SetOpennes(false);
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}
