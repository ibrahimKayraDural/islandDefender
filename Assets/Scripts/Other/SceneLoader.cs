using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; } = null;

    public event EventHandler e_OnScenesAreLoaded;
    public bool IsLoadingScenes { get; private set; } = false;

    [SerializeField] SceneField _LoadingScene;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadScenes(List<string> scenes)
    {
        List<AsyncOperation> operations = new List<AsyncOperation>();

        operations.Add(SceneManager.LoadSceneAsync(_LoadingScene));

        foreach (var scene in scenes)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            operations.Add(op);
        }

        IsLoadingScenes = true;
        StartCoroutine(SceneProgressChecker(operations));
    }

    IEnumerator SceneProgressChecker(List<AsyncOperation> operations)
    {
        for (int i = 0; i < operations.Count; i++)
        {
            while(operations[i].isDone == false)
            {
                yield return null;
            }
        }

        //All scenes are loaded

        IsLoadingScenes = false;
        e_OnScenesAreLoaded?.Invoke(this, EventArgs.Empty);

        //Wait a moment for good measure
        yield return new WaitForSeconds(.1f);
        SceneManager.UnloadSceneAsync(_LoadingScene);
    }
}
