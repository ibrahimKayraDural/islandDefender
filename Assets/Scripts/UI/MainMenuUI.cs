using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] SceneField LoadingScene;
    [SerializeField] SceneField PlayerScene;
    [SerializeField] SceneField StartLevel;

    [SerializeField] GameObject _DefaultCanvas;
    [SerializeField] GameObject _SettingsCanvas;
    [SerializeField] GameObject _CreditsCanvas;

    [SerializeField] Animator _CameraAnimator;

    public void Play()
    {
        SceneManager.LoadSceneAsync(LoadingScene, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(PlayerScene);
        SceneManager.LoadSceneAsync(StartLevel, LoadSceneMode.Additive);
    }
    public void ToDefaultCanvas()
    {
        StartCoroutine(SelectCanvas("default"));
    }
    public void ToSettingsCanvas()
    {
        StartCoroutine(SelectCanvas("settings"));
    }
    public void ToCreditsCanvas()
    {
        StartCoroutine(SelectCanvas("credits"));
    }
    public IEnumerator SelectCanvas(string canvasName)
    {
        _CameraAnimator.SetTrigger(canvasName);

        _DefaultCanvas.SetActive(false);
        _SettingsCanvas.SetActive(false);
        _CreditsCanvas.SetActive(false);

        float waitTime = .3f;
        yield return new WaitForSeconds(waitTime);

        canvasName = canvasName.ToLower();

        if (canvasName == "default") _DefaultCanvas.SetActive(true);
        if (canvasName == "settings") _SettingsCanvas.SetActive(true);
        if (canvasName == "credits") _CreditsCanvas.SetActive(true);
    }
    public void Exit()
    {
        Application.Quit();
    }
}