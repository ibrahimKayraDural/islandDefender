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
    [SerializeField] bool _ShowDebugLogs;

    [SerializeField] SwarmData data;

    void Awake()
    {
        WriteDebug("Awake ran");
    }
    void Start()
    {
        WriteDebug("Start ran");
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

        _Tmesh.text = "";
        foreach (var wave in SpawnManager.CurrentSwarm.Waves)
        {
            foreach (var lane in wave.Lanes)
            {
                _Tmesh.text += lane.Enemies[0].Enemy.DisplayName +" - ";
            }
            _Tmesh.text += "\n";
        }
    }

    public void TestMethod()//this is hooked up with the button in the inspector
    {
        WriteDebug("Test Method was invoked");

        SpawnManager.CurrentSwarm.InsertSwarmValues(data.AsValue, 1);
    }

    void WriteDebug(string message)
    {
        if (_ShowDebugLogs == false) return;
        Debug.Log(message);
    }
    void WriteDebug(string[] messages)
    {
        if (_ShowDebugLogs == false) return;
        foreach (var msg in messages)
        {
            Debug.Log(msg);
        }
    }

    private void O_WasPressed()
    {
        WriteDebug("O_WasPressed");
    }

    private void P_WasPressed()
    {
        WriteDebug("P_WasPressed");
    }

    private void MMB_WasPressed()
    {
        WriteDebug("MMB_WasPressed");
    }

    private void RMB_WasPressed()
    {
        WriteDebug("RMB_WasPressed");
    }

    private void LMB_WasPressed()
    {
        WriteDebug("LMB_WasPressed");
    }

    private void D_WasPressed()
    {
        WriteDebug("D_WasPressed");
    }

    private void S_WasPressed()
    {
        WriteDebug("S_WasPressed");
    }

    private void A_WasPressed()
    {
        WriteDebug("A_WasPressed");
    }

    private void W_WasPressed()
    {
        WriteDebug("W_WasPressed");
    }
}
