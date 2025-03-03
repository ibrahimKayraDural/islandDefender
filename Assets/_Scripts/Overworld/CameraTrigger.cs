using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] string _TagToEnter = GLOBAL.UnassignedString;
    [SerializeField] List<string> _TagsToInteract;

    CameraManager _CamManager
    {
        get
        {
            if (AUTO_camManager == null)
                AUTO_camManager = CameraManager.Instance;

            return AUTO_camManager;
        }
    }
    CameraManager AUTO_camManager =null;

    string _tagToReturn = null;

    void OnTriggerEnter(Collider other)
    {
        if (_TagsToInteract.Contains(other.gameObject.tag) == false) return;

        _tagToReturn = _CamManager.CurrentCamera.gameObject.tag;
        _CamManager.TrySetCameraWithTag(_TagToEnter);
    }

    void OnTriggerExit(Collider other)
    {
        if (_tagToReturn == null) return;
        if (_TagsToInteract.Contains(other.gameObject.tag) == false) return;

        _CamManager.TrySetCameraWithTag(_tagToReturn);
        _tagToReturn = null;
    }
}
