using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour, IUserInterface
{
    [SerializeField] Camera mapCam;
    [SerializeField] Camera miniMapCam;

    [SerializeField] GameObject mapRenderer;
    [SerializeField] RawImage miniMapRenderer;
    [SerializeField] RawImage MapRenderer;

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
        MapRenderer.enabled = changedTo;
        mapRenderer.SetActive(changedTo);
        IsOpen = changedTo;
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

        if (IsOpen)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                mapCam.orthographicSize -= scroll * 10f;
                mapCam.orthographicSize = Mathf.Clamp(mapCam.orthographicSize, 5f, 50f); // Min-max zoom
            }
        }
    }
    
    public void RevealScannerIcons(ScannerUpgradeData data)
    {
        // data.RevealMineCount kadar mine yeri göster
        // data.RevealRuinCount kadar ruin yeri göster
    }

}
