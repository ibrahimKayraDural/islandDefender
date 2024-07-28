using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicButFastGun : OverworldGun
{
    public override void Shoot()
    {
        base.Shoot();
        Debug.Log("shootin ma basic (but fast) gun");
    }
}
