using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] GameObject mapCam;
    [SerializeField] GameObject miniMapCam;

    [SerializeField] GameObject mapRenderer;
    [SerializeField] GameObject miniMapRenderer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            mapCam.GetComponent<Camera>().enabled = !mapCam.GetComponent<Camera>().enabled;
            mapRenderer.GetComponent<RawImage>().enabled = !mapRenderer.GetComponent<RawImage>().enabled;
        }
    }
}
