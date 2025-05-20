using SaveSystem;
using UnityEngine;
using UnityEngine.Events;

public class SaveHandler : MonoBehaviour
{
    [SerializeField] UnityEvent OnSaved;
    [SerializeField] UnityEvent OnLoaded;

    SaveManager _SaveManager
    {
        get
        {
            if (AUTO_saveManager == null)
                AUTO_saveManager = SaveManager.Instance;

            return AUTO_saveManager;
        }
    }
    SaveManager AUTO_saveManager = null;

    bool _isSaved;//prevents race conditions

    void Start()
    {
        OnLoaded?.Invoke();
    }
    void OnApplicationQuit()
    {
        OnSaved?.Invoke();
        _isSaved = true;
    }
    void OnDestroy()
    {
        if (_isSaved == false) OnSaved?.Invoke();
    }
}
