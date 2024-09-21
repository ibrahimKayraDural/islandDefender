using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProximityInteractableUI : MonoBehaviour, IUserInterface
{
    public ProximityInteractable CurrentPI { get; private set; } = null;
    public bool IsOpen { get; set; }

    internal List<KeyCode> UICloseKeys = new List<KeyCode>() {
            KeyCode.I
        };

    public abstract void OnEnablityChanged(bool changedTo);

    public void SetEnablityGetter(bool setTo) => (this as IUserInterface).SetEnablity(setTo);

    public bool TrySetProximityInteractor(ProximityInteractable setTo)
    {
        if (CurrentPI == null)//Proximity Interactable is opened
        {
            CurrentPI = setTo;
            StartCoroutine(nameof(PIUpdate));
        }
        else if (setTo == null)//Proximity Interactable is closed
        {
            CurrentPI = null;
            _breakChestUpdate = true;
        }
        else
        {
            return false;
        }

        return true;
    }

    internal bool _breakChestUpdate = false;

    internal abstract void OnPIUpdate_Start();
    internal abstract void OnPIUpdate_Loop();
    internal abstract void OnPIUpdate_End();

    IEnumerator PIUpdate()
    {
        yield return new WaitForSeconds(.1f);

        OnPIUpdate_Start();

        //Update is the inside of this loop.
        //Code above will run once before the update loop.
        while (_breakChestUpdate == false)
        {
            HandleInput();
            OnPIUpdate_Loop();

            yield return null;
        }
        //Update is the inside of the loop above.
        //Code below will run once after the update loop has ended.

        OnPIUpdate_End();

        _breakChestUpdate = false;
    }

    internal virtual void HandleInput()
    {
        foreach (var key in UICloseKeys)
        {
            if (Input.GetKeyDown(key))
            {
                CurrentPI.SetOpennes(false);
                return;
            }
        }
        if (Input.GetButtonDown("Interact") || Input.GetButtonDown("Exit"))
        {
            CurrentPI.SetOpennes(false);
            return;
        }
    }
}
