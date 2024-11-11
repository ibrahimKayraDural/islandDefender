using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; } = null;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    [SerializeField] TDPlayerController _TDPlayerController;
    [SerializeField] string _TargetCameraTag = GLOBAL.UnassignedString;

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

    string _returnCameraTag = GLOBAL.UnassignedString;

    public void EnterBattle(TowerDefenceControlMode mode) => EnterBattle(_cameraManager.CurrentCamera.gameObject.tag, mode);
    public void EnterBattle(string cameraTagToReturn, TowerDefenceControlMode mode)
    {
        _returnCameraTag = cameraTagToReturn;

        _TDPlayerController.EnterBattle(mode);

        _cameraManager.TrySetCameraWithTag(_TargetCameraTag, false, true);
    }
    public void ExitBattle()
    {
        if (_returnCameraTag == GLOBAL.UnassignedString) return;

        _TDPlayerController.ExitBattle();

        _cameraManager.TrySetCameraWithTag(_returnCameraTag, false, false);

        _returnCameraTag = GLOBAL.UnassignedString;
    }
}
