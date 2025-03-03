using System;
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
    }
    public void BaseIsDead(object sender, EventArgs e)
    {
        _baseIsDead = true;
    }
}
