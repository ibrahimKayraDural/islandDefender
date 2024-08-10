using Overworld;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Minable : MonoBehaviour
{
    [SerializeField] Cost[] _Gains;

    [SerializeField] float _MineDuration = 2;
    [SerializeField] float _ForgetDuration = .5f;

    [SerializeField] Slider _ProgressBar;

    float CurrentAmount {
        get=> AUTO_currentAmount;
        set
        {
            value = Mathf.Clamp(value, 0, _MineDuration);
            AUTO_currentAmount = value;
            _ProgressBar.value = value;
        }
    }
    float AUTO_currentAmount  =0;
    bool _isMining = false;

    void Start()
    {
        _ProgressBar.maxValue = _MineDuration;
        _ProgressBar.minValue = 0;
        CurrentAmount = 0;
        SetProgressBarVisibility(0);
    }

    public void StartMining()
    {
        if (_isMining) return;

        SetProgressBarVisibility(1);

        StopCoroutine(nameof(ForgetIEnum));
        StartCoroutine(nameof(MineIEnum));

        _isMining = true;
    }
    public void StopMining()
    {
        if (_isMining == false) return;

        StopCoroutine(nameof(MineIEnum));
        StartCoroutine(nameof(ForgetIEnum));

        _isMining = false;
    }

    IEnumerator MineIEnum()
    {
        while(true)
        {
            float step = .1f;
            yield return new WaitForSeconds(step);
            CurrentAmount = CurrentAmount + step;
            if (CurrentAmount == _MineDuration) MineSuccessful();
        }
    }
    IEnumerator ForgetIEnum()
    {
        yield return new WaitForSeconds(_ForgetDuration);
        CurrentAmount = 0;
        SetProgressBarVisibility(0);
    }
    void SetProgressBarVisibility(float setTo)
    {
        Image[] images = _ProgressBar.GetComponentsInChildren<Image>();

        foreach (var img in images)
        {
            Color color = img.color;
            color.a = Mathf.Clamp(setTo, 0, 1);
            img.color = color;
        }
    }
    void MineSuccessful()
    {
        StopAllCoroutines();

        foreach (var item in _Gains)
        {
            Inventory.Instance?.TryAddItemWithSpill(new ResourceItem(item.Resource, item.Amount));
        }

        Destroy(gameObject);
    }
}
