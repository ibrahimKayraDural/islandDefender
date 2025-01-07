using Overworld;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Minable : MonoBehaviour
{
    [SerializeField] UnityEvent OnMineSuccessfull;

    [SerializeField] Cost[] _Gains;

    [SerializeField, Min(1)] int RequiredDrillLevel = 1;
    [SerializeField] float _MiningSFXCooldown = .25f;
    [SerializeField] float _MineDuration = 2;
    [SerializeField] float _ForgetDuration = .5f;
    [SerializeField] bool _DeleteFromTilemap;
    [SerializeField] ShaderFillScript _SPS;
    [SerializeField] AdjustableAudioClip _MiningSFX = AdjustableAudioClip.Default;
    [SerializeField] AdjustableAudioClip _MineCompletedSFX = AdjustableAudioClip.Default;

    float CurrentAmount
    {
        get => AUTO_currentAmount;
        set
        {
            value = Mathf.Clamp(value, 0, _MineDuration);
            AUTO_currentAmount = value;
            _SPS?.SetValue(CurrentAmount, _MineDuration);
        }
    }
    float AUTO_currentAmount = 0;
    float TargetTime_MiningSFX = -1;
    bool _isMining = false;
    bool _miningIsDone;

    void Start()
    {
        CurrentAmount = 0;
    }

    public void StartMining(float speedMultiplier, int drillLevel)
    {
        if (_isMining) return;
        if (drillLevel < RequiredDrillLevel) return;

        StopCoroutine(nameof(ForgetIEnum));
        StartCoroutine(nameof(MineIEnum), speedMultiplier);

        _isMining = true;
    }

    public void StopMining()
    {
        if (_isMining == false) return;

        StopCoroutine(nameof(MineIEnum));
        StartCoroutine(nameof(ForgetIEnum));

        _isMining = false;
    }

    IEnumerator MineIEnum(float speedMultiplier)
    {
        speedMultiplier = Mathf.Max(0, speedMultiplier);

        while (_miningIsDone == false)
        {
            float step = .05f;

            if (TargetTime_MiningSFX <= Time.time)
            {
                AudioManager.Instance.PlayClip(gameObject.name + "_mining",
                    _MiningSFX.AudioClip, volume: _MiningSFX.Volume, pitch: _MiningSFX.Pitch);

                TargetTime_MiningSFX = Time.time + _MiningSFXCooldown;
            }

            yield return new WaitForSeconds(step);
            CurrentAmount = CurrentAmount + step * speedMultiplier;
            if (CurrentAmount >= _MineDuration) MineSuccessful();
        }
    }
    IEnumerator ForgetIEnum()
    {
        yield return new WaitForSeconds(_ForgetDuration);
        CurrentAmount = 0;
        TargetTime_MiningSFX = -1;
    }

    void MineSuccessful()
    {
        StopAllCoroutines();
        _miningIsDone = true;

        AudioManager.Instance.PlayClip(gameObject.name + "_mineComplete",
                    _MineCompletedSFX.AudioClip, volume: _MineCompletedSFX.Volume, pitch: _MineCompletedSFX.Pitch);

        foreach (var item in _Gains)
        {
            PlayerInstance.Instance?.Inventory_Ref?.TryAddItemWithSpill(new ResourceItem(item.Resource, item.Amount), true);
        }

        OnMineSuccessfull?.Invoke();

        TilemapManager tm = TilemapManager.Instance;
        if (tm != null && _DeleteFromTilemap)
        {
            if (tm.DeleteTile(transform.position, "objects") == false) Destroy(gameObject);
        }
        else Destroy(gameObject);
    }
}
