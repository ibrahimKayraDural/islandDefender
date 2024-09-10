using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInteractor : MonoBehaviour
{
    CanvasManager _canvasManager;

    void Awake()
    {
        SceneLoader scLoader = SceneLoader.Instance;
        if (scLoader != null && scLoader.IsLoadingScenes)
        {
            scLoader.e_OnScenesAreLoaded += OnScenesAreLoaded;
        }
        else
        {
            StartCoroutine(DelayedSceneLoaded(.2f));
        }
    }

    IEnumerator DelayedSceneLoaded(float delay)
    {
        yield return new WaitForSeconds(delay);

        OnScenesAreLoaded(this, System.EventArgs.Empty);
    }

    void OnScenesAreLoaded(object sender, System.EventArgs e)
    {
        _canvasManager = CanvasManager.Instance;

        SceneLoader scLoader = SceneLoader.Instance;
        if (sender != this as object && scLoader != null)
        {
            scLoader.e_OnScenesAreLoaded -= OnScenesAreLoaded;
        }
    }

    void Update()
    {
        if (CanvasManager.SomethingIsOpen) return;
        if (Time.timeScale <= 0) return;

        IInteractable currentInteractable = null;
        Vector3 targetOrigin = transform.position;
        targetOrigin.y = -10;
        Ray ray = new Ray(targetOrigin, Vector3.up);
        foreach (var item in Physics.RaycastAll(ray, 20, ~0, QueryTriggerInteraction.Collide))
        {
            if (item.collider.TryGetComponent(out currentInteractable)) break;
        }

        if (Input.GetButtonDown("Interact"))
        { currentInteractable?.OnInteracted(gameObject); }

        _canvasManager?.SetInteractionText(currentInteractable == null ? null : currentInteractable.InteractDescription);
    }
}
