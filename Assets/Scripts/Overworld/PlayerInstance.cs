using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInstance : MonoBehaviour
{
    public static PlayerInstance Instance { get; private set; } = null;

    public PlayerController PlayerController_Ref => _PlayerController;
    public PlayerInteractor PlayerInteractor_Ref => _PlayerInteractor;
    public Inventory Inventory_Ref => _Inventory;
    public PlayerToolController PlayerToolController_Ref => _PlayerToolController;

    [SerializeField] PlayerController _PlayerController;
    [SerializeField] PlayerInteractor _PlayerInteractor;
    [SerializeField] Inventory _Inventory;
    [SerializeField] PlayerToolController _PlayerToolController;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this)
        {
            Debug.Log("Multiple players found. Deleting excess...");
            Destroy(gameObject);
        }
    }
}
