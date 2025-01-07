using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeedModifierHelper : MonoBehaviour
{
    PlayerInstance _playerInstance
    {
        get
        {
            if (AUTO_playerInstance == null)
                AUTO_playerInstance = PlayerInstance.Instance;
            return AUTO_playerInstance;
        }
    }
    PlayerInstance AUTO_playerInstance = null;

    PlayerController _playerController => _playerInstance.PlayerController_Ref;

    string usedID = null;

    public void StartModifier(string id, float value)
    {
        if (usedID != null) EndModifier();
        _playerController.AddSpeedModifier(id, value, out usedID);
    }
    public void EndModifier()
    {
        if (usedID == null) return;
        _playerController.RemoveSpeedModifier(usedID);
        usedID = null;
    }
}
