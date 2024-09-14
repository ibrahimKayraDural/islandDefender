using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    CameraManager _CameraManager
    {
        get
        {
            if (AUTO_cameraManager == null)
                AUTO_cameraManager = CameraManager.Instance;
            return AUTO_cameraManager;
        }
    }
    CameraManager AUTO_cameraManager = null;

    bool _lookAtUpdate = false;

    public void Look(Camera cam = null)
    {
        if (cam == null)
        {
            if (_CameraManager == null) return;

            cam = _CameraManager.CurrentCamera;
        }

        Quaternion rot = Quaternion.LookRotation(cam.transform.position, Vector3.up);
        transform.rotation = rot;
    }

    public void SetLookAtUpdate(bool doLook)
    {
        if (doLook == _lookAtUpdate) return;

        StopCoroutine(UpdateIEnum());

        if(doLook)
        {
            StartCoroutine(UpdateIEnum());
        }

        _lookAtUpdate = doLook;
    }

    IEnumerator UpdateIEnum()
    {
        while(true)
        {
            yield return null;
            Look();
        }
    }
}
