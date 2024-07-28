using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OverworldGun : MonoBehaviour
{
    public string DisplayName => _DisplayName;
    public string ID => _ID;

    [SerializeField,Min(0)] internal float _ShootCooldown = 1;
    [SerializeField] internal Transform _Barrel;
    [SerializeField] string _DisplayName = GLOBAL.UnassignedString;
    [SerializeField] string _ID = GLOBAL.UnassignedString;
    [SerializeField] GameObject[] Visuals;

    bool _isEquipped;
    float _shoot_TargetTime = -1;

    virtual public void Initialize(Transform equipPoint)
    {
        transform.parent = equipPoint;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        foreach (var item in Visuals) item.SetActive(false);
    }
    virtual public void Equip() 
    {
        if (_isEquipped) return;

        foreach (var item in Visuals) item.SetActive(true);

        _isEquipped = true;
    }
    virtual public void Unequip()
    {
        if (_isEquipped == false) return;

        foreach (var item in Visuals) item.SetActive(false);

        _isEquipped = false;
    }
    virtual public void Shoot() 
    {
        if (_isEquipped == false) return;
        if (_shoot_TargetTime > Time.time) return;

        _shoot_TargetTime = Time.time + _ShootCooldown;
    }
}
