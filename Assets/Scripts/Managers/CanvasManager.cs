using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameUI;
using System;

public class CanvasManager : MonoBehaviour
{
    public static event EventHandler<UserInterface> e_OnCurrentInterfaceChanged;

    public static bool SomethingIsOpen => CurrentInterface != null;
    public static UserInterface CurrentInterface
    {
        get => AUTO_currentInterface;
        set
        {
            if (AUTO_currentInterface == null) AUTO_currentInterface = value;
            else if (value == null) AUTO_currentInterface = null;
            else return;

            e_OnCurrentInterfaceChanged?.Invoke(Instance, AUTO_currentInterface);
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

    #region Interaction Text
#nullable enable

    public void SetInteractionText(string? text)
    {
        if (SomethingIsOpen) text = null;

        _InteractableHelper.SetHelperText(text == null ? "" : text);
        _InteractableHelper.SetHelperEnablity(text != null);
    }

#nullable disable
    #endregion

    #region Chest UI

    public void SetChestUIEnablity(bool setTo)
    {
        _ChestUI.SetEnablityGetter(setTo);
    }
    public bool TrySetCurrentChestOfChestUI(ChestScript setTo) => _ChestUI.TrySetCurrentChest(setTo);

    #endregion
}
