using SaveSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        SceneManager.activeSceneChanged += CheckSave;

        OnLoaded?.Invoke();
    }
    void OnApplicationQuit()
    {
        SceneManager.activeSceneChanged -= CheckSave;
        OnSaved?.Invoke();
        _isSaved = true;
    }
    void CheckSave(Scene oldScene, Scene newScene)
    {
        SceneManager.activeSceneChanged -= CheckSave;

        if (_isSaved == false) OnSaved?.Invoke();
    }
}
