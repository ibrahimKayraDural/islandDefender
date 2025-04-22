using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnHoverOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("If true, invokes ''_OnHoverExit'' with an uninitialized Hover Data when the script gets disabled.")]
    [Space(15), SerializeField] bool _ActivateExitOnDisable = true;
    [SerializeField] List<float> _AdditionalFloatDatas;
    [Space(25)]
    [SerializeField] UnityEvent<HoverData> _OnHoverEnter;
    [SerializeField] UnityEvent<HoverData> _OnHoverExit;

    void OnDisable()
    {
        if (_ActivateExitOnDisable)
            _OnHoverExit?.Invoke(new HoverData());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _OnHoverEnter?.Invoke(new HoverData(eventData, _AdditionalFloatDatas));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _OnHoverExit?.Invoke(new HoverData(eventData, _AdditionalFloatDatas));
    }
    public struct HoverData
    {
        public PointerEventData EventData;
        public List<float> AdditionalFloatDatas;

        public HoverData(PointerEventData eventData, List<float> additionalFloatDatas)
        {
            EventData = eventData;
            AdditionalFloatDatas = additionalFloatDatas;
        }
    }
}
