using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : OverworldGun
{
    public override void Shoot()
    {
        base.Shoot();
        Debug.Log("shootin ma basic gun");
    }
}
