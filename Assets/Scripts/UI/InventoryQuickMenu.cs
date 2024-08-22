using Overworld;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryQuickMenu : MonoBehaviour
{
    public static bool IsOpen => Instance != null;
    public static InventoryQuickMenu Instance = null;

    [SerializeField] TextMeshProUGUI _TitleTM;

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
        if(_isMouseHovering == false)
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

    public void Initialize(InventoryCellScript item, GraphicRaycasterScript grs)
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
    }

    public void DropSelectedItem()
    {
        Inventory.Instance.DropItemAtIndex(_item.CellIndex);
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
        RaycastResult result = e.Find(x=> x.gameObject.transform.IsChildOf(transform));
        _isMouseHovering = result.isValid;
    }
}
