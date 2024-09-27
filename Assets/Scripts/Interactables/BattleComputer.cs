using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleComputer : MonoBehaviour, IInteractable
{
    public string InteractDescription { get => "Manage battlefield"; set { } }

    [SerializeField] GameObject[] BattleGameObjects;
    [SerializeField] string _TargetCameraTag = GLOBAL.UnassignedString;
    [SerializeField] string _ReturnCameraTag = GLOBAL.UnassignedString;

    PlayerInstance _playerInstance
    {
        get
        {
            if(AUTO_playerInstance == null)
                AUTO_playerInstance = PlayerInstance.Instance;
            return AUTO_playerInstance;
        }
    }
    PlayerInstance AUTO_playerInstance = null;

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

    CanvasManager _canvasManager
    {
        get
        {
            if (AUTO_canvasManager == null)
                AUTO_canvasManager = CanvasManager.Instance;
            return AUTO_canvasManager;
        }
    }
    CanvasManager AUTO_canvasManager = null;

    public void OnInteracted(GameObject interactor)
    {
        SetBattleEnablity(true);
    }

    public void SetBattleEnablity(bool setTo)
    {
        foreach (var item in BattleGameObjects) item.SetActive(setTo);
        _playerInstance.PlayerController_Ref.enabled = !setTo;
        _playerInstance.PlayerInteractor_Ref.enabled = !setTo;
        _playerInstance.PlayerToolController_Ref.enabled = !setTo;
        _canvasManager.gameObject.SetActive(!setTo);
        _cameraManager.TrySetCameraWithTag(setTo ? _TargetCameraTag : _ReturnCameraTag, false);
    }
}
