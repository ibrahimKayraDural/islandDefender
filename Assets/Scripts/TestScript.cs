using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TowerDefence;

public class TestScript : MonoBehaviour
{
    [SerializeField] GameObject _Cube;
    [SerializeField] TextMeshProUGUI _Tmesh;
    [SerializeField] Image _Img;

    void Awake()
    {
        
    }
    void Start()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) P_WasPressed();
        else if (Input.GetKeyDown(KeyCode.O)) O_WasPressed();
        else if (Input.GetKeyDown(KeyCode.W)) W_WasPressed();
        else if (Input.GetKeyDown(KeyCode.A)) A_WasPressed();
        else if (Input.GetKeyDown(KeyCode.S)) S_WasPressed();
        else if (Input.GetKeyDown(KeyCode.D)) D_WasPressed();
        else if (Input.GetMouseButtonDown(0)) LMB_WasPressed();
        else if (Input.GetMouseButtonDown(1)) RMB_WasPressed();
        else if (Input.GetMouseButtonDown(2)) MMB_WasPressed();
    }

    private void MMB_WasPressed()
    {
        Debug.Log("MMB_WasPressed");
    }

    private void RMB_WasPressed()
    {
        Debug.Log("RMB_WasPressed");
    }

    private void LMB_WasPressed()
    {
        Debug.Log("LMB_WasPressed");
    }

    private void D_WasPressed()
    {
        Debug.Log("D_WasPressed");
    }

    private void S_WasPressed()
    {
        Debug.Log("S_WasPressed");
    }

    private void A_WasPressed()
    {
        Debug.Log("A_WasPressed");
    }

    private void W_WasPressed()
    {
        Debug.Log("W_WasPressed");
    }

    private void O_WasPressed()
    {
        Debug.Log("O_WasPressed");
    }

    private void P_WasPressed()
    {
        Debug.Log("P_WasPressed");
    }
}
