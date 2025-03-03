using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWall : MonoBehaviour, IHealth
{
    [SerializeField] BaseManager _BaseManager;

    public float Health { get; }

    public void SetHealth(float setTo) => _BaseManager.SetHealth(setTo);
    public virtual void RemoveHealth(float amount) => _BaseManager.RemoveHealth(amount);
}
