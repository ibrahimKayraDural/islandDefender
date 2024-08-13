using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance { get; private set; }

    [SerializeField] Canvas _MainCanvas;
    [SerializeField] InventoryUIScript _Inventory;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    void Start()
    {
        _MainCanvas.worldCamera = Camera.main;

        SetInventoryEnablity(false);
    }

    #region Inventory
    public void ToggleInventory() => _Inventory.ToggleInventory();
    public void SetInventoryEnablity(bool setTo) => _Inventory.SetInventoryEnablity(setTo);
    public void RefreshInventory() => _Inventory.RefreshInventory();
    #endregion
}
