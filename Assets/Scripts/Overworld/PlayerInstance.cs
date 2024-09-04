using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInstance : MonoBehaviour
{
    public static PlayerInstance Instance { get; private set; } = null;

    public PlayerController PlayerControllerREF => _PlayerController;
    public PlayerInteractor PlayerInteractorREF => _PlayerInteractor;
    public Inventory InventoryREF => _Inventory;
    public PlayerToolController PlayerToolControllerREF => _PlayerToolController;

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
