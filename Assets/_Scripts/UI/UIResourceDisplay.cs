using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResourceDisplay : MonoBehaviour
{
    [SerializeField] GameObject _resourceUIPrefab;
    [SerializeField] Transform _contentParent;

    Dictionary<ResourceData, TMP_Text> _resourceTexts = new();

    void Start()
    {
        var allResources = GLOBAL.GetResourceDatabase().DataList;

        foreach (var data in allResources)
        {
            var go = Instantiate(_resourceUIPrefab, _contentParent);
            var image = go.transform.Find("Icon").GetComponent<Image>();
            var text = go.transform.Find("CountText").GetComponent<TMP_Text>();

            image.sprite = data.UISprite;
            text.text = "0";

            _resourceTexts[data] = text;
        }

        RefreshAll();
    }

    public void RefreshAll()
    {
        foreach (var kvp in _resourceTexts)
        {
            int count = BaseResourceController.Instance.CheckItemCount(kvp.Key.AsItem());
            kvp.Value.text = count.ToString();
        }
    }
}