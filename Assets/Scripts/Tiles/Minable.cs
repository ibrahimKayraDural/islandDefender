using Overworld;
using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public struct AdjustableAudioClip
{
    public AudioClip @AudioClip;
    [Range(-3, 3)] public float Pitch;
    [Range(0, 1)] public float Volume;

    public static AdjustableAudioClip Default
    {
        get
        {
            var @return = new AdjustableAudioClip();
            @return.Pitch = 1;
            @return.Volume = 1;
            return @return;
        }
    }
}
public class Minable : MonoBehaviour
{
    [SerializeField] Cost[] _Gains;

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
    float TargetTime_Mining = -1;
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
        while (true)
        {
            float step = .1f;

            if (TargetTime_Mining <= Time.time)
            {
                AudioManager.Instance.PlayClip(gameObject.name + "_mining",
                    _MiningSFX.AudioClip, volume: _MiningSFX.Volume, pitch: _MiningSFX.Pitch);

                TargetTime_Mining = Time.time + _MiningSFXCooldown;
            }

            yield return new WaitForSeconds(step);
            CurrentAmount = CurrentAmount + step;
            if (CurrentAmount == _MineDuration) MineSuccessful();
        }
    }
    IEnumerator ForgetIEnum()
    {
        yield return new WaitForSeconds(_ForgetDuration);
        CurrentAmount = 0;
        TargetTime_Mining = -1;
    }

    void MineSuccessful()
    {
        StopAllCoroutines();

        AudioManager.Instance.PlayClip(gameObject.name + "_mineComplete",
                    _MineCompletedSFX.AudioClip, volume: _MineCompletedSFX.Volume, pitch: _MineCompletedSFX.Pitch);

        foreach (var item in _Gains)
        {
            PlayerInstance.Instance?.Inventory_Ref?.TryAddItemWithSpill(new ResourceItem(item.Resource, item.Amount), true);
        }

        TilemapManager tm = TilemapManager.Instance;
        if (tm != null && _DeleteFromTilemap)
        {
            if (tm.DeleteTile(transform.position, "objects") == false) Destroy(gameObject);
        }
        else Destroy(gameObject);
    }
}
