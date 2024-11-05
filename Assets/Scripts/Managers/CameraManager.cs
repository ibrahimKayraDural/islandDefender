using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; } = null;

    public event EventHandler<Camera> e_OnCameraChanged;
    public Camera CurrentCamera => _camerasWithTag.Count > 0 ? _camerasWithTag[_currentIndex].Value.Value.Item1 : null;
    public List<KeyValuePair<string, Tuple<Camera, AudioListener>>?> _camerasWithTag = new List<KeyValuePair<string, Tuple<Camera, AudioListener>>?>();

    [SerializeField] List<string> _IgnoreTags = new List<string>();
    [SerializeField] string _StartingCameraTag = GLOBAL.UnassignedString;
    [SerializeField, Min(0)] float _WalkDuration = .5f;
    [SerializeField, Min(0)] float _StopDuration = .5f;

    PlayerInstance _playerInstance
    {
        get
        {
            if (AUTO_playerInstance == null)
                AUTO_playerInstance = PlayerInstance.Instance;
            return AUTO_playerInstance;
        }
    }
    PlayerInstance AUTO_playerInstance = null;

    int _currentIndex = 0;

    bool _runLateStart = false;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        SceneLoader scLoader = SceneLoader.Instance;
        if (scLoader != null && scLoader.IsLoadingScenes)
        {
            scLoader.e_OnScenesAreLoaded += Initialize;
        }
        else
        {
            _runLateStart = true;
        }
    }

    void Start()
    {
        if (_runLateStart)
        {
            _runLateStart = false;
            Invoke(nameof(LateStart), .2f);
        }
    }

    void LateStart()
    {
        Initialize(null, EventArgs.Empty);
    }

    void Initialize(object sender, EventArgs e)
    {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (cameras.Length <= 0)
        {
            Debug.LogError("No cameras were found in scene");
            this.enabled = false;
            return;
        }

        foreach (var cam in cameras)
        {
            string tag = cam.gameObject.tag;
            if (_IgnoreTags.Contains(tag)) continue;

            cam.gameObject.TryGetComponent(out AudioListener al);

            _camerasWithTag.Add(new KeyValuePair<string, Tuple<Camera, AudioListener>>(tag, new Tuple<Camera, AudioListener>(cam, al)));
        }

        if (TrySetCameraWithTag(_StartingCameraTag, false) == false)
        {
            int idx = _camerasWithTag.FindIndex(x => x.Value.Value.Item1.enabled);

            if (idx != -1) SetCurrentIndex(idx, false);
        }

        RefreshCameras(false);

        SceneLoader scLoader = SceneLoader.Instance;
        if (sender != null && scLoader != null)
        {
            scLoader.e_OnScenesAreLoaded -= Initialize;
        }
    }

    void RefreshCameras(bool runAnimation = true, bool? lockPlayer = null)
    {
        for (int i = 0; i < _camerasWithTag.Count; i++)
        {
            bool enablity = _currentIndex == i;

            Camera cam = _camerasWithTag[i].Value.Value.Item1;
            AudioListener al = _camerasWithTag[i].Value.Value.Item2;

            if (cam == null)
            {
                _camerasWithTag.RemoveAt(i);
                continue;
            }

            cam.enabled = enablity;
            if (al) al.enabled = enablity;
        }

        if (lockPlayer != null) _playerInstance.SetPlayerIsLocked(lockPlayer.Value);

        if (runAnimation)
        {
            Overworld.PlayerController pc = PlayerInstance.Instance.PlayerController_Ref;
            pc.AddMovementModifierForSeconds("CameraManager2", Overworld.MovementMode.Locked, _WalkDuration + _StopDuration, true);
            pc.AddMovementModifierForSeconds("CameraManager1", Overworld.MovementMode.Repeating, _WalkDuration, true);
        }
    }

    public bool TrySetCameraWithTag(string tag, bool runAnimation = true, bool? lockPlayer = null)
    {
        if ((GLOBAL.StringHasValue(tag) && _camerasWithTag.Count > 0) == false) return false;

        var pair = _camerasWithTag.Find(x => x.Value.Key == tag);
        if (pair.HasValue == false) return false;

        return SetCurrentIndex(_camerasWithTag.FindIndex(x => x.Value.Key == tag), runAnimation, lockPlayer);
    }

    bool SetCurrentIndex(int value, bool runAnimation = true, bool? lockPlayer = null)
    {
        if (value < 0 || value >= _camerasWithTag.Count) return false;

        if (_currentIndex != value)
        {
            _currentIndex = value;
            RefreshCameras(runAnimation, lockPlayer);
            e_OnCameraChanged?.Invoke(this, CurrentCamera);
            return true;
        }

        return false;
    }
}
