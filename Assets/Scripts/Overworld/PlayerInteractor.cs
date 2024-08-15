using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInteractor : MonoBehaviour
{
    CanvasManager _canvasManager;

    void Start()
    {
        _canvasManager = CanvasManager.Instance;
    }
    void Update()
    {
        IInteractable currentInteractable = null;
        Vector3 targetOrigin = transform.position;
        targetOrigin.y = -10;
        Ray ray = new Ray(targetOrigin, Vector3.up);
        foreach (var item in Physics.RaycastAll(ray, 20, ~0, QueryTriggerInteraction.Collide))
        {
            if (item.collider.TryGetComponent(out currentInteractable)) break;
        }

        if (Input.GetButtonDown("Interact"))
        { currentInteractable?.OnInteracted(); }

        _canvasManager.SetInteractionText(currentInteractable == null ? null : currentInteractable.InteractDescription);
    }
}
