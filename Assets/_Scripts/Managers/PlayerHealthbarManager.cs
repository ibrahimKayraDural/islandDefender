using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbarManager : MonoBehaviour
{
    [SerializeField] GameObject _Visuals;
    [SerializeField] Slider _Slider;

    public void SetEnablity(bool setTo)
    {
        _Visuals.SetActive(setTo);
    }

    public void SetSliderValue(float setTo)
    {
        _Slider.value = setTo;
    }
}
