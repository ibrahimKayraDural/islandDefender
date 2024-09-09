using Overworld;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryQuickMenu : MonoBehaviour
{
    public static bool IsOpen => Instance != null;
    public static InventoryQuickMenu Instance { get; private set; } = null;

    [SerializeField] TextMeshProUGUI _TitleTM;
    [SerializeField] Transform _ButtonParent;
    [SerializeField] GameObject _ButtonPrefab;
    [SerializeField] VerticalLayoutGroup _ButtonParentGroup;

    Dictionary<string, UnityAction> Options;


    InventoryCellScript _item = null;
    bool _isMouseHovering = false;
    GraphicRaycasterScript _gRaycaster;

    List<KeyCode> _Keys_CloseOnNoHover = new List<KeyCode>()
    {
       KeyCode.Mouse0, KeyCode.Mouse1
    };
    List<KeyCode> _Keys_CloseAnytime = new List<KeyCode>()
    {
       KeyCode.Escape
    };

    private void Update()
    {
        if (_isMouseHovering == false)
        {
            foreach (var key in _Keys_CloseOnNoHover)
            {
                if (Input.GetKeyDown(key))
                {
                    Close();
                    break;
                }
            }
        }
        foreach (var key in _Keys_CloseAnytime)
        {
            if (Input.GetKeyDown(key))
            {
                Close();
                break;
            }
        }
    }

    public void Initialize(InventoryCellScript item, GraphicRaycasterScript grs, List<string> ButtonsToIgnore = null)
    {
        if (Instance == null) Instance = this;
        else if (Instance != this)
        {
            Instance.Close();
            Instance = this;
        }

        _item = item;
        _TitleTM.text = item.ItemData.DisplayName;

        _gRaycaster = grs;
        _gRaycaster.e_OnEventDataGathered += OnRaycastDataGathered;

        InitOptions(ButtonsToIgnore);
    }

    void InitOptions(List<string> ignoreList)
    {
        int buttonCount = 0;

        Options = new Dictionary<string, UnityAction>()
        {
            { "Drop", DropSelectedItem },
            { "Test", TestButton },
        };

        if (ignoreList == null) ignoreList = new List<string>();
        for (int i = 0; i < ignoreList.Count; i++)
        {
            ignoreList[i] = ignoreList[i].ToLower();
        }

        foreach (var pair in Options)
        {
            if (ignoreList.Contains(pair.Key.ToLower())) continue;

            SpawnButton(pair.Key);
        }

        if (buttonCount <= 0) Close();
        else
        {
            RectTransform buttonRT = _ButtonPrefab.GetComponent<RectTransform>();
            RectTransform titleRT = _TitleTM.gameObject.GetComponent<RectTransform>();
            RectTransform selfRT = GetComponent<RectTransform>();

            float targetHeight = buttonCount * (buttonRT.rect.height + _ButtonParentGroup.spacing);
            targetHeight += _ButtonParentGroup.padding.top + _ButtonParentGroup.padding.bottom;
            targetHeight += titleRT.rect.height;

            selfRT.sizeDelta = new Vector2(selfRT.sizeDelta.x, targetHeight);
        }

        void SpawnButton(string option)
        {
            GameObject buton = Instantiate(_ButtonPrefab, _ButtonParent);
            buton.GetComponentInChildren<TextMeshProUGUI>().text = option;
            buton.GetComponentInChildren<Button>().onClick.AddListener(Options[option]);

            buttonCount++;
        }
    }

    public void DropSelectedItem()
    {
        PlayerInstance.Instance.Inventory_Ref.DropItemAtIndex(_item.CellIndex);
        Close();
    }

    public void TestButton()
    {
        Debug.Log("test");
        Close();
    }

    public void Close()
    {
        _gRaycaster.e_OnEventDataGathered -= OnRaycastDataGathered;
        Instance = null;
        Destroy(gameObject);
    }

    void OnRaycastDataGathered(object sender, List<RaycastResult> e)
    {
        RaycastResult result = e.Find(x => x.gameObject.transform.IsChildOf(transform));
        _isMouseHovering = result.isValid;
    }
}
