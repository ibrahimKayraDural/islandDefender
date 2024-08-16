using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class GraphicRaycasterScript : MonoBehaviour
{
    public event EventHandler<List<RaycastResult>> e_OnEventDataGathered;

    [SerializeField] GraphicRaycaster _Raycaster;
    [SerializeField] EventSystem _EventSystem;

    void Start()
    {
        if (_Raycaster == null || _EventSystem == null)
        {
            Debug.LogError("No raycaster/eventSystem was assigned");
            this.enabled = false;
        }
    }

    void Update()
    {
        PointerEventData pointerEventData = new PointerEventData(_EventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        _Raycaster.Raycast(pointerEventData, results);
        e_OnEventDataGathered?.Invoke(this, results);
    }
}