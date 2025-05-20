using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SaveSystem;
using static UnityEngine.Rendering.HighDefinition.ScalableSettingLevelParameter;

public class ScannerManager : ProximityInteractableUI
{
    public const string SCANNERLEVELKEY = "scanner-level";
    public const string ALLOWEDLEVELKEY = "allowed-level";

    [SerializeField] ScannerUpgradeDatabase _scannerDatabase;

    [SerializeField] GameObject _scannerUpgradePanel;
    [SerializeField] GameObject _LockPanel;
    [SerializeField] Transform _costGroupParent;
    [SerializeField] GameObject _costItemPrefab;
    [SerializeField] Button _scanButton;
    [SerializeField] TextMeshProUGUI _infoText;
    [SerializeField] Button _backButton;

    BaseResourceController _resourceController => BaseResourceController.Instance;
    SaveManager _SaveManager
    {
        get
        {
            if (AUTO_saveManager == null)
                AUTO_saveManager = SaveManager.Instance;

            return AUTO_saveManager;
        }
    }
    SaveManager AUTO_saveManager = null;
    MapManager _mapManager => FindObjectOfType<MapManager>(true); // veya Singleton kullanılabilir

    int AllowedLevel = 0;

    void Awake()
    {
        _scanButton.onClick.AddListener(OnScanClicked);
    }

    public void Save()
    {
        _SaveManager?.AddOrReplaceSavedInteger(ALLOWEDLEVELKEY, AllowedLevel);
    }
    public void Load()
    {
        var save = _SaveManager?.CurrentSave?.SavedIntegers;
        if (save == null) return;

        var val = save.Find(x => x.ID == ALLOWEDLEVELKEY)?.Value;

        AllowedLevel = val ?? 0;
        UpdateUI();
    }
    int GetScannerLevel() => _SaveManager.CurrentSave.SavedIntegers.Find(x => x.ID == SCANNERLEVELKEY)?.Value ?? 0;
    void OnScanClicked()
    {
        var scannerLevel = GetScannerLevel();
        var nextData = _scannerDatabase.GetDataByLevel(scannerLevel + 1);
        if (nextData == null) return;

        if (!_resourceController.TrySpendResource(nextData.Costs.ToArray()))
        {
            _infoText.text = "<color=red>Not enough resources!";
            return;
        }

        _SaveManager.AddOrReplaceSavedInteger(SCANNERLEVELKEY, scannerLevel + 1);
        _mapManager?.RevealScannerIcons(nextData);
        UpdateUI();
    }

    void UpdateUI()
    {
        var scannerLevel = GetScannerLevel();
        var nextData = _scannerDatabase.GetDataByLevel(scannerLevel + 1);
        if (nextData == null)
        {
            foreach (Transform child in _costGroupParent) Destroy(child.gameObject);
            _scanButton.interactable = false;
            return;
        }

        foreach (Transform child in _costGroupParent) Destroy(child.gameObject);

        foreach (var cost in nextData.Costs)
        {
            GameObject go = Instantiate(_costItemPrefab, _costGroupParent);
            go.GetComponent<UICostItem>().Initialize(cost.Resource.UISprite, cost.Amount);
        }

        bool allow = AllowedLevel >= scannerLevel + 1;

        _LockPanel.SetActive(!allow);
        _scanButton.interactable = allow;
    }

    /// <summary>
    /// Set allowed level. Does not work if setTo is less than or equal to current AllowedLevel
    /// </summary>
    /// <param name="setTo">Value to set allowed level to</param>
    public void IncreasinglySetAllowedLevel(int setTo)
    {
        if (AllowedLevel >= setTo) return;

        AllowedLevel = setTo;
        UpdateUI();
    }
    public override void OnEnablityChanged(bool changedTo, List<string> optionalParameters = null)
    {
        _scannerUpgradePanel.SetActive(changedTo);
        if (changedTo)
        {
            UpdateUI();
        }
    }

    public void DisableUI() => SetEnablityGetter(false, null);

    internal override void OnPIUpdate_Start() { }
    internal override void OnPIUpdate_Loop() { }
    internal override void OnPIUpdate_End() { }
}

