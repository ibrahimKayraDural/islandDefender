using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeedModifierHelper : MonoBehaviour
{
    [SerializeField] float value = 1f;
    [SerializeField] string ID = GLOBAL.UnassignedString;

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

    public void StartModifier()
    {
        if (ID == GLOBAL.UnassignedString) return;

        if (usedID != null) StopModifier();
        _playerController.AddSpeedModifier(ID, value, out usedID);
    }
    public void StopModifier()
    {
        if (usedID == null) return;
        _playerController.RemoveSpeedModifier(usedID);
        usedID = null;
    }
}
