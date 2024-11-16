using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    float Health { get;}
    virtual void RemoveHealth(float amount) => SetHealth(Health - amount);
    virtual void AddHealth(float amount) => SetHealth(Health + amount);
    void SetHealth(float setTo);
}
