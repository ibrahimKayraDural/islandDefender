using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// -----  IMPORTANT INFO  -----

//Do not forget to register new proximityInteractables at CanvasManager/TrySetCurrentProximityInteractabe()

public abstract class ProximityInteractable : MonoBehaviour, IInteractable
{
    abstract public string InteractDescription { get; set; }

    [SerializeField] internal Animator b_Animator;
    [SerializeField] internal float b_ForgetDistance = 1;

    internal CanvasManager CanvasManagerGetter
    {
        get
        {
            if (AUTO_canvasManager == null)
                AUTO_canvasManager = CanvasManager.Instance;

            return AUTO_canvasManager;
        }
    }
    CanvasManager AUTO_canvasManager = null;

    internal Transform b_currentInteractor = null;
    internal bool b_isOpen = false;

    virtual public void OnInteracted(GameObject interactor)
    {
        if (CanvasManagerGetter.TrySetCurrentProximityInteractabe(this) == false) return;

        b_currentInteractor = interactor.transform;
        SetOpennes(true);
    }
    virtual public void SetOpennes(bool setTo)
    {
        if (b_isOpen == setTo) return;

        CanvasManagerGetter.SetProximityInteractableUIEnablity(this, setTo);

        if (b_Animator) b_Animator.SetBool("IsOpen", setTo);

        if (setTo) OnOpened();
        else OnClosed();

        b_isOpen = setTo;
    }

    virtual internal void OnOpened()
    {
        StartCoroutine(nameof(CheckForget));
    }
    virtual internal void OnClosed()
    {
        StopCoroutine(nameof(CheckForget));
        b_currentInteractor = null;

        CanvasManagerGetter.TrySetCurrentProximityInteractabe(this, true);
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
