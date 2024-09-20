using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameUI;
using System;

public class CanvasManager : MonoBehaviour
{
    public static event EventHandler<IUserInterface> e_OnCurrentInterfaceChanged;

    public static bool SomethingIsOpen => CurrentInterface != null;
    public static IUserInterface CurrentInterface
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
    static IUserInterface AUTO_currentInterface = null;

    public static CanvasManager Instance { get; private set; }

    [SerializeField] Canvas _MainCanvas;
    [SerializeField] InventoryUIScript _InventoryUI;
    [SerializeField] ChestUIScript _ChestUI;
    [SerializeField] ToolRackUI _ToolRackUI;
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

    //Do not forget to register new Proximity Interactables here
    #region ProximityInteractable

    public bool TrySetCurrentProximityInteractabe(ProximityInteractable sender, bool setToNull = false)
    {
        if (sender == null) return false;

        Type type = sender.GetType();

        if (type == typeof(ChestScript)) return _ChestUI.TrySetCurrentChest(setToNull ? null : sender as ChestScript);
        else if (type == typeof(ToolRack)) return _ToolRackUI.TrySetCurrentRack(setToNull ? null : sender as ToolRack);

        return false;
    }

    public void SetProximityInteractableUIEnablity(ProximityInteractable sender, bool setTo)
    {
        if (sender == null) return;

        Type type = sender.GetType();

        if (type == typeof(ChestScript)) _ChestUI.SetEnablityGetter(setTo);
        else if (type == typeof(ToolRack)) _ToolRackUI.SetEnablityGetter(setTo);
    }

    #endregion

    #region Inventory
    public void ToggleInventory()
    {
        _InventoryUI.ToggleInventory();
    }

    public void SetInventoryEnablity(bool setTo)
    {
        _InventoryUI.SetEnablityGetter(setTo);
    }
    public void RefreshInventory() => _InventoryUI.RefreshInventory();
    #endregion

    #region Interaction Text
#nullable enable

    public void SetInteractionText(string? text)
    {
        if (SomethingIsOpen) text = null;
        if (text != null && (text == GLOBAL.UnassignedString || text == "")) text = null; 

        _InteractableHelper.SetHelperText(text == null ? "" : text);
        _InteractableHelper.SetHelperEnablity(text != null);
    }

#nullable disable
    #endregion
}
