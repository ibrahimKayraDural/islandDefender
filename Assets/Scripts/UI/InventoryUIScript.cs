using Overworld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryUIScript : MonoBehaviour
{
    [SerializeField] GameObject _Visuals;
    [SerializeField] Transform _InventoryCellParent;
    [SerializeField] GameObject _CellPrefab;

    public void SetInventoryEnablity(bool setTo)
    {
        _Visuals.SetActive(false);
    }
    public void RefreshInventory(IInventoryItem[] items)
    {
        List<Transform> temp = _InventoryCellParent.Cast<Transform>().ToList();
        foreach (var item in temp)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in items)
        {
            GameObject cell = Instantiate(_CellPrefab, _InventoryCellParent);
            cell.GetComponent<TextMeshProUGUI>().text = item.Count.ToString();
        }
    }
}
