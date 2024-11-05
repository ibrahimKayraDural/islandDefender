using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; } = null;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    [SerializeField] GameObject[] _BattleGameObjects;
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

    public void EnterBattle() => EnterBattle(_cameraManager.CurrentCamera.gameObject.tag);
    public void EnterBattle(string cameraTagToReturn)
    {
        _returnCameraTag = cameraTagToReturn;

        foreach (var item in _BattleGameObjects) item.SetActive(true);
        _cameraManager.TrySetCameraWithTag(_TargetCameraTag, false, true);
    }
    public void ExitBattle()
    {
        if (_returnCameraTag == GLOBAL.UnassignedString) return;

        foreach (var item in _BattleGameObjects) item.SetActive(false);
        _cameraManager.TrySetCameraWithTag(_returnCameraTag, false, false);

        _returnCameraTag = GLOBAL.UnassignedString;
    }
}
