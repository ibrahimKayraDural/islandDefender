using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNotificator : MonoBehaviour
{
    [SerializeField] BaseManager _BaseMngr;

    bool _baseIsDead;

    void Start()
    {
        _BaseMngr.e_BaseHasDied += BaseIsDead;
        _BaseMngr.e_BaseIsAttacked += BaseIsAttacked;
    }

    public void BaseIsAttacked(object sender, float remainingHealth)
    {
        if (_baseIsDead) return;
        Debug.Log($"BASEA IS ATTACKED OMG GUYS ONLY {remainingHealth} HEALTH IS LEFT NOOOOOOOOO");
    }
    public void BaseIsDead(object sender, bool _)
    {
        Debug.Log("base is dead 3(");
        _baseIsDead = true;
    }
}
