using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerFuelController : MonoBehaviour
{
    public int CurrentFuel => _currentFuel;
    public bool FuelEmpty => _currentFuel <= 0;

    [SerializeField] int _BaseFuelMax = 5;
    [SerializeField] PlayerCanvasSlider _Slider;

    int _currentFuel = 0;

    public void Awake()
    {
        _currentFuel = _BaseFuelMax;
        _Slider.SetSliderMaxValue(_BaseFuelMax);
        _Slider.SetSliderValue(_BaseFuelMax);
    }

    public bool TrySpendFuel()
    {
        if (FuelEmpty) return false;

        _currentFuel--;
        _Slider.SetSliderValue(_currentFuel);
        return true;
    }
}
