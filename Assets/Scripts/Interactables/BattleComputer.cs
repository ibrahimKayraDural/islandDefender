using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleComputer : MonoBehaviour, IInteractable
{
    public string InteractDescription { get => "Manage battlefield"; set { } }

    CameraManager _cameraManager
    {
        get
        {
            if (AUTO_cameraManager == null)
                AUTO_cameraManager = CameraManager.Instance;
            return AUTO_cameraManager;
        }
    }
    CameraManager AUTO_cameraManager = null;

    BattleManager _battleManager
    {
        get
        {
            if (AUTO_battleManager == null)
                AUTO_battleManager = BattleManager.Instance;
            return AUTO_battleManager;
        }
    }
    BattleManager AUTO_battleManager = null;

    public void OnInteracted(GameObject interactor)
    {
        _battleManager.EnterBattle();
    }
}
