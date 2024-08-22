using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.Collections;

public class GraphicRaycasterScript : MonoBehaviour
{
    public event EventHandler<List<RaycastResult>> e_OnEventDataGathered;
    public bool RunOnUpdate
    {
        get => AUTO_runOnUpdate;
        set
        {
            AUTO_runOnUpdate = value;

            StopCoroutine(nameof(UpdateIEnum));

            if(AUTO_runOnUpdate) StartCoroutine(nameof(UpdateIEnum));
        }
    }
    bool AUTO_runOnUpdate = true;

    [SerializeField] GraphicRaycaster _Raycaster;
    [SerializeField] EventSystem _EventSystem;

    void Start()
    {
        if (_Raycaster == null || _EventSystem == null)
        {
            Debug.LogError("No raycaster/eventSystem was assigned");
            this.enabled = false;
        }

        if (RunOnUpdate) StartCoroutine(nameof(UpdateIEnum));
    }

    private void OnEnable()
    {
        if (RunOnUpdate) StartCoroutine(nameof(UpdateIEnum));
    }

    IEnumerator UpdateIEnum()
    {
        while(true)
        {
            Raycast();
            yield return null;
        }
    }

    public List<RaycastResult> Raycast()
    {
        PointerEventData pointerEventData = new PointerEventData(_EventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        _Raycaster.Raycast(pointerEventData, results);

        e_OnEventDataGathered?.Invoke(this, results);
        return results;
    }
}