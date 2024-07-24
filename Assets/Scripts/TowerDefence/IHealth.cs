using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    float Health { get;}
    virtual public void RemoveHealth(float amount) => SetHealth(Health - amount);
    virtual public void AddHealth(float amount) => SetHealth(Health + amount);
    void SetHealth(float setTo);
}
