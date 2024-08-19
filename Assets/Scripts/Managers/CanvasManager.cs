using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameUI;
using System;

public class CanvasManager : MonoBehaviour
{
    public static bool SomethingIsOpen => CurrentInterface != null;
    public static UserInterface CurrentInterface
    {
        get => AUTO_currentInterface;
        set
        {
            if (AUTO_currentInterface == null) AUTO_currentInterface = value;
            else if (value == null) AUTO_currentInterface = null;
            else return;
            Instance.OnCurrentInterfaceChanged(AUTO_currentInterface);
        }
    }
    static UserInterface AUTO_currentInterface = null;

    public static CanvasManager Instance { get; private set; }

    [SerializeField] Canvas _MainCanvas;
    [SerializeField] InventoryUIScript _Inventory;
    [SerializeField] ChestUIScript _ChestUI;
    [SerializeField] InteractableHelperUI _InteractableHelper;

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

    void OnCurrentInterfaceChanged(UserInterface changedTo)
    {

    }

    #region Inventory
    public void ToggleInventory()
    {
        _Inventory.ToggleInventory();
    }

    public void SetInventoryEnablity(bool setTo)
    {
        _Inventory.SetEnablityGetter(setTo);
    }
    public void RefreshInventory() => _Inventory.RefreshInventory();
    #endregion

#nullable enable
    public void SetInteractionText(string? text)
    {
        _InteractableHelper.SetHelperText(text == null ? "" : text);
        _InteractableHelper.SetHelperEnablity(text != null);
    }
#nullable disable

    public void SetChestUIEnablity(bool setTo)
    {
        _ChestUI.SetEnablityGetter(setTo);
    }
}
