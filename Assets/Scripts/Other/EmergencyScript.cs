using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmergencyScript : MonoBehaviour
{
    [SerializeField] string playerTag = "OverworldPlayer";
    [SerializeField] SceneField sceneField;

    bool _playerIsIn;

    void Update()
    {
        if (_playerIsIn == false) return;

        if (Input.GetKeyDown(KeyCode.F)) EndGame();
    }
    void EndGame()
    {
        SceneManager.LoadScene(sceneField);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != playerTag) return;
        _playerIsIn = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != playerTag) return;
        _playerIsIn = false;
    }
}
