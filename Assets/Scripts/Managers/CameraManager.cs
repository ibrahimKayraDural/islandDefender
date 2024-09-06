using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; } = null;

    public event EventHandler<Camera> e_OnCameraChanged;
    public Camera CurrentCamera => _camerasWithTag[CurrentIndex].Value.Value.Item1;
    public int CurrentIndex 
    { 
        get => AUTO_currentIndex;
        set 
        {
            if (value < 0 || value >= _camerasWithTag.Count) return;

            if(AUTO_currentIndex != value)
            {
                AUTO_currentIndex = value;
                RefreshCameras();
                e_OnCameraChanged?.Invoke(this, CurrentCamera);
            }
        }
    }

    [SerializeField] string _StartingCameraTag = GLOBAL.UnassignedString;

    List<KeyValuePair<string, Tuple<Camera, AudioListener>>?> _camerasWithTag = new List<KeyValuePair<string, Tuple<Camera, AudioListener>>?>();
    int AUTO_currentIndex = 0;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var cam in cameras)
        {
            string tag = cam.gameObject.tag;
            cam.gameObject.TryGetComponent(out AudioListener al);

            _camerasWithTag.Add(new KeyValuePair<string, Tuple<Camera, AudioListener>>(tag, new Tuple<Camera, AudioListener>(cam, al)));
        }

        if (TrySetCameraWithTag(_StartingCameraTag) == false)
        {
            int idx = _camerasWithTag.FindIndex(x => x.Value.Value.Item1.enabled);

            if (idx != -1) CurrentIndex = idx;
        }

        RefreshCameras();
    }

    void RefreshCameras()
    {
        for (int i = 0; i < _camerasWithTag.Count; i++)
        {
            bool enablity = CurrentIndex == i;

            Camera cam = _camerasWithTag[i].Value.Value.Item1;
            AudioListener al = _camerasWithTag[i].Value.Value.Item2;

            cam.enabled = enablity;
            if (al) al.enabled = enablity;
        }
    }

    public void IncrementCameraIndex() => SetCameraIndex((CurrentIndex + 1) % _camerasWithTag.Count);
    public void DecrementCameraIndex() => SetCameraIndex((CurrentIndex - 1) >= 0 ? CurrentIndex - 1 : _camerasWithTag.Count - 1);
    public void SetCameraIndex(int setTo)=> CurrentIndex = setTo;
    public bool TrySetCameraWithTag(string tag)
    {
        if ((GLOBAL.StringHasValue(tag) && _camerasWithTag.Count > 0) == false) return false;

        var pair = _camerasWithTag.Find(x => x.Value.Key == tag);
        if (pair.HasValue == false) return false;

        CurrentIndex = _camerasWithTag.FindIndex(x => x.Value.Key == tag);
        return true;
    }
}
