using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour, IUserInterface
{
    [SerializeField] Camera mapCam;
    [SerializeField] Camera miniMapCam;

    [SerializeField] RawImage mapRenderer;
    [SerializeField] RawImage miniMapRenderer;

    CanvasManager _CanvasManager
    {
        get
        {
            if (AUTO_canvasManager == null)
                AUTO_canvasManager = CanvasManager.Instance;

            return AUTO_canvasManager;
        }
    }
    CanvasManager AUTO_canvasManager = null;

    public bool IsOpen { get; set; }

    public void OnEnablityChanged(bool changedTo, List<string> optionalParameters = null)
    {
        mapCam.enabled = changedTo;
        mapRenderer.enabled = changedTo;
    }
    public void SetMinimapEnablity(bool setTo)
    {
        miniMapCam.enabled = setTo;
        miniMapRenderer.enabled = setTo;
    }

    public void SetEnablityGetter(bool setTo, List<string> optionalParameters) => (this as IUserInterface).SetEnablity(setTo, optionalParameters);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            _CanvasManager.SetMapEnablity(!IsOpen);
        }
    }
}
