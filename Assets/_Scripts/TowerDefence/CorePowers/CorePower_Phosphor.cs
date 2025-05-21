using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorePower_Phosphor : CorePower_Base
{
    internal override void OnActivated()
    {
        Debug.Log("Used phosphor");
    }
}
