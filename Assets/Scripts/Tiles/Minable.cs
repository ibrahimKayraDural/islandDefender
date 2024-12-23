using Overworld;
using System;
using System.Collections;
using UnityEngine;

public class Minable : MonoBehaviour
{
    [SerializeField] Cost[] _Gains;

    [SerializeField] float _MineDuration = 2;
    [SerializeField] float _ForgetDuration = .5f;
    [SerializeField] bool _DeleteFromTilemap;
    [SerializeField] ShaderFillScript _SPS;

    float CurrentAmount {
        get=> AUTO_currentAmount;
        set
        {
            value = Mathf.Clamp(value, 0, _MineDuration);
            AUTO_currentAmount = value;
            _SPS?.SetValue(CurrentAmount, _MineDuration);
        }
    }
    float AUTO_currentAmount = 0;
    bool _isMining = false;

    void Start()
    {
        CurrentAmount = 0;
    }

    public void StartMining()
    {
        if (_isMining) return;

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
    }

    void MineSuccessful()
    {
        StopAllCoroutines();

        foreach (var item in _Gains)
        {
            PlayerInstance.Instance?.Inventory_Ref?.TryAddItemWithSpill(new ResourceItem(item.Resource, item.Amount), true);
        }

        TilemapManager tm = TilemapManager.Instance;
        if (tm != null && _DeleteFromTilemap)
        {
            if(tm.DeleteTile(transform.position, "objects") == false) Destroy(gameObject);
        }
        else Destroy(gameObject);
    }
}
