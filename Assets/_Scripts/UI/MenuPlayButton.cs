using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayButton : MonoBehaviour
{
    [SerializeField] List<SceneField> _ScenesToLoad;

    public void Play()
    {
        List<string> sceneStrings = new List<string>();
        foreach (var item in _ScenesToLoad) sceneStrings.Add(item);

        SceneLoader.Instance.LoadScenes(sceneStrings);
    }
}
