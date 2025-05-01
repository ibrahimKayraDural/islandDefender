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

            OnCurrentInterfaceChanged();
        }
    }
    static IUserInterface AUTO_currentInterface = null;

    public static CanvasManager Instance { get; private set; }

    [SerializeField] Canvas _MainCanvas;
    [SerializeField] ChestUIScript _ChestUI;
    [SerializeField] ToolRackUI _ToolRackUI;
    [SerializeField] DoorInteractableUI _DoorUI;
    [SerializeField] CrackInteractableUI _CrackUI;
    [SerializeField] WorkBenchInteractableUI _WorkBenchUI;
    [SerializeField] InteractableHelperUI _InteractableHelper;
    [SerializeField] MapManager _MapManager;
    [SerializeField] InventoryUIScript _InventoryUI;
    [SerializeField] PlayerCanvasSlider _PlayerHealthbarManager;
    [SerializeField] PlayerCanvasSlider _PlayerFuelbarManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }
    void Start()
    {
        _MainCanvas.worldCamera = Camera.main;

        //SetInventoryEnablity(false);
    }

    static void OnCurrentInterfaceChanged()
    {
        Instance.SetHealthbarEnablity(!SomethingIsOpen);
        Instance.SetFuelbarEnablity(!SomethingIsOpen);
        Instance.SetMinimapEnablity(!SomethingIsOpen);
        Instance.SetInventoryEnablity(!SomethingIsOpen);

        e_OnCurrentInterfaceChanged?.Invoke(Instance, AUTO_currentInterface);
    }
    public void SetEnablity(bool setTo)
    {
        gameObject.SetActive(setTo);
    }

    //Do not forget to register new Proximity Interactables here
    #region ProximityInteractable

    public bool TrySetCurrentProximityInteractabe(ProximityInteractable sender, bool setToNull = false)
    {
        ProximityInteractableUI PIUI = GetProximityInteractorUI(sender);
        if (PIUI == null) return false;

        return PIUI.TrySetProximityInteractor(setToNull ? null : sender);
    }

    public void SetProximityInteractableUIEnablity(ProximityInteractable sender, bool setTo, List<string> optionalParameters)
    {
        ProximityInteractableUI PIUI = GetProximityInteractorUI(sender);
        if (PIUI == null) return;

        PIUI.SetEnablityGetter(setTo, optionalParameters);
    }

    //register zone
    ProximityInteractableUI GetProximityInteractorUI(ProximityInteractable sender)
    {
        if (sender == null) return null;

        Type type = sender.GetType();

        if (type == typeof(ChestScript)) return _ChestUI;
        else if (type == typeof(ToolRack)) return _ToolRackUI;
        else if (type == typeof(DoorInteractable)) return _DoorUI;
        else if (type == typeof(WorkBench)) return _WorkBenchUI;
        else if (type == typeof(CrackInteractable)) return _CrackUI;

        //register it above --^
        Debug.LogError(type + " is not a registered proximity interactable. If you want to use it, register it HERE (<- click that)");
        return null;
    }

    #endregion

    #region Inventory
    //public void ToggleInventory()
    //{
    //    _InventoryUI.ToggleInventory();
    //}

    public void SetInventoryEnablity(bool setTo) => _InventoryUI.SetEnablity(setTo);
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

    #region Map Manager

    public void SetMapEnablity(bool setTo)
    {
        _MapManager.SetEnablityGetter(setTo, null);
    }
    public void SetMinimapEnablity(bool setTo)
    {
        _MapManager.SetMinimapEnablity(setTo);
    }

    #endregion

    #region Player Healthbar Manager

    public void SetHealthbarEnablity(bool setTo)
    {
        _PlayerHealthbarManager.SetEnablity(setTo);
    }

    #endregion

    #region Player Fuelbar Manager

    public void SetFuelbarEnablity(bool setTo)
    {
        _PlayerFuelbarManager.SetEnablity(setTo);
    }

    #endregion
}
