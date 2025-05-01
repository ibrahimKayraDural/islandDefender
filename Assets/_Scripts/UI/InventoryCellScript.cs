using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Overworld;
using GameUI;

public class InventoryCellScript : UICell
{
    public int CellIndex = -1;

    public InventoryItem ItemData => _item;
    InventoryItem _item = null;

    [SerializeField] internal TextMeshProUGUI _CountTM;

    public void Initialize(InventoryItem item, int i, bool isInteractable = true, string ownerID = null)
    {
        _item = item;

        int count = item.Count;
        if (count > 0)
        {
            _CountTM.text = count.ToString();
            _CountTM.gameObject.SetActive(true);
        }

        CellIndex = i;

        base.Initialize(item.UISprite, _item.DisplayName, _item.Description, isInteractable, ownerID);
    }
    public override void Initialize()
    {
        CellIndex = -1;
        _item = null;
        _CountTM.text = "";

        base.Initialize();
    }
}
