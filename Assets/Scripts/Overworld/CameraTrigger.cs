using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] string _TagToEnter = GLOBAL.UnassignedString;
    [SerializeField] List<string> _TagsToInteract;

    CameraManager _camManager;
    string _tagToReturn = null;

    void Start()
    {
        _camManager = CameraManager.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_TagsToInteract.Contains(other.gameObject.tag) == false) return;

        _tagToReturn = _camManager.CurrentCamera.gameObject.tag;
        _camManager.TrySetCameraWithTag(_TagToEnter);
    }

    void OnTriggerExit(Collider other)
    {
        if (_tagToReturn == null) return;
        if (_TagsToInteract.Contains(other.gameObject.tag) == false) return;

        _camManager.TrySetCameraWithTag(_tagToReturn);
        _tagToReturn = null;
    }
}
