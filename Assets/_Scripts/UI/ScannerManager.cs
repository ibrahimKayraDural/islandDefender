using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SaveSystem;


[CreateAssetMenu(fileName = "ScannerUpgradeData", menuName = "Upgrade/Scanner Upgrade Level", order = 0)]
public class ScannerUpgradeData : ScriptableObject
{
    public int ScannerLevel;
    public List<Cost> Costs;

    public int RevealMineCount = 1;
    public int RevealRuinCount = 1;
}

[CreateAssetMenu(fileName = "ScannerUpgradeDatabase", menuName = "Upgrade/Scanner Upgrade Database", order = 1)]
public class ScannerUpgradeDatabase : ScriptableObject
{
    public List<ScannerUpgradeData> Levels;

    public ScannerUpgradeData GetDataByLevel(int level)
    {
        return Levels.Find(x => x.ScannerLevel == level);
    }
}

public class ScannerManager : ProximityInteractableUI
{
    [SerializeField] ScannerUpgradeDatabase _scannerDatabase;
    
    [SerializeField] GameObject _scannerUpgradePanel;
    [SerializeField] Transform _costGroupParent;
    [SerializeField] GameObject _costItemPrefab;
    [SerializeField] Button _scanButton;
    [SerializeField] TextMeshProUGUI _infoText;
    [SerializeField] Button _backButton;
    
    BaseResourceController _resourceController => BaseResourceController.Instance;
    SaveManager _saveManager => SaveManager.Instance;
    MapManager _mapManager => FindObjectOfType<MapManager>(true); // veya Singleton kullanılabilir

    const string SaveKey = "ScannerLevel";
    int ScannerLevel => _saveManager.CurrentSave.SavedIntegers.Find(x => x.ID == SaveKey)?.Value ?? 0;

    void Awake()
    {
        _scanButton.onClick.AddListener(OnScanClicked);
        UpdateUI();
    }

    void OnScanClicked()
    {
        var nextData = _scannerDatabase.GetDataByLevel(ScannerLevel + 1);
        if (nextData == null) return;

        if (!_resourceController.TrySpendResource(nextData.Costs.ToArray()))
        {
            _infoText.text = "<color=red>Not enough resources!";
            return;
        }

        _saveManager.AddOrReplaceSavedInteger(SaveKey, ScannerLevel + 1);
        _mapManager?.RevealScannerIcons(nextData);
        UpdateUI();
    }

    void UpdateUI()
    {
    var nextData = _scannerDatabase.GetDataByLevel(ScannerLevel + 1);
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
    
    _scanButton.interactable = true;
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

