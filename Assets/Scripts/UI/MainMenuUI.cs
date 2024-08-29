using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] SceneField LoadingScene;
    [SerializeField] SceneField PlayerScene;
    [SerializeField] SceneField StartLevel;

    public void Play()
    {
        SceneManager.LoadSceneAsync(LoadingScene, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(PlayerScene);
        SceneManager.LoadSceneAsync(StartLevel, LoadSceneMode.Additive);
    }
    public void Settings()
    {

    }
    public void Credits()
    {

    }
    public void Exit()
    {
        Application.Quit();
    }
}
